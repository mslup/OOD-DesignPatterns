using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using static ProjOb.IRoom;
using static ProjOb.ITeacher;

namespace ProjOb
{
    public class ByTEPartialTxtAdapter : IByTE
    {
        private ByTEPartialTxt adaptee;

        public List<IRoom> Rooms
            => adaptee.Rooms.Select(
                room => (IRoom)(new RoomPartialTxtAdapter(room))
                ).ToList();
        public List<ICourse> Courses
            => adaptee.Courses.Select(
                course => (ICourse)(new CoursePartialTxtAdapter(course))
                ).ToList();
        public List<ITeacher> Teachers
            => adaptee.Teachers.Select(
                teacher => (ITeacher)(new TeacherPartialTxtAdapter(teacher))
                ).ToList();
        public List<IStudent> Students
            => adaptee.Students.Select(
                student => (IStudent)(new StudentPartialTxtAdapter(student))
                ).ToList();

        public ByTEPartialTxtAdapter(ByTEPartialTxt adaptee)
            => this.adaptee = adaptee;

        public override string ToString()
        {
            string str = "";
            str += "Rooms\n";
            int i = 1;
            foreach (var room in Rooms)
            {
                str += $"{i++}. ";
                str += room;
                str += '\n';
            }
            i = 1;
            str += "\nCourses\n";
            foreach (var course in Courses)
            {
                str += $"{i++}. ";
                str += course;
                str += '\n';
            }
            i = 1;
            str += "\nTeachers\n";
            foreach (var teacher in Teachers)
            {
                str += $"{i++}. ";
                str += teacher;
                str += '\n';
            }
            i = 1;
            str += "\nStudents\n";
            foreach (var student in Students)
            {
                str += $"{i++}. ";
                str += student;
                str += '\n';
            }

            return str;
        }
    }

    public partial class RoomPartialTxtAdapter : IRoom
    {
        private RoomPartialTxt adaptee;

        public RoomPartialTxtAdapter(RoomPartialTxt adaptee)
        {
            this.adaptee = adaptee;
            InitDictionary();
        }

        public int Number
        {
            get => adaptee.Number;
            set => adaptee.Number = value;
        }
        public IRoom.RoomTypeEnum RoomType
        {
            get => (RoomTypeEnum)Enum.Parse(typeof(RoomTypeEnum), adaptee.RoomType, true);
            set => adaptee.RoomType = value.ToString();
        }

        public string Representation { get => adaptee.Representation; }

        public List<ICourse> Courses
        {
            get
            {
                var regex = new Regex(@"(?:\(([\w\d]+)\),?)+");
                GroupCollection groups = regex.Match(adaptee.Courses).Groups;

                var ret = new List<ICourse>();

                foreach (Capture capture in groups[1].Captures)
                    ret.Add(Dictionaries.CourseDict[capture.Value]);

                return ret;
            }
        }

        public override string ToString()
        {
            var str = "";
            str += $"{Number}, {RoomType}, ";

            var regex = new Regex(@"(?:\(([\w\d]+)\),?)+");
            GroupCollection groups = regex.Match(adaptee.Courses).Groups;

            str += "[";
            str += string.Join(", ", groups[1].Captures);
            str += "]";

            return str;
        }
    }

    public partial class CoursePartialTxtAdapter : ICourse
    {
        private CoursePartialTxt adaptee;

        public CoursePartialTxtAdapter(CoursePartialTxt adaptee)
        {
            this.adaptee = adaptee;
            InitDictionary();
        }

        public string Name
        {
            get => adaptee.Name;
            set => adaptee.Name = value;
        }

        public string Code
        {
            get => adaptee.Code;
            set => adaptee.Code = value;
        }

        public int Duration
        {
            get => adaptee.Duration;
            set => adaptee.Duration = value;
        }
        public string Representation
        {
            get => adaptee.Representation;
        }

        public List<ITeacher> Teachers
        {
            get
            {
                var sepRegex = new Regex(@"(?<teachers>.*)\$(?<students>.*)");
                GroupCollection groups = sepRegex.Match(adaptee.People).Groups;

                var interRegex = new Regex(@"(?:([\w\d]+),?)+");
                GroupCollection interGroups =
                    interRegex.Match(groups["teachers"].Value).Groups;

                var ret = new List<ITeacher>();

                foreach (Capture capture in interGroups[1].Captures)
                    ret.Add(Dictionaries.TeacherDict[capture.Value]);

                return ret;
            }
        }
        public List<IStudent> Students
        {
            get
            {
                var sepRegex = new Regex(@"(?<teachers>.*)\$(?<students>.*)");
                GroupCollection groups = sepRegex.Match(adaptee.People).Groups;

                var interRegex = new Regex(@"(?:([\w\d]+),?)+");
                GroupCollection interGroups =
                    interRegex.Match(groups["students"].Value).Groups;

                var ret = new List<IStudent>();

                foreach (Capture capture in interGroups[1].Captures)
                    ret.Add(Dictionaries.StudentDict[capture.Value]);

                return ret;
            }
        }

        public override string ToString()
        {
            var str = "";
            str += $"{Name}, {Code}, {Duration}h, ";

            var sepRegex = new Regex(@"(?<teachers>.*)\$(?<students>.*)");
            GroupCollection groups = sepRegex.Match(adaptee.People).Groups;

            var interRegex = new Regex(@"(?:([\w\d]+),?)+");
            GroupCollection interGroups;

            interGroups = interRegex.Match(groups["teachers"].Value).Groups;
            str += "[" + string.Join(", ", interGroups[1].Captures) + "], ";
            interGroups = interRegex.Match(groups["students"].Value).Groups;
            str += "[" + string.Join(", ", interGroups[1].Captures) + "]";

            return str;
        }
    }

    public partial class TeacherPartialTxtAdapter : ITeacher
    {
        private TeacherPartialTxt adaptee;

        public TeacherPartialTxtAdapter(TeacherPartialTxt adaptee)
        {
            this.adaptee = adaptee;
            InitDictionary();
        }

        public List<string> Names
        {
            get
            {
                var identityRegex = new Regex(@"(?<surname>[\w\s]+),(?:(?<name>[\w\s]+),?)+");
                GroupCollection groups = identityRegex.Match(adaptee.Identity).Groups;

                var ret = new List<string>();

                foreach (Capture capture in groups["name"].Captures)
                    ret.Add(capture.Value);

                return ret;
            }
            set
            {
                string[] identity = adaptee.Identity.Split(",", 2);
                adaptee.Identity = identity[0] + "," + string.Join(",", value);
            }
        }

        public string Surname
        {
            get
            {
                var identityRegex = new Regex(@"(?<surname>[\w\s]+),(?:(?<name>[\w\s]+),?)+");
                GroupCollection groups = identityRegex.Match(adaptee.Identity).Groups;

                return groups["surname"].Value;
            }
            set
            {
                string[] identity = adaptee.Identity.Split(",", 2);
                adaptee.Identity = value + "," + identity[1];
            }
        }

        public string Code
        {
            get => adaptee.Code;
            set => adaptee.Code = value;
        }

        public TeacherRankEnum TeacherRank
        {
            get => (TeacherRankEnum)Enum.Parse(typeof(TeacherRankEnum), adaptee.Rank, true);
            set => adaptee.Rank = value.ToString();
        }

        public string Representation
        {
            get => adaptee.Representation;
        }

        public List<ICourse> Courses
        {
            get
            {
                var coursesRegex = new Regex(@"(?:([\w\d]+),?)+");
                var groups = coursesRegex.Match(adaptee.Courses).Groups;

                var ret = new List<ICourse>();

                foreach (Capture capture in groups[1].Captures)
                    ret.Add(Dictionaries.CourseDict[capture.Value]);

                return ret;
            }
        }

        public override string ToString()
        {
            var str = "";

            var identityRegex = new Regex(@"(?<surname>[\w\s]+),(?:(?<name>[\w\s]+),?)+");
            GroupCollection groups = identityRegex.Match(adaptee.Identity).Groups;

            str += "[" + string.Join(", ", groups["name"].Captures) + "], ";
            str += groups["surname"] + ", ";

            str += $"{adaptee.Rank}, {adaptee.Code}, ";

            var coursesRegex = new Regex(@"(?:([\w\d]+),?)+");
            groups = coursesRegex.Match(adaptee.Courses).Groups;
            str += "[" + string.Join(", ", groups[1].Captures) + "]";

            return str;
        }
    }

    public partial class StudentPartialTxtAdapter : IStudent
    {
        private readonly StudentPartialTxt adaptee;

        public StudentPartialTxtAdapter(StudentPartialTxt adaptee)
        {
            this.adaptee = adaptee;
            InitDictionary();
        }

        public List<string> Names
        {
            get
            {
                var identityRegex = new Regex(@"(?<surname>[\w\s]+),(?:(?<name>[\w\s]+),?)+");
                GroupCollection groups = identityRegex.Match(adaptee.Identity).Groups;

                var ret = new List<string>();

                foreach (Capture capture in groups["name"].Captures)
                    ret.Add(capture.Value);

                return ret;
            }
            set
            {
                string[] identity = adaptee.Identity.Split(",", 2);
                adaptee.Identity = identity[0] + "," + string.Join(",", value);
            }
        }

        public string Surname
        {
            get
            {
                var identityRegex = new Regex(@"(?<surname>[\w\s]+),(?:(?<name>[\w\s]+),?)+");
                GroupCollection groups = identityRegex.Match(adaptee.Identity).Groups;

                return groups["surname"].Value;
            }
            set
            {
                string[] identity = adaptee.Identity.Split(",", 2);
                adaptee.Identity = value + "," + identity[1];
            }
        }

        public string Code
        {
            get => adaptee.Code;
            set => adaptee.Code = value;
        }


        public int Semester
        {
            get => adaptee.Semester;
            set => adaptee.Semester = value;
        }

        public string Representation
        {
            get => adaptee.Representation;
        }

        public List<ICourse> Courses
        {
            get
            {
                var coursesRegex = new Regex(@"(?:([\w\d]+),?)+");
                var groups = coursesRegex.Match(adaptee.Courses).Groups;

                var ret = new List<ICourse>();

                foreach (Capture capture in groups[1].Captures)
                    ret.Add(Dictionaries.CourseDict[capture.Value]);

                return ret;
            }
        }

        public override string ToString()
        {
            var str = "";

            var identityRegex = new Regex(@"(?<surname>[\w\s]+),(?:(?<name>[\w\s]+),?)+");
            GroupCollection groups = identityRegex.Match(adaptee.Identity).Groups;

            str += "[" + string.Join(", ", groups["name"].Captures) + "], ";
            str += groups["surname"] + ", ";

            str += $"{Semester}, {Code}, ";

            var coursesRegex = new Regex(@"(?:([\w\d]+),?)+");
            groups = coursesRegex.Match(adaptee.Courses).Groups;
            str += "[" + string.Join(", ", groups[1].Captures) + "]";

            return str;
        }
    }
}
