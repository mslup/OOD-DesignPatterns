using System.Diagnostics.CodeAnalysis;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ProjOb
{
    public interface IFilterable
    {
        string Representation { get; }
        string Type { get; }
        string ID { get; }
        IComparable GetFieldByName(string name);
        static string GetFieldTypeByName(string name) { return ""; }
    }

    public partial class Room
    {
        private Dictionary<string, IComparable> nameFieldPairs;
        private static Dictionary<string, string> nameTypePairs = new()
        {
            { "number", "int" },
            { "type", "enum" }
        };

        public string Type { get => "room"; }
        public string ID { get => Number.ToString(); }

        [MemberNotNull(nameof(nameFieldPairs))]
        public void InitDictionary()
        {
            nameFieldPairs = new()
            {
                { "number", Number },
                { "type", RoomType.ToString() }
            };
        }

        public IComparable GetFieldByName(string name)
        {
            return nameFieldPairs[name];
        }

        public static string GetFieldTypeByName(string name)
        {
            return nameTypePairs[name];
        }
    }

    public partial class RoomPartialTxtAdapter
    {
        private Dictionary<string, IComparable> nameFieldPairs;
        private static Dictionary<string, string> nameTypePairs = new()
        {
            { "number", "int" },
            { "type", "enum" }
        };
        public string Type { get => "room"; }
        public string ID { get => Number.ToString(); }

        [MemberNotNull(nameof(nameFieldPairs))]
        public void InitDictionary()
        {
            nameFieldPairs = new()
            {
                { "number", Number },
                { "type", RoomType.ToString() }
            };
        }

        public IComparable GetFieldByName(string name)
        {
            return nameFieldPairs[name];
        }

        public static string GetFieldTypeByName(string name)
        {
            return nameTypePairs[name];
        }
    }

    public partial class Course
    {
        private Dictionary<string, IComparable> nameFieldPairs;
        private static Dictionary<string, string> nameTypePairs = new()
        {
            { "name", "string" },
            { "code", "string" },
            { "duration", "int" }
        };
        public string Type { get => "course"; }
        public string ID { get => Code; }

        [MemberNotNull(nameof(nameFieldPairs))]
        public void InitDictionary()
        {
            nameFieldPairs = new()
            {
                { "number", Name },
                { "code", Code },
                { "duration", Duration },
            };
        }

        public IComparable GetFieldByName(string name)
        {
            return nameFieldPairs[name];
        }

        public static string GetFieldTypeByName(string name)
        {
            return nameTypePairs[name];
        }
    }

    public partial class CoursePartialTxtAdapter
    {
        private Dictionary<string, IComparable> nameFieldPairs;
        private static Dictionary<string, string> nameTypePairs = new()
        {
            { "name", "string" },
            { "code", "string" },
            { "duration", "int" }
        };
        public string Type { get => "course"; }
        public string ID { get => Code; }

        [MemberNotNull(nameof(nameFieldPairs))]
        public void InitDictionary()
        {
            nameFieldPairs = new()
            {
                { "number", Name },
                { "code", Code },
                { "duration", Duration },
            };
        }

        public IComparable GetFieldByName(string name)
        {
            return nameFieldPairs[name];
        }

        public static string GetFieldTypeByName(string name)
        {
            return nameTypePairs[name];
        }
    }

    public partial class Teacher
    {
        private Dictionary<string, IComparable> nameFieldPairs;
        private static Dictionary<string, string> nameTypePairs = new()
        {
            { "names", "string" },
            { "name", "string" },
            { "surname", "string" },
            { "rank", "enum" },
            { "code", "string" },
        };
        public string Type { get => "teacher"; }
        public string ID { get => Code; }

        [MemberNotNull(nameof(nameFieldPairs))]
        public void InitDictionary()
        {
            nameFieldPairs = new()
            {
                { "names", string.Join(" ", Names) },
                { "name", string.Join(" ", Names) },
                { "surname", Surname },
                { "rank", TeacherRank.ToString() },
                { "code", Code }
            };
        }

        public IComparable GetFieldByName(string name)
        {
            return nameFieldPairs[name];
        }

        public static string GetFieldTypeByName(string name)
        {
            return nameTypePairs[name];
        }
    }

    public partial class TeacherPartialTxtAdapter
    {
        private Dictionary<string, IComparable> nameFieldPairs;
        private static Dictionary<string, string> nameTypePairs = new()
        {
            { "names", "string" },
            { "name", "string" },
            { "surname", "string" },
            { "rank", "enum" },
            { "code", "string" },
        };

        public string Type { get => "teacher"; }
        public string ID { get => Code; }

        [MemberNotNull(nameof(nameFieldPairs))]
        public void InitDictionary()
        {
            nameFieldPairs = new()
            {
                { "names", string.Join(" ", Names) },
                { "name", string.Join(" ", Names) },
                { "surname", Surname },
                { "rank", TeacherRank.ToString() },
                { "code", Code }
            };
        }

        public IComparable GetFieldByName(string name)
        {
            return nameFieldPairs[name];
        }

        public static string GetFieldTypeByName(string name)
        {
            return nameTypePairs[name];
        }
    }

    public partial class Student
    {
        private Dictionary<string, IComparable> nameFieldPairs;
        private static Dictionary<string, string> nameTypePairs= new()
        {
            { "names", "string" },
            { "name", "string" },
            { "surname", "string" },
            { "semester", "int" },
            { "code", "string" },
        };

        public string Type { get => "student"; }
        public string ID { get => Code; }

        [MemberNotNull(nameof(nameFieldPairs))]
        public void InitDictionary()
        {
            nameFieldPairs = new()
            {
                { "names", string.Join(" ", Names) },
                { "name", string.Join(" ", Names) },
                { "surname", Surname },
                { "semester", Semester },
                { "code", Code }
            };
        }

        public IComparable GetFieldByName(string name)
        {
            return nameFieldPairs[name];
        }

        public static string GetFieldTypeByName(string name)
        {
            return nameTypePairs[name];
        }
    }

    public partial class StudentPartialTxtAdapter
    {
        private Dictionary<string, IComparable> nameFieldPairs;
        private static Dictionary<string, string> nameTypePairs = new()
        {
            { "names", "string" },
            { "surname", "string" },
            { "semester", "int" },
            { "code", "string" },
        };

        public string Type { get => "student"; }
        public string ID { get => Code; }

        [MemberNotNull(nameof(nameFieldPairs))]
        public void InitDictionary()
        {
            nameFieldPairs = new()
            {
                { "names", string.Join(" ", Names) },
                { "surname", Surname },
                { "semester", Semester },
                { "code", Code }
            };
        }

        public IComparable GetFieldByName(string name)
        {
            return nameFieldPairs[name];
        }

        public static string GetFieldTypeByName(string name)
        {
            return nameTypePairs[name];
        }
    }
}
