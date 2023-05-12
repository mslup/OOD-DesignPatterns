using System.Data;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ProjOb
{
    public interface IBuilder 
    {
        public Dictionary<string, Action<string>> fieldSetterPairs { get; }
        public object Build(); // refactor 'object'
        public object Update(object t); // as above
    }

    public class RoomBuilder : IBuilder
    {
        protected int Number;
        protected IRoom.RoomTypeEnum RoomType;
        public Dictionary<string, Action<string>> fieldSetterPairs
        { get; }
        public Dictionary<string, bool> updatedFields;

        public RoomBuilder()
        {
            Number = 0;
            RoomType = IRoom.RoomTypeEnum.other;
            fieldSetterPairs = new Dictionary<string, Action<string>>
            {
                { "number", SetNumber },
                { "type", SetRoomType },
            };
            updatedFields = new Dictionary<string, bool>
            {
                { "number", false },
                { "type", false}
            };
        }

        public void SetNumber(string value)
        {
            if (value == null)
                throw new ArgumentException();

            if (!Number.SafeTryParse(value))
                throw new ArgumentException();

            updatedFields["number"] = true;
        }

        public void SetRoomType(string value)
        {
            if (value == null)
                throw new ArgumentException();

            // unwanted behaviour when value > 4
            if (!RoomType.SafeTryParse(value))
                throw new ArgumentException();

            updatedFields["type"] = true;
        }

        public object Build()
        {
            return new Room(Number, RoomType);
        }

        public object Update(object room)
        { 
            if (updatedFields["number"])
                (room as IRoom).Number = Number;
            if (updatedFields["type"])
                (room as IRoom).RoomType = RoomType;

            return room;
        }
    }

    public class RoomPartialTxtBuilder : RoomBuilder
    {
        public RoomPartialTxt Build() // to do
        {
            return new RoomPartialTxt(Number, RoomType.ToString(), "");
        }
    }

    public class CourseBuilder : IBuilder
    {
        protected string Name;
        protected string Code;
        protected int Duration;
        public Dictionary<string, Action<string>> fieldSetterPairs { get; }
        public Dictionary<string, bool> updatedFields;

        public CourseBuilder()
        {
            Name = "";
            Code = "";
            Duration = 0;
            fieldSetterPairs = new Dictionary<string, Action<string>>
            {
                { "name", SetName },
                { "code", SetCode },
                { "duration", SetDuration }
            };
            updatedFields = new Dictionary<string, bool>
            {
                { "name", false },
                { "code", false},
                { "duration", false }
            };
        }

        public object Build()
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

        public void SetDuration(string value)
        {
            if (value == null)
                throw new ArgumentException();

            if (!Duration.SafeTryParse(value))
                throw new ArgumentException();
        }

        public object Update(object course)
        {
            if (updatedFields["name"])
                (course as ICourse).Name = Name;
            if (updatedFields["code"])
                (course as ICourse).Code = Code;
            if (updatedFields["duration"])
                (course as ICourse).Duration = Duration;

            return course;
        }
    }

    public class CoursePartialTxtBuilder : CourseBuilder
    {
        public CoursePartialTxt Build()
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
        public Dictionary<string, Action<string>> fieldSetterPairs { get; }
        public Dictionary<string, bool> updatedFields;

        public TeacherBuilder()
        {
            Names = new string[] { "" };
            Surname = "";
            Code = "";
            TeacherRank = ITeacher.TeacherRankEnum.KiB;
            fieldSetterPairs = new Dictionary<string, Action<string>>
            {
                { "names", SetNames },
                { "surname", SetSurname },
                { "code", SetCode },
                { "rank", SetTeacherRank }
            };
            updatedFields = new Dictionary<string, bool>
            {
                { "names", false },
                { "surname", false },
                { "code", false },
                { "rank", false }
            };
        }

        public object Build()
        {
            return new Teacher(Names, Surname, TeacherRank, Code);
        }

        public object Update(object teacher)
        {
            if (updatedFields["names"])
                (teacher as ITeacher).Names = Names.ToList();
            if (updatedFields["surname"])
                (teacher as ITeacher).Surname = Surname;
            if (updatedFields["code"])
                (teacher as ITeacher).Code = Code;
            if (updatedFields["rank"])
                (teacher as ITeacher).TeacherRank = TeacherRank;

            return teacher;
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
        public TeacherPartialTxt Build()
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
        public Dictionary<string, Action<string>> fieldSetterPairs
        { get; }
        public Dictionary<string, bool> updatedFields;
        public StudentBuilder()
        {
            Names = new string[] { "" };
            Surname = "";
            Code = "";
            Semester = 0;
            fieldSetterPairs = new Dictionary<string, Action<string>>
            {
                { "names", SetNames },
                { "surname", SetSurname },
                { "code", SetCode },
                { "semester", SetSemester }
            };
            updatedFields = new Dictionary<string, bool>
            {
                { "names", false },
                { "surname", false },
                { "code", false },
                { "semester", false }
            };
        }

        public object Build()
        {
            return new Student(Names, Surname, Semester, Code);
        }

        public object Update(object student)
        {
            if (updatedFields["names"])
                (student as IStudent).Names = Names.ToList();
            if (updatedFields["surname"])
                (student as IStudent).Surname = Surname;
            if (updatedFields["code"])
                (student as IStudent).Code = Code;
            if (updatedFields["semester"])
                (student as IStudent).Semester = Semester;

            return student;
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
        public StudentPartialTxt Build()
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
