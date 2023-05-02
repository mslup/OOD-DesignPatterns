using System.Collections;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace ProjOb
{
    public interface ICommand
    {
        public string Argument { get; set; }
        public void Execute();

        //public string ToString();
    }

    public class CommandList : ICommand
    {
        public string Argument { get; set; }

        public CommandList() => Argument = "";

        public void Execute()
        {
            if (!Dictionaries.objectDictionary.TryGetValue(Argument, out IDictionary? dict))
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
        public string Argument { get; set; }
        public CommandFind() => Argument = "";

        private class Comparison
        {
            public Comparison(string requirement)
            {
                string[] parts = Regex.Split(requirement, @"([<=>])");
                if (parts.Length != 3)
                {
                    throw new ArgumentException();
                }

                FieldName = parts[0];
                FieldType = "";
                Operator = parts[1];
                ValueString = parts[2];
                Value = null;
                ValueParsed = false; ;
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
            string[] tokens = Argument.Split(' ', 2);

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

            // handle no requierements...
            // handle quotes...
            string[] requirements = tokens[1].Split(' ');
            var predicates = new List<Comparison>();

            foreach (var requirement in requirements)
            {
                try
                {
                    var comparer = new Comparison(requirement);
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
                bool predicateSatisfied = true;
                foreach (var predicate in predicates)
                {
                    try
                    {
                        if (!predicate.Compare(pair.Value as IFilterable))
                            predicateSatisfied = false;
                    }
                    catch (Exception ex) when (
                        ex is ArgumentException ||
                        ex is KeyNotFoundException)
                    {
                        Console.WriteLine("Invalid argument");
                        return;
                    }

                }
                
                if (predicateSatisfied)
                    Console.WriteLine(pair.Value);
            }
        }

    }

    public class CommandAdd : ICommand
    {
        public string Argument { get; set; }
        public CommandAdd() => Argument = "";
        public void Execute() 
        { 
            
        }
    }
}
