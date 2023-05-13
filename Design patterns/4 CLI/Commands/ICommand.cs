using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace ProjOb
{
    public interface ICommand
    {
        public string Arguments { get; set; }
        public bool Preprocess();
        public void Execute();
        public string ToString();
    }

    [DataContract]
    public abstract class AbstractCommand
    {
        protected static Dictionary<string, Func<IBuilder>> BuilderDictionary
            = new()
            {
                ["room"] = () => new RoomBuilder(),
                ["course"] = () => new CourseBuilder(),
                ["teacher"] = () => new TeacherBuilder(),
                ["student"] = () => new StudentBuilder()
            };

        [DataContract, KnownType(typeof(Predicate))]
        protected class Predicate
        {
            private static Dictionary<string, Func<string, string>> typeDictionary
            = new Dictionary<string, Func<string, string>>
            {
                ["room"] = Room.GetFieldTypeByName,
                ["rooms"] = Room.GetFieldTypeByName,
                ["course"] = Course.GetFieldTypeByName,
                ["courses"] = Course.GetFieldTypeByName,
                ["teacher"] = Teacher.GetFieldTypeByName,
                ["teachers"] = Teacher.GetFieldTypeByName,
                ["student"] = Student.GetFieldTypeByName,
                ["students"] = Student.GetFieldTypeByName
            };

            [DataMember] private string Operator { get; set; }
            [DataMember] private string FieldName { get; set; }
            [DataMember] private IComparable? Value { get; set; }
            private string FieldType { get; set; }
            private string ValueString { get; set; }

            public Predicate(string objectType, string requirement)
            {
                string[] parts = Regex.Split(requirement, @"([<=>])");
                if (parts.Length != 3)
                {
                    throw new ArgumentException($"Unable to parse requirement: '{requirement}'");
                }

                FieldName = parts[0];
                if (!typeDictionary.TryGetValue(objectType, out Func<string, string>? getter))
                {
                    throw new KeyNotFoundException($"Unrecognized class name: '{objectType}'");
                }

                try
                {
                    FieldType = getter(FieldName);
                }
                catch (KeyNotFoundException)
                {
                    throw new KeyNotFoundException($"Unrecognized field name: '{FieldName}'");
                }
                Operator = parts[1];
                ValueString = parts[2];
                ParseValue();
            }
            private void ParseValue()
            {
                ValueString = ValueString.TrimWQ();

                switch (FieldType)
                {
                case "int":
                    if (int.TryParse(ValueString, out int intValue))
                        Value = intValue;
                    break;
                case "double":
                    if (double.TryParse(ValueString, out double doubleValue))
                        Value = doubleValue;
                    break;
                case "bool":
                    if (bool.TryParse(ValueString, out bool boolValue))
                        Value = boolValue;
                    break;
                case "string":
                case "enum":
                    Value = ValueString;
                    break;
                default:
                    throw new ArgumentException($"Unable to parse '{ValueString}'");
                }

                if (Value == null)
                    throw new ArgumentException($"Unable to parse '{ValueString}'");
            }
            public bool Compare(IFilterable? obj)
            {
                if (obj == null)
                    return false;

                if (Value == null)
                    return false;

                switch (Operator)
                {
                case "=":
                    return obj.GetFieldByName(FieldName).Equals(Value);
                case "<":
                    return obj.GetFieldByName(FieldName).CompareTo(Value) < 0;
                case ">":
                    return obj.GetFieldByName(FieldName).CompareTo(Value) > 0;
                }
                return false;

                // throw...?
            }
        }

        protected bool FindCollection(string arg)
        {
            if (!Dictionaries.objectDictionary.ContainsKey(arg))
            {
                Console.WriteLine($"Unrecognized object type: '{arg}'");
                return false;
            }

            return true;
        }

        protected bool FindCollection(string arg, [NotNullWhen(true)] out IDictionary? collection)
        {
            if (!Dictionaries.objectDictionary.TryGetValue(arg, out collection))
            {
                Console.WriteLine($"Unrecognized object type: '{arg}'");
                return false;
            }

            if (collection == null)
            {
                Console.WriteLine("Collection retrieval failed");
                return false;
            }

            return true;
        }

        protected void PrintCollection(IDictionary? collection)
        {
            if (collection == null)
                throw new NullReferenceException();

            if (collection.Count == 0)
            {
                Console.WriteLine("No objects found");
                return;
            }

            foreach (DictionaryEntry obj in collection)
            {
                Console.WriteLine(obj.Value);
            }
        }

        protected void PrintCollection<T>(INewCollection<T> collection)
        {
            if (collection == null)
                throw new NullReferenceException();

            if (collection.Count == 0)
            {
                Console.WriteLine("No objects found");
                return;
            }

            CollectionAlgorithms.Print(collection);
        }

        protected bool CheckRepresentation(string rep)
        {
            if (!Dictionaries.Representations.Contains(rep))
            {
                Console.WriteLine($"Unrecognized representation: {rep}." +
                    $"Available represenations: {string.Join(", ", Dictionaries.Representations)}");
                return false;
            }
            return true;
        }

        private static Regex RequirementRegex =
            new Regex(@"\S+\s*[=><]\s*(?:"".*?""|'.*?'|\S+)", RegexOptions.Compiled);

        protected bool ParseRequirements(string arguments, string type,
            out List<Predicate> predicates)
        {
            var requirementMatches = RequirementRegex.Matches(arguments);

            predicates = new();
            foreach (Match match in requirementMatches)
            {
                try
                {
                    predicates.Add(new Predicate(type, match.Value));
                }
                catch (Exception ex) when (
                    ex is ArgumentException ||
                    ex is KeyNotFoundException)
                {
                    Console.WriteLine(ex.Message);
                    return false;
                }
            }

            return true;
        }

        // maybe return bool and vector as out parameter
        protected Vector<IFilterable>? RunPredicates(IDictionary? iteratedObjects,
            List<Predicate>? predicates, bool CheckIfUnique = false)
        {
            if (iteratedObjects == null || predicates == null)
                return null;

            if (CheckIfUnique && iteratedObjects.Count > 1 && predicates.Count == 0)
            {
                Console.WriteLine("Provide requirements to uniquely identify an object");
                return null;
            }

            Vector<IFilterable>? result = new();

            foreach (DictionaryEntry pair in iteratedObjects)
            {
                bool predicatesSatisfied = true;
                foreach (var predicate in predicates)
                {
                    try
                    {
                        if (!predicate.Compare(pair.Value as IFilterable))
                        {
                            predicatesSatisfied = false;
                            break;
                        }
                    }
                    catch (Exception ex) when (
                        ex is ArgumentException ||
                        ex is KeyNotFoundException)
                    {
                        Console.WriteLine("Invalid argument");
                        return null;
                    }
                }

                if (!predicatesSatisfied)
                    continue;

                if (CheckIfUnique && result.Count > 0)
                {
                    Console.WriteLine("Set requirements don't uniquely identify one object");
                    return null;
                }

                if (pair.Value is IFilterable obj)
                    result.Add(obj);
            }

            return result;
        }

        protected bool GetBuilder(string objectType, [NotNullWhen(true)] out IBuilder? builder)
        {
            if (!BuilderDictionary.TryGetValue(objectType,
                out Func<IBuilder>? constructor))
            {
                Console.WriteLine("Unrecognized object type");
                builder = null;
                return false;
            }

            if (constructor == null)
                throw new NullReferenceException();

            builder = constructor();
            return true;
        }

        protected enum BuilderType { Create, Edit };

        protected bool FillBuilder(ref IBuilder builder, BuilderType type = BuilderType.Create)
        {
            string action = "";
            switch (type)
            {
            case BuilderType.Create:
                action = "creation"; break;
            case BuilderType.Edit:
                action = "edition"; break;
            default:
                Console.WriteLine("Internal error. Wrong `FillBuilder` method call");
                return false;
            }

            Console.WriteLine("Available fields: '" +
               string.Join(", ", builder.Setters.Select(x => x.Key)) +
               "'");
            Console.WriteLine($"Type DONE to confirm {action} or EXIT to abandon.");

            string? input;
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("* ");
                Console.ResetColor();

                input = Console.ReadLine();

                if (input == null || input.Length == 0)
                    continue;

                if (input.ToLower() == "done")
                {
                    Console.WriteLine($"Object {action} request accepted.");
                    return true;
                }
                else if (input.ToLower() == "exit")
                {
                    Console.WriteLine($"Object {action} creation abandoned.");
                    return false;
                }

                string[] nameValue = input.Trim().Split("=");
                if (nameValue.Length != 2)
                {
                    Console.WriteLine("Invalid argument");
                    continue;
                }

                // trygetvalue ...
                try
                {
                    builder.Setters[nameValue[0].Trim()](nameValue[1].TrimWQ());
                }
                catch (Exception ex) when (
                    ex is ArgumentException ||
                    ex is KeyNotFoundException)
                {
                    Console.WriteLine("Invalid argument");
                }
            }
        }
    }

}
