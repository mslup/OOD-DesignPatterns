using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ProjOb
{
    public interface IFilterable
    {
        string Representation { get; }
        IComparable GetFieldByName(string name);
        string GetFieldTypeByName(string name);
    }

    public partial class Room
    {
        private Dictionary<string, IComparable> nameFieldPairs;
        private static Dictionary<string, string> nameTypePairs
            = new Dictionary<string, string>
            {
                { "number", "int" },
                { "type", "enum" }
            };

        [MemberNotNull(nameof(nameFieldPairs))]
        public void InitDictionary()
        {
            nameFieldPairs = new Dictionary<string, IComparable>
            {
                { "number", Number },
                { "type", RoomType.ToString() }
            };
        }

        public IComparable GetFieldByName(string name)
        {
            return nameFieldPairs[name];
        }

        public string GetFieldTypeByName(string name)
        {
            return nameTypePairs[name];
        }
    }

    public partial class RoomPartialTxtAdapter
    {
        private Dictionary<string, IComparable> nameFieldPairs;
        private static Dictionary<string, string> nameTypePairs
            = new Dictionary<string, string>
            {
                { "number", "int" },
                { "type", "enum" }
            };

        [MemberNotNull(nameof(nameFieldPairs))]
        public void InitDictionary()
        {
            nameFieldPairs = new Dictionary<string, IComparable>
            {
                { "number", Number },
                { "type", RoomType.ToString() }
            };
        }

        public IComparable GetFieldByName(string name)
        {
            return nameFieldPairs[name];
        }

        public string GetFieldTypeByName(string name)
        {
            return nameTypePairs[name];
        }
    }

    public partial class Course
    {
        private Dictionary<string, IComparable> nameFieldPairs;
        private static Dictionary<string, string> nameTypePairs
            = new Dictionary<string, string>
            {
                { "name", "string" },
                { "code", "string" },
                { "duration", "int" }
            };

        [MemberNotNull(nameof(nameFieldPairs))]
        public void InitDictionary()
        {
            nameFieldPairs = new Dictionary<string, IComparable>
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

        public string GetFieldTypeByName(string name)
        {
            return nameTypePairs[name];
        }
    }

    public partial class CoursePartialTxtAdapter
    {
        private Dictionary<string, IComparable> nameFieldPairs;
        private static Dictionary<string, string> nameTypePairs
            = new Dictionary<string, string>
            {
                { "name", "string" },
                { "code", "string" },
                { "duration", "int" }
            };

        [MemberNotNull(nameof(nameFieldPairs))]
        public void InitDictionary()
        {
            nameFieldPairs = new Dictionary<string, IComparable>
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

        public string GetFieldTypeByName(string name)
        {
            return nameTypePairs[name];
        }
    }

    public partial class Teacher
    {
        private Dictionary<string, IComparable> nameFieldPairs;
        private static Dictionary<string, string> nameTypePairs
            = new Dictionary<string, string>
            {
                { "names", "string" },
                { "name", "string" },
                { "surname", "string" },
                { "rank", "enum" },
                { "code", "string" },
            };

        [MemberNotNull(nameof(nameFieldPairs))]
        public void InitDictionary()
        {
            nameFieldPairs = new Dictionary<string, IComparable>
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

        public string GetFieldTypeByName(string name)
        {
            return nameTypePairs[name];
        }
    }

    public partial class TeacherPartialTxtAdapter
    {
        private Dictionary<string, IComparable> nameFieldPairs;
        private static Dictionary<string, string> nameTypePairs
            = new Dictionary<string, string>
            {
                { "names", "string" },
                { "name", "string" },
                { "surname", "string" },
                { "rank", "enum" },
                { "code", "string" },
            };

        [MemberNotNull(nameof(nameFieldPairs))]
        public void InitDictionary()
        {
            nameFieldPairs = new Dictionary<string, IComparable>
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

        public string GetFieldTypeByName(string name)
        {
            return nameTypePairs[name];
        }
    }

    public partial class Student
    {
        private Dictionary<string, IComparable> nameFieldPairs;
        private static Dictionary<string, string> nameTypePairs
            = new Dictionary<string, string>
            {
                { "names", "string" },
                { "name", "string" },
                { "surname", "string" },
                { "semester", "int" },
                { "code", "string" },
            };

        [MemberNotNull(nameof(nameFieldPairs))]
        public void InitDictionary()
        {
            nameFieldPairs = new Dictionary<string, IComparable>
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

        public string GetFieldTypeByName(string name)
        {
            return nameTypePairs[name];
        }
    }

    public partial class StudentPartialTxtAdapter
    {
        private Dictionary<string, IComparable> nameFieldPairs;
        private static Dictionary<string, string> nameTypePairs
            = new Dictionary<string, string>
            {
                { "names", "string" },
                { "surname", "string" },
                { "semester", "int" },
                { "code", "string" },
            };

        [MemberNotNull(nameof(nameFieldPairs))]
        public void InitDictionary()
        {
            nameFieldPairs = new Dictionary<string, IComparable>
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

        public string GetFieldTypeByName(string name)
        {
            return nameTypePairs[name];
        }
    }
}
