using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.RegularExpressions;

namespace ProjOb
{
    public interface ICommand
    {
        public string Arguments { get; set; }
        public bool Preprocess();
        public void Execute();
        public string ToString();
    }
    public class CommandLogic
    {
        public static Dictionary<string, Func<IBuilder>> BuilderDictionary
            = new Dictionary<string, Func<IBuilder>>
            {
                ["room"] = () => new RoomBuilder(),
                ["course"] = () => new CourseBuilder(),
                ["teacher"] = () => new TeacherBuilder(),
                ["student"] = () => new StudentBuilder()
            };

        public class Requirement
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

            public Requirement(string objectType, string requirement)
            {
                string[] parts = Regex.Split(requirement, @"([<=>])");
                if (parts.Length != 3)
                {
                    throw new ArgumentException();
                }

                FieldName = parts[0];
                FieldType = typeDictionary[objectType](FieldName);
                Operator = parts[1];
                ValueString = parts[2];
                ParseValue();
            }

            private string Operator { get; set; }
            private string FieldName { get; set; }
            private IComparable? Value { get; set; }
            private string ValueString { get; set; }
            private string FieldType { get; set; }
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
                    throw new ArgumentException();
                }

                if (Value == null)
                    throw new ArgumentException();
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
    }
    public class CommandList : ICommand
    {
        public string Arguments { get; set; }

        public CommandList() => Arguments = "";

        private IDictionary? objectsToList;

        public bool Preprocess()
        {
            if (!Dictionaries.objectDictionary.TryGetValue(Arguments, out objectsToList))
            {
                Console.WriteLine("Invalid argument");
                return false;
            }
            return true;
        }

        public void Execute()
        {
            if (objectsToList == null || objectsToList.Count == 0)
            {
                Console.WriteLine("No objects found");
                return;
            }

            foreach (DictionaryEntry obj in objectsToList)
            {
                Console.WriteLine(obj.Value);
            }
        }

        public override string ToString()
        {
            return "list " + Arguments;
        }
    }
    public class CommandFind : CommandLogic, ICommand
    {
        public string Arguments { get; set; }
        public CommandFind()
        {
            Arguments = "";
            foundObjectsCollection = new Vector<IFilterable>();
        }

        private Vector<IFilterable> foundObjectsCollection;

        public bool Preprocess()
        {
            string[] tokens = Arguments.Split(' ', 2);
            string objectType = tokens[0];

            if (!Dictionaries.objectDictionary.TryGetValue(objectType,
                out IDictionary? iteratedObjects))
            {
                Console.WriteLine("Invalid argument");
                return false;
            }

            if (iteratedObjects == null || iteratedObjects.Count == 0)
            {
                Console.WriteLine("No objects found");
                return false;
            }

            // TODO
            if (tokens.Length < 2)
            {
                var fct = new CommandFactory();
                var cmd = fct.BuildCommand("list " + Arguments);
                cmd.Execute();
                return false;
            }

            var requirements = new Regex(@"\S+\s*[=><]\s*(?:"".*?""|'.*?'|\S+)")
                .Matches(tokens[1]);
            var predicates = new List<Requirement>();

            foreach (Match requirement in requirements)
            {
                try
                {
                    var comparer = new Requirement(objectType, 
                        requirement.Value);
                    predicates.Add(comparer);
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("Invalid argument");
                    return false;
                }
            }

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
                        return false;
                    }
                }

                if (predicatesSatisfied && (pair.Value is IFilterable obj))
                    foundObjectsCollection.Add(obj);
            }

            return true;
        }

        public void Execute()
        {
            // change IteratorTest name
            IteratorTest.Print(foundObjectsCollection);
        }

        public override string ToString()
        {
            return "find " + Arguments;
        }
    }
    public class CommandAdd : CommandLogic, ICommand
    {
        public string Arguments { get; set; }
        private string Representation;
        public CommandAdd() => Arguments = "";

        private IBuilder builder;
        public bool Preprocess()
        {
            string[] tokens = Arguments.Split(' ');
            if (tokens.Length > 2)
            {
                Console.WriteLine("Invalid arguments");
                return false;
            }

            Representation = tokens.Length == 2 ? tokens[1] : "base";
            if (!Dictionaries.Representations.Contains(Representation)) 
            {
                Console.WriteLine($"Unrecognized representation: {Representation}." +
                    $"Available represenations: {
                        string.Join(", ", Dictionaries.Representations)}");
                return false;
            }

            try
            {
                builder = BuilderDictionary[tokens[0]]();
            }
            catch (KeyNotFoundException)
            {
                Console.WriteLine("Invalid argument");
                return false;
            }
            //gettryvalue...

            string? input = "";
            Console.WriteLine("Available fields: '" +
                string.Join(", ", builder.Setters.Select(x => x.Key)) +
                "'");
            Console.WriteLine("Type DONE to confirm creation or EXIT to abandon.");

            while (true)
            {
                Console.Write("~ ");
                input = Console.ReadLine();

                if (input == null || input.Length == 0)
                    continue;

                if (input.ToLower() == "done")
                {
                    Console.WriteLine("Object creation request accepted.");
                    return true;
                }
                else if (input.ToLower() == "exit")
                {
                    Console.WriteLine("Object creation abandoned.");
                    return false;
                }

                string[] nameValue = input.Trim().Split("=");
                if (nameValue.Length != 2)
                {
                    Console.WriteLine("Invalid argument");
                    continue;
                }

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

        public void Execute()
        {
            Console.WriteLine("Added object:");
            Console.WriteLine(builder.BuildMethods[Representation]());
        }

        public override string ToString()
        {
            return "add " + Arguments;
        }
    }
    public class CommandEdit : CommandLogic, ICommand
    {
        public string Arguments { get; set; }
        private IBuilder? builder;
        private IFilterable? found;
        public CommandEdit() => Arguments = "";

        private IFilterable? Find()
        {
            string[] tokens = Arguments.Split(' ', 2);
            string objectType = tokens[0];

            if (!Dictionaries.objectDictionary.TryGetValue(tokens[0],
                out IDictionary? iteratedObjects))
            {
                Console.WriteLine("Invalid argument");
                return null;
            }

            if (iteratedObjects == null || iteratedObjects.Count == 0)
            {
                Console.WriteLine("No objects found");
                return null;
            }

            var requirements = new Regex(@"\S+\s*[=><]\s*(?:"".*?""|'.*?'|\S+)")
               .Matches(tokens[1]);
            var predicates = new List<Requirement>();

            foreach (Match requirement in requirements)
            {
                try
                {
                    var comparer = new Requirement(objectType
                        , requirement.Value);
                    predicates.Add(comparer);
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("Invalid argument");
                    return null;
                }
            }

            IFilterable? ret = null;

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

                if (predicatesSatisfied)
                {
                    if (ret == null)
                    {
                        ret = pair.Value as IFilterable;
                    }
                    else
                    {
                        Console.WriteLine("Set requirements don't uniquely identify one object");
                        return null;
                    }
                }
            }

            return ret;
        }

        public bool Preprocess()
        {
            found = Find();
            if (found == null)
            {
                Console.WriteLine("No objects found");
                Console.Beep();
                return false;
            }

            string[] tokens = Arguments.Split(' ', 2);
            string objectName = tokens[0];

            IBuilder builder;
            try
            {
                builder = BuilderDictionary[tokens[0]]();
            }
            catch (KeyNotFoundException)
            {
                Console.WriteLine("Invalid argument");
                return false;
            }

            string? input = "";
            Console.WriteLine("Available fields: '" +
                string.Join(", ", builder.Setters.Select(x => x.Key)) +
                "'");
            Console.WriteLine("Type DONE to confirm creation or EXIT to abandon.");
            
            while (true)
            {
                Console.Write("~ ");
                input = Console.ReadLine();

                if (input == null || input.Length == 0)
                    continue;

                if (input.ToLower() == "done")
                {
                    Console.WriteLine("Object edition request accepted.");
                    return true;
                }
                else if (input.ToLower() == "exit")
                {
                    Console.WriteLine("Object edition abandoned.");
                    return false;
                }

                string[] nameValue = input.Trim().Split("=");
                if (nameValue.Length != 2)
                {
                    Console.WriteLine("Invalid argument");
                    continue;
                }

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

        public void Execute()
        {
            Console.WriteLine("Some sensible edit message");
            if (found != null)
                builder.Update(found);
        }

        public override string ToString()
        {
            return "edit " + Arguments;
        }
    }

}
