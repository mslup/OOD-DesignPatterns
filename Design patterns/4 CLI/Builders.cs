using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ProjOb
{
    // refactor marker interface
    public abstract class IBuilder
    {
        public abstract Dictionary<string, Action<string>>
            fieldSetterPairs { get; }          
        public abstract object Build(); // refactor 'object'
    }

    public class RoomBuilder : IBuilder
    {
        private int Number;
        private IRoom.RoomTypeEnum RoomType;
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

            if (!int.TryParse(value, out Number))
                throw new ArgumentException();
        }

        public void SetRoomType(string value)
        {
            if (value == null)
                throw new ArgumentException();

            // unwanted behaviour when value > 4
            if (!Enum.TryParse(value, out RoomType))
                throw new ArgumentException();
        }
    }

    public class CourseBuilder : IBuilder
    {
        private string Name;
        private string Code;
        private int Duration;
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

            if (!int.TryParse(value, out Duration))
                throw new ArgumentException();
        }
    }

    public class TeacherBuilder : IBuilder
    {
        private string[] Names;
        private string Surname;
        private string Code;
        private ITeacher.TeacherRankEnum TeacherRank;
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
            if (!Enum.TryParse(value, out TeacherRank))
                throw new ArgumentException();
        }
    }

    public class StudentBuilder : IBuilder
    {
        private string[] Names;
        private string Surname;
        private int Semester;
        private string Code;
        public override Dictionary<string, Action<string>> fieldSetterPairs
        { get; }
        public StudentBuilder()
        {
            Names = new string[] { "" };
            Surname = "";
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

            if (!int.TryParse(value, out Semester))
                throw new ArgumentException();
        }
    }

    //public class RoomPartialTxtBuilder
    //{
    //    private int Number;
    //    private IRoom.RoomTypeEnum RoomType;
    //    public readonly Dictionary<string, Action<string>> fieldSetterPairs;

    //    public RoomPartialTxtBuilder()
    //    {
    //        Number = 0;
    //        RoomType = IRoom.RoomTypeEnum.other;
    //        fieldSetterPairs = new Dictionary<string, Action<string>>
    //        {
    //            { "number", new Action<string>(SetNumber) },
    //            { "type", new Action<string>(SetRoomType) },
    //        };
    //    }

    //    public RoomPartialTxtAdapter Build()
    //    {
    //        return new RoomPartialTxtAdapter(
    //            new RoomPartialTxt());
    //    }
    //}
}
