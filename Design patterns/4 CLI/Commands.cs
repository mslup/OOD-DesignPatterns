using System.Collections;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace ProjOb
{
    public interface ICommand
    {
        public string Arguments { get; set; }
        public void Execute();

        //public string ToString();
    }

    public class CommandList : ICommand
    {
        public string Arguments { get; set; }

        public CommandList() => Arguments = "";

        public void Execute()
        {
            if (!Dictionaries.objectDictionary.TryGetValue(Arguments, out IDictionary? dict))
            {
                Console.WriteLine("Invalid argument");
                Console.WriteLine(this);
                return;
            }

            if (dict == null || dict.Count == 0)
            {
                Console.WriteLine("No objects found");
                return;
            }

            foreach (DictionaryEntry obj in dict)
            {
                Console.WriteLine(obj.Value);
            }
        }
        public override string ToString()
        {
            // TODO
            return "NAME".Bold() + "\r\n" +
                "   list - prints all objects of a particular type\r\n" +
                "\r\nThe format of the command should be as follows:\r\n" +
                "\r\n    list <name_of_the_class>";
        }
    }

    public class CommandFind : ICommand
    {
        public string Arguments { get; set; }
        public CommandFind() => Arguments = "";

        private class Comparison
        {
            public Comparison(string requirement)
            {
                string[] parts = Regex.Split(requirement, @"([<=>])");
                // what if set name has an equal sign
                if (parts.Length != 3)
                {
                    throw new ArgumentException();
                }

                FieldName = parts[0];
                FieldType = "";
                Operator = parts[1];
                ValueString = parts[2];
                Value = null;
                ValueParsed = false;
            }

            private string Operator { get; set; }
            private string FieldName { get; set; }
            private IComparable? Value { get; set; }
            private string ValueString { get; set; }
            private string FieldType { get; set; }
            private bool ValueParsed;
            private void ParseValue(IFilterable obj)
            {
                ValueParsed = true;

                if (ValueString.StartsWith("\"") && ValueString.EndsWith("\""))
                    ValueString = ValueString.Substring(1, ValueString.Length - 2);

                FieldType = obj.GetFieldTypeByName(FieldName);

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

            public bool Compare(IFilterable? left)
            {
                if (left == null)
                    return false;

                if (!ValueParsed)
                    ParseValue(left);

                if (Value == null)
                    return false;

                switch (Operator)
                {
                case "=":
                    return left.GetFieldByName(FieldName).Equals(Value);
                case "<":
                    return left.GetFieldByName(FieldName).CompareTo(Value) < 0;
                case ">":
                    return left.GetFieldByName(FieldName).CompareTo(Value) > 0;
                }
                return false;

                // throw...?
            }
        }

        public void Execute()
        {
            string[] tokens = Arguments.Split(' ', 2);

            if (!Dictionaries.objectDictionary.TryGetValue(tokens[0],
                out IDictionary? iteratedObjects))
            {
                Console.WriteLine("Invalid argument");
                Console.WriteLine(this);
                return;
            }

            if (iteratedObjects == null || iteratedObjects.Count == 0)
            {
                Console.WriteLine("No objects found");
                return;
            }

            // handle no requierements... & merge with list
            var requirements = new Regex(@"\S+\s*[=><]\s*(?:"".*?""|'.*?'|\S+)")
                .Matches(tokens[1]);
            var predicates = new List<Comparison>();

            foreach (Match requirement in requirements)
            {
                try
                {
                    var comparer = new Comparison(requirement.Value);
                    predicates.Add(comparer);
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("Invalid argument");
                    return;
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
                        return;
                    }

                }
                
                if (predicatesSatisfied)
                    Console.WriteLine(pair.Value);
            }
        }

    }

    public class CommandAdd : ICommand
    {
        
        public string Arguments { get; set; }
        public CommandAdd() => Arguments = "";
        public void Execute() 
        {
            string[] tokens = Arguments.Split(' ');
            if (tokens.Length > 2)
            {
                Console.WriteLine("Invalid arguments");
                return;
            }

            var builder = new RoomBuilder();

            string? input = "";
            Console.WriteLine("Available fields: '" +
                string.Join(", ", builder.fieldSetterPairs.Select(x => x.Key)) +
                "'");
            Console.WriteLine("Type DONE to confirm creation or EXIT to abandon.");
            while (true)
            {
                Console.Write(CLI.Prompt);
                input = Console.ReadLine();

                if (input == null || input.Length == 0)
                    continue;

                if (input == "DONE")
                {
                    builder.Build();
                    Console.WriteLine("Object created.");
                    break;
                }
                if (input == "EXIT")
                {
                    builder.Build();
                    Console.WriteLine("Object creation abandoned.");
                    break;
                }

                string[] nameValue = input.Trim().Split("=");
                if (nameValue.Length != 2)
                {
                    Console.WriteLine("Invalid argument");
                    continue;
                }

                try
                {
                    builder.fieldSetterPairs[nameValue[0].Trim()](nameValue[1].Trim());
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
