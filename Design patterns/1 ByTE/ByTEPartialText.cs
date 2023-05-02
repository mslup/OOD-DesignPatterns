namespace ProjOb
{
    public class ByTEPartialTxt
    {
        private List<RoomPartialTxt> rooms;
        private List<CoursePartialTxt> courses;
        private List<TeacherPartialTxt> teachers;
        private List<StudentPartialTxt> students;

        public List<RoomPartialTxt> Rooms { get => rooms; }
        public List<CoursePartialTxt> Courses { get => courses; }
        public List<TeacherPartialTxt> Teachers { get => teachers; }
        public List<StudentPartialTxt> Students { get => students; }

        public ByTEPartialTxt()
        {
            rooms = new List<RoomPartialTxt>();
            courses = new List<CoursePartialTxt>();
            teachers = new List<TeacherPartialTxt>();
            students = new List<StudentPartialTxt>();
        }

        public void AddRoom(RoomPartialTxt room)
        {
            this.rooms.Add(room);
        }

        public void AddCourse(CoursePartialTxt course)
        {
            this.courses.Add(course);
        }

        public void AddTeacher(TeacherPartialTxt teacher)
        {
            this.teachers.Add(teacher);
        }

        public void AddStudent(StudentPartialTxt student)
        {
            this.students.Add(student);
        }
    }

    public class RoomPartialTxt
    {
        private int _number;
        private string _type;
        private string _courses;

        public int Number { get => _number; }
        public string RoomType { get => _type; }
        public string Courses { get => _courses; }

        public RoomPartialTxt(int number, string type, string courses)
        {
            this._number = number;
            this._type = type.ToString();
            this._courses = courses;
            Dictionaries.AddRoom(new RoomPartialTxtAdapter(this));
        }
    }

    public class CoursePartialTxt
    {
        private string _name;
        private string _code;
        private int _duration;
        private string _people;

        public string Name { get => _name; }
        public string Code { get => _code; }
        public int Duration { get => _duration; }
        public string People { get => _people; }

        public CoursePartialTxt(string name, string code, int duration,
            string people)
        {
            this._name = name;
            this._code = code;
            this._duration = duration;
            this._people = people;
            Dictionaries.AddCourse(new CoursePartialTxtAdapter(this));
        }
    }

    public class TeacherPartialTxt
    {
        private string _identity;
        private string _rank;
        private string _code;
        private string _courses;

        public string Identity { get => _identity; }
        public string Rank { get => _rank; }
        public string Code { get => _code; }
        public string Courses { get => _courses; }

        public TeacherPartialTxt(string identity, string rank, string code, string courses)
        {
            this._identity = identity;
            this._rank = rank;
            this._code = code;
            this._courses = courses;
            Dictionaries.AddTeacher(new TeacherPartialTxtAdapter(this));
        }
    }

    public class StudentPartialTxt
    {
        private string _identity;
        private int _semester;
        private string _code;
        private string _courses;

        public string Identity { get => _identity; }
        public int Semester { get => _semester; }
        public string Code { get => _code; }
        public string Courses { get => _courses; }

        public StudentPartialTxt(string identity, int semester, string code, string courses)
        {
            this._identity = identity;
            this._semester = semester;
            this._code = code;
            this._courses = courses;
            Dictionaries.AddStudent(new StudentPartialTxtAdapter(this));
        }
    }
}