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
        public Dictionary<string, Action<string>> Setters { get; }
        public Dictionary<string, Func<object>> BuildMethods { get; }
        public object Update(object t);
    }

    public class RoomBuilder : IBuilder
    {
        protected int Number;
        protected IRoom.RoomTypeEnum RoomType;
        public Dictionary<string, Action<string>> Setters { get; }
        public Dictionary<string, Func<object>> BuildMethods { get; }
        private Dictionary<string, bool> updatedFields;

        public RoomBuilder()
        {
            Number = 0;
            RoomType = IRoom.RoomTypeEnum.other;
            Setters = new Dictionary<string, Action<string>>
            {
                ["number"] = SetNumber,
                ["type"] = SetRoomType,
            };
            BuildMethods = new Dictionary<string, Func<object>>
            {
                ["base"] = Build,
                ["secondary"] = BuildPartialTxt
            };
            updatedFields = new Dictionary<string, bool>
            {
                ["number"] = false,
                ["type"] = false
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

        public IRoom Build()
        {
            return new Room(Number, RoomType);
        }
        public IRoom BuildPartialTxt()
        {
            return new RoomPartialTxtAdapter(
                new RoomPartialTxt(Number, RoomType.ToString(), ""));
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


    public class CourseBuilder : IBuilder
    {
        protected string Name;
        protected string Code;
        protected int Duration;
        public Dictionary<string, Action<string>> Setters { get; }
        public Dictionary<string, Func<object>> BuildMethods { get; }

        private Dictionary<string, bool> updatedFields;

        public CourseBuilder()
        {
            Name = "";
            Code = "";
            Duration = 0;
            Setters = new Dictionary<string, Action<string>>
            {
                { "name", SetName },
                { "code", SetCode },
                { "duration", SetDuration }
            };
            BuildMethods = new Dictionary<string, Func<object>>
            {
                ["base"] = Build,
                ["secondary"] = BuildPartialTxt
            };
            updatedFields = new Dictionary<string, bool>
            {
                { "name", false },
                { "code", false},
                { "duration", false }
            };
        }

        public ICourse Build()
        {
            return new Course(Name, Code, Duration);
        }
        public ICourse BuildPartialTxt()
        {
            return new CoursePartialTxtAdapter(
                new CoursePartialTxt(Name, Code, Duration, ""));
        }

        public void SetName(string value)
        {
            if (value == null)
                throw new ArgumentException();

            Name = value;

            updatedFields["name"] = true;
        }

        public void SetCode(string value)
        {
            if (value == null)
                throw new ArgumentException();

            Code = value;

            updatedFields["code"] = true;
        }

        public void SetDuration(string value)
        {
            if (value == null)
                throw new ArgumentException();

            if (!Duration.SafeTryParse(value))
                throw new ArgumentException();

            updatedFields["duration"] = true;
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

    public class TeacherBuilder : IBuilder
    {
        protected string[] Names;
        protected string Surname;
        protected string Code;
        protected ITeacher.TeacherRankEnum TeacherRank;
        public Dictionary<string, Action<string>> Setters { get; }
        public Dictionary<string, Func<object>> BuildMethods { get; }

        private Dictionary<string, bool> updatedFields;

        public TeacherBuilder()
        {
            Names = new string[] { "" };
            Surname = "";
            Code = "";
            TeacherRank = ITeacher.TeacherRankEnum.KiB;
            Setters = new Dictionary<string, Action<string>>
            {
                { "names", SetNames },
                { "surname", SetSurname },
                { "code", SetCode },
                { "rank", SetTeacherRank }
            };
            BuildMethods = new Dictionary<string, Func<object>>
            {
                ["base"] = Build,
                ["secondary"] = BuildPartialTxt
            };
            updatedFields = new Dictionary<string, bool>
            {
                { "names", false },
                { "surname", false },
                { "code", false },
                { "rank", false }
            };
        }

        public ITeacher Build()
        {
            return new Teacher(Names, Surname, TeacherRank, Code);
        }
        public ITeacher BuildPartialTxt()
        {
            return new TeacherPartialTxtAdapter(
                new TeacherPartialTxt(Surname + "," + string.Join(",", Names),
                TeacherRank.ToString(), Code, ""));
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

            updatedFields["names"] = true;
        }

        public void SetSurname(string value)
        {
            if (value == null)
                throw new ArgumentException();

            Surname = value;

            updatedFields["surname"] = true;
        }

        public void SetCode(string value)
        {
            if (value == null)
                throw new ArgumentException();

            Code = value;

            updatedFields["code"] = true;
        }

        public void SetTeacherRank(string value)
        {
            if (value == null)
                throw new ArgumentException();

            // unwanted behaviour same as in RoomBuilder
            if (!TeacherRank.SafeTryParse(value))
                throw new ArgumentException();

            updatedFields["rank"] = true;
        }
    }


    public class StudentBuilder : IBuilder
    {
        protected string[] Names;
        protected string Surname;
        protected int Semester;
        protected string Code;
        public Dictionary<string, Action<string>> Setters
        { get; }
        public Dictionary<string, Func<object>> BuildMethods { get; }

        private Dictionary<string, bool> updatedFields;
        public StudentBuilder()
        {
            Names = new string[] { "" };
            Surname = "";
            Code = "";
            Semester = 0;
            Setters = new Dictionary<string, Action<string>>
            {
                { "names", SetNames },
                { "surname", SetSurname },
                { "code", SetCode },
                { "semester", SetSemester }
            };
            BuildMethods = new Dictionary<string, Func<object>>
            {
                ["base"] = Build,
                ["secondary"] = BuildPartialTxt
            };
            updatedFields = new Dictionary<string, bool>
            {
                { "names", false },
                { "surname", false },
                { "code", false },
                { "semester", false }
            };
        }

        public IStudent Build()
        {
            return new Student(Names, Surname, Semester, Code);
        }
        public IStudent BuildPartialTxt()
        {
            return new StudentPartialTxtAdapter(
                new StudentPartialTxt(Surname + "," + string.Join(",", Names),
                Semester, Code, ""));
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

            updatedFields["names"] = true;
        }

        public void SetSurname(string value)
        {
            if (value == null)
                throw new ArgumentException();

            Surname = value;

            updatedFields["surname"] = true;
        }

        public void SetCode(string value)
        {
            if (value == null)
                throw new ArgumentException();

            Code = value;

            updatedFields["code"] = true;
        }

        public void SetSemester(string value)
        {
            if (value == null)
                throw new ArgumentException();

            if (!Semester.SafeTryParse(value))
                throw new ArgumentException();

            updatedFields["semester"] = true;
        }
    }

    public class StudentPartialTxtBuilder : StudentBuilder
    {

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
