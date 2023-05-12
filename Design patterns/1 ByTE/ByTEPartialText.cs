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
        public int Number { get; set; }
        public string RoomType { get; set; }
        public string Courses { get; set; }
        public string Representation { get => "secondary"; }

        public RoomPartialTxt(int number, string type, string courses)
        {
            Number = number;
            RoomType = type.ToString();
            Courses = courses;
            Dictionaries.AddRoom(new RoomPartialTxtAdapter(this));
        }
    }

    public class CoursePartialTxt
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public int Duration { get; set; }
        public string People { get; set; }
        public string Representation { get => "secondary"; }

        public CoursePartialTxt(string name, string code, int duration,
            string people)
        {
            Name = name;
            Code = code;
            Duration = duration;
            People = people;
            Dictionaries.AddCourse(new CoursePartialTxtAdapter(this));
        }
    }

    public class TeacherPartialTxt
    {
        public string Identity { get; set; }
        public string Rank { get; set; }
        public string Code { get; set; }
        public string Courses { get; set; }
        public string Representation { get => "secondary"; }

        public TeacherPartialTxt(string identity, string rank, string code, string courses)
        {
            Identity = identity;
            Rank = rank;
            Code = code;
            Courses = courses;
            Dictionaries.AddTeacher(new TeacherPartialTxtAdapter(this));
        }
    }

    public class StudentPartialTxt
    {
        public string Identity { get; set; }
        public int Semester { get; set; }
        public string Code { get; set; }
        public string Courses { get; set; }
        public string Representation { get => "secondary"; }

        public StudentPartialTxt(string identity, int semester, string code, string courses)
        {
            Identity = identity;
            Semester = semester;
            Code = code;
            Courses = courses;
            Dictionaries.AddStudent(new StudentPartialTxtAdapter(this));
        }
    }
}