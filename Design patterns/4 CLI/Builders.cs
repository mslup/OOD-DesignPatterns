using System.Data;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ProjOb
{
    [DataContract, KnownType(typeof(IBuilder)),
        KnownType(typeof(RoomBuilder)), KnownType(typeof(CourseBuilder)), KnownType(typeof(TeacherBuilder)), KnownType(typeof(StudentBuilder))]
    public abstract class IBuilder
    {
        [DataMember] public abstract List<string> updatedFields { get; protected set; }
        public abstract Dictionary<string, Action<string>> Setters { get; }
        public abstract Dictionary<string, Func<object>> BuildMethods { get; }
        //protected abstract Dictionary<string, bool> updatedFields { get; }
        public abstract object Update(object t);

        //protected abstract Dictionary<string, Func<object>> Getters { get; set;  }
        protected abstract Dictionary<string, object> Getters { get; set;  }

        //[OnDeserializing]
        //protected abstract void OnDeserializing();
        protected abstract void GetFields();

        public override string ToString()
        {
            GetFields(); 
            var sb = new StringBuilder();

            foreach (var field in updatedFields)
            {
                sb.Append(field);
                sb.Append('=');
                sb.Append(Getters[field]);
                //Console.WriteLine(Getters[field]());

                sb.Append("\n");
            }

            sb.Append("done");

            return sb.ToString();
        }
    }

    [DataContract, KnownType(typeof(RoomBuilder)), KnownType(typeof(IBuilder))]
    public class RoomBuilder : IBuilder
    {
        [DataMember] private int Number;
        [DataMember] private IRoom.RoomTypeEnum RoomType;
        [DataMember] public override List<string> updatedFields { get; protected set; }
        public override Dictionary<string, Action<string>> Setters { get; }
        [DataMember]
        protected override Dictionary<string, object> Getters { get; set; }
        protected override void GetFields()
        {
            Getters = new Dictionary<string, object>
            {
                ["number"] = Number,
                ["type"] = RoomType
            };
        }
        public override Dictionary<string, Func<object>> BuildMethods { get; }

        public RoomBuilder()
        {
            Number = 0;
            RoomType = IRoom.RoomTypeEnum.other;
            Setters = new Dictionary<string, Action<string>>
            {
                ["number"] = SetNumber,
                ["type"] = SetRoomType
            };

            updatedFields = new();
            BuildMethods = new()
            {
                ["base"] = Build,
                ["secondary"] = BuildPartialTxt
            };
        }

        public void SetNumber(string value)
        {
            if (value == null)
                throw new ArgumentException();

            if (!Number.SafeTryParse(value))
                throw new ArgumentException();

            updatedFields.Add("number");
        }

        public void SetRoomType(string value)
        {
            if (value == null)
                throw new ArgumentException();

            // unwanted behaviour when value > 4
            if (!RoomType.SafeTryParse(value))
                throw new ArgumentException();

            updatedFields.Add("type");
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

        public override object Update(object room)
        {
            if (updatedFields.Contains("number"))
                (room as IRoom).Number = Number;
            if (updatedFields.Contains("type"))
                (room as IRoom).RoomType = RoomType;

            return room;
        }

    }

    [DataContract, KnownType(typeof(CourseBuilder)), KnownType(typeof(IBuilder))]
    public class CourseBuilder : IBuilder
    {
        [DataMember] private string Name;
        [DataMember] private string Code;
        [DataMember] private int Duration;
        [DataMember] public override List<string> updatedFields { get; protected set; }
        protected override Dictionary<string, object> Getters { get; set; }
        public override Dictionary<string, Action<string>> Setters { get; }
        public override Dictionary<string, Func<object>> BuildMethods { get; }
       
        protected override void GetFields()
        {
            Getters = new Dictionary<string, object>
            {
                { "name", Name },
                { "code", Code },
                { "duration", Duration }
            };
        }

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
            updatedFields = new();
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

            updatedFields.Add("name");
        }

        public void SetCode(string value)
        {
            if (value == null)
                throw new ArgumentException();

            Code = value;

            updatedFields.Add("code");
        }

        public void SetDuration(string value)
        {
            if (value == null)
                throw new ArgumentException();

            if (!Duration.SafeTryParse(value))
                throw new ArgumentException();

            updatedFields.Add("duration");
        }

        public override object Update(object course)
        {
            if (updatedFields.Contains("name"))
                (course as ICourse).Name = Name;
            if (updatedFields.Contains("code"))
                (course as ICourse).Code = Code;
            if (updatedFields.Contains("duration"))
                (course as ICourse).Duration = Duration;

            return course;
        }
    }

    [DataContract, KnownType(typeof(TeacherBuilder)), KnownType(typeof(IBuilder))]
    public class TeacherBuilder : IBuilder
    {
        [DataMember] private string[] Names;
        [DataMember] private string Surname;
        [DataMember] private string Code;
        [DataMember] private ITeacher.TeacherRankEnum TeacherRank;
        [DataMember] public override List<string> updatedFields { get; protected set; }
        protected override Dictionary<string, object> Getters { get; set; }
        public override Dictionary<string, Action<string>> Setters { get; }
        public override Dictionary<string, Func<object>> BuildMethods { get; }

        protected override void GetFields()
        {
            Getters = new Dictionary<string, object>
            {
                { "names", Names },
                { "surname", Surname },
                { "code", Code },
                { "rank", TeacherRank }
            };
        }

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
            updatedFields = new();
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

        public override object Update(object teacher)
        {
            if (updatedFields.Contains("names"))
                (teacher as ITeacher).Names = Names.ToList();
            if (updatedFields.Contains("surname"))
                (teacher as ITeacher).Surname = Surname;
            if (updatedFields.Contains("code"))
                (teacher as ITeacher).Code = Code;
            if (updatedFields.Contains("rank"))
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

            updatedFields.Add("names");
        }

        public void SetSurname(string value)
        {
            if (value == null)
                throw new ArgumentException();

            Surname = value;

            updatedFields.Add("surname");
        }

        public void SetCode(string value)
        {
            if (value == null)
                throw new ArgumentException();

            Code = value;

            updatedFields.Add("code");
        }

        public void SetTeacherRank(string value)
        {
            if (value == null)
                throw new ArgumentException();

            // unwanted behaviour same as in RoomBuilder
            if (!TeacherRank.SafeTryParse(value))
                throw new ArgumentException();

            updatedFields.Add("rank");
        }
    }

    [DataContract, KnownType(typeof(StudentBuilder)), KnownType(typeof(IBuilder))]
    public class StudentBuilder : IBuilder
    {
        [DataMember] private string[] Names;
        [DataMember] private string Surname;
        [DataMember] private int Semester;
        [DataMember] private string Code;
        [DataMember] public override List<string> updatedFields { get; protected set; }
        protected override Dictionary<string, object> Getters { get; set; }
        public override Dictionary<string, Action<string>> Setters { get; }
        public override Dictionary<string, Func<object>> BuildMethods { get; }

        protected override void GetFields()
        {
            Getters = new Dictionary<string, object>
            {
                { "names", Names },
                { "surname", Surname },
                { "code", Code },
                { "semester", Semester }
            };
        }

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
            updatedFields = new();
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

        public override object Update(object student)
        {
            if (updatedFields.Contains("names"))
                (student as IStudent).Names = Names.ToList();
            if (updatedFields.Contains("surname"))
                (student as IStudent).Surname = Surname;
            if (updatedFields.Contains("code"))
                (student as IStudent).Code = Code;
            if (updatedFields.Contains("semester"))
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

            updatedFields.Add("names");
        }

        public void SetSurname(string value)
        {
            if (value == null)
                throw new ArgumentException();

            Surname = value;

            updatedFields.Add("surname");
        }

        public void SetCode(string value)
        {
            if (value == null)
                throw new ArgumentException();

            Code = value;

            updatedFields.Add("code");
        }

        public void SetSemester(string value)
        {
            if (value == null)
                throw new ArgumentException();

            if (!Semester.SafeTryParse(value))
                throw new ArgumentException();

            updatedFields.Add("semester");
        }
    }
}
