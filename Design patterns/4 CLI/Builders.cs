using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ProjOb
{
    // issues:
    // in each TryParse keep old value and assign if parse unsuccesful
    // refactor marker interface
    // refactor PartialTxtBuilders...

    public abstract class IBuilder
    {
        public abstract Dictionary<string, Action<string>>
            fieldSetterPairs { get; }          
        public abstract object Build(); // refactor 'object'
    }

    public class RoomBuilder : IBuilder
    {
        protected int Number;
        protected IRoom.RoomTypeEnum RoomType;
        public override Dictionary<string, Action<string>> fieldSetterPairs 
        { get; }           

        public RoomBuilder() 
        {
            Number = 0;
            RoomType = IRoom.RoomTypeEnum.other;
            fieldSetterPairs = new Dictionary<string, Action<string>>
            {
                { "number", new Action<string>(SetNumber) },
                { "type", new Action<string>(SetRoomType) },
            };
        }

        override public object Build()
        {
            return new Room(Number, RoomType);
        }

        public void SetNumber(string value)
        {
            if (value == null)
                throw new ArgumentException();

            if (!Number.SafeTryParse(value))
                throw new ArgumentException();
        }

        public void SetRoomType(string value)
        {
            if (value == null)
                throw new ArgumentException();

            // unwanted behaviour when value > 4
            if (!RoomType.SafeTryParse(value))
                throw new ArgumentException();
        }
    }

    public class RoomPartialTxtBuilder : RoomBuilder
    {
        override public object Build()
        {
            return new RoomPartialTxt(Number, RoomType.ToString(), "");
        }
    }

    public class CourseBuilder : IBuilder
    {
        protected string Name;
        protected string Code;
        protected int Duration;
        public override Dictionary<string, Action<string>> fieldSetterPairs
        { get; }

        public CourseBuilder()
        {
            Name = "";
            Code = "";
            Duration = 0;
            fieldSetterPairs = new Dictionary<string, Action<string>>
            {
                { "name", new Action<string>(SetName) },
                { "code", new Action<string>(SetCode) },
                { "duration", new Action<string>(SetDuration) }
            };
        }

        override public object Build()
        {
            return new Course(Name, Code, Duration);
        }

        public void SetName(string value)
        {
            if (value == null)
                throw new ArgumentException();

            Name = value;
        }

        public void SetCode(string value)
        {
            if (value == null)
                throw new ArgumentException();

            Code = value;
        }

        public void SetDuration (string value) 
        {
            if (value == null)
                throw new ArgumentException();

            if (!Duration.SafeTryParse(value))
                throw new ArgumentException();
        }
    }

    public class CoursePartialTxtBuilder : CourseBuilder
    {
        override public object Build()
        {
            return new CoursePartialTxt(Name, Code, Duration, "");
        }
    }

    public class TeacherBuilder : IBuilder
    {
        protected string[] Names;
        protected string Surname;
        protected string Code;
        protected ITeacher.TeacherRankEnum TeacherRank;
        public override Dictionary<string, Action<string>> fieldSetterPairs
        { get; }
        public TeacherBuilder()
        {
            Names = new string[] { "" };
            Surname = "";
            Code = "";
            TeacherRank = ITeacher.TeacherRankEnum.KiB;
            fieldSetterPairs = new Dictionary<string, Action<string>>
            {
                { "names", new Action<string>(SetNames) },
                { "surname", new Action<string>(SetSurname) },
                { "code", new Action<string>(SetCode) },
                { "rank", new Action<string>(SetTeacherRank) }
            };
        }

        override public object Build()
        {
            return new Teacher(Names, Surname, TeacherRank, Code);
        }

        public void SetNames(string value)
        {
            if (value == null)
                throw new ArgumentException();

            Names = value.Split(",")
                    .Select(str => str.Trim())
                    .ToArray();
        }

        public void SetSurname(string value)
        {
            if (value == null)
                throw new ArgumentException();

            Surname = value;
        }

        public void SetCode(string value)
        {
            if (value == null)
                throw new ArgumentException();

            Code = value;
        }

        public void SetTeacherRank(string value)
        {
            if (value == null)
                throw new ArgumentException();

            // unwanted behaviour same as in RoomBuilder
            if (!TeacherRank.SafeTryParse(value))
                throw new ArgumentException();
        }
    }

    public class TeacherPartialTxtBuilder : TeacherBuilder
    {
        public override object Build()
        {
            return new TeacherPartialTxt(Surname + "," + string.Join(",", Names),
                TeacherRank.ToString(), Code, "");
        }
    }

    public class StudentBuilder : IBuilder
    {
        protected string[] Names;
        protected string Surname;
        protected int Semester;
        protected string Code;
        public override Dictionary<string, Action<string>> fieldSetterPairs
        { get; }
        public StudentBuilder()
        {
            Names = new string[] { "" };
            Surname = "";
            Code = "";
            Semester = 0;
            fieldSetterPairs = new Dictionary<string, Action<string>>
            {
                { "names", new Action<string>(SetNames) },
                { "surname", new Action<string>(SetSurname) },
                { "code", new Action<string>(SetCode) },
                { "semester", new Action<string>(SetSemester) }
            };
        }

        override public object Build()
        {
            return new Student(Names, Surname, Semester, Code);
        }

        public void SetNames(string value)
        {
            if (value == null)
                throw new ArgumentException();

            Names = value.Split(",")
                    .Select(str => str.Trim())
                    .ToArray();
        }

        public void SetSurname(string value)
        {
            if (value == null)
                throw new ArgumentException();

            Surname = value;
        }

        public void SetCode(string value)
        {
            if (value == null)
                throw new ArgumentException();

            Code = value;
        }

        public void SetSemester(string value)
        {
            if (value == null)
                throw new ArgumentException();

            if (!Semester.SafeTryParse(value))
                throw new ArgumentException();
        }
    }

    public class StudentPartialTxtBuilder : StudentBuilder
    {
        override public object Build()
        {
            return new StudentPartialTxt(Surname + "," + string.Join(",", Names),
                Semester, Code, "");
        }
    }

    public static class EnumExtension
    {
        public static bool SafeTryParse<TEnum>(this ref TEnum Field, 
            string value) where TEnum : struct
        {
            TEnum oldValue = Field;
            if (!Enum.TryParse(value, out Field))
            {
                Field = oldValue;
                return false;
            }
            return true;
        }
    }

    public static class IntExtension
    {
        public static bool SafeTryParse(this ref int Field,
            string value)
        {
            int oldValue = Field;
            if (!int.TryParse(value, out Field))
            {
                Field = oldValue;
                return false;
            }
            return true;
        }
    }

}
