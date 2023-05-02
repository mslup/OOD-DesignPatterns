using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static ProjOb.Teacher;

namespace ProjOb
{ 
    public partial class ByTE : IByTE
    {
        private List<IRoom> rooms;
        private List<ICourse> courses;
        private List<ITeacher> teachers;
        private List<IStudent> students;

        public List<IRoom> Rooms { get => rooms; }
        public List<ICourse> Courses { get => courses; }
        public List<ITeacher> Teachers { get => teachers; }
        public List<IStudent> Students { get => students; }

        public ByTE()
        {
            rooms = new List<IRoom>();
            courses = new List<ICourse>();
            teachers = new List<ITeacher>();
            students = new List<IStudent>();
        }

        public void AddRooms(params Room[] rooms)
        {
            foreach (var room in rooms)
                this.rooms.Add(room);
        }

        public void AddCourses(params Course[] courses)
        {
            foreach (var course in courses)
                this.courses.Add(course);
        }

        public void AddTeachers(params Teacher[] teachers)
        {
            foreach (var teacher in teachers)
                this.teachers.Add(teacher);
        }

        public void AddStudents(params Student[] students)
        {
            foreach (var student in students)
                this.students.Add(student);
        }

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

    public partial class Room : IRoom
    {
        private int _number;
        private IRoom.RoomTypeEnum _type;
        private List<ICourse> _classes;

        public int Number { get => _number; }
        public IRoom.RoomTypeEnum RoomType { get => _type; }
        public List<ICourse> Courses { get => _classes; }


        public Room(int number, IRoom.RoomTypeEnum type)
        {
            this._number = number;
            this._type = type;
            this._classes = new List<ICourse>();
            InitDictionary();
            Dictionaries.AddRoom(this);
        }

        public void AddCourses(params Course[] courses)
        {
            foreach (var c in courses)
                AddCourse(c);
        }
        public void AddCourse(Course c) { _classes.Add(c); }

        public override string ToString()
        {
            return $"{Number}, {RoomType}, " +
                $"[{string.Join(", ", Courses.Select(course => course.Code))}]"; 
        }        
    }
    public partial class Course : ICourse
    {
        private string _name;
        private string _code;
        private int _duration;
        private List<ITeacher> _teachers;
        private List<IStudent> _students;

        public string Name { get => _name; }
        public string Code { get => _code; }
        public int Duration { get => _duration; }
        public List<ITeacher> Teachers { get => _teachers; }
        public List<IStudent> Students { get => _students; }

        public Course(string name, string code, int duration)
        {
            this._name = name;
            this._code = code;
            this._duration = duration;
            _teachers = new List<ITeacher>();
            _students = new List<IStudent>();
            InitDictionary();
            Dictionaries.AddCourse(this);
        }

        public void LinkTeachers(params Teacher[] teachers)
        {
            foreach (var teacher in teachers)
            {
                _teachers.Add(teacher);
                teacher.AddCourse(this);
            }
        }
        public void LinkStudents(params Student[] students)
        {
            foreach (var student in students)
            {
                _students.Add(student);
                student.AddCourse(this);
            }
        }

        public override string ToString()
        {
            return $"{Name}, {Code}, {Duration}h, " +
                $"[{string.Join(", ", Teachers.Select(teacher => teacher.Code))}], " +
                $"[{string.Join(", ", Students.Select(student => student.Code))}]";
        }

    }
    public partial class Teacher : ITeacher
    {
        private List<string> _names;
        private string _surname;
        private ITeacher.TeacherRankEnum _rank;
        private string _code;
        private List<ICourse> _classes;

        public List<string> Names { get => _names; }
        public string Surname { get => _surname; }
        public string Code { get => _code; }
        public ITeacher.TeacherRankEnum TeacherRank { get => _rank; }
        public List<ICourse> Courses { get => _classes; }

        public Teacher(string name, string surname, ITeacher.TeacherRankEnum rank, string code)
        {
            _names = new List<string>();
            _names.Add(name);
            this._surname = surname;
            this._code = code;
            this._rank = rank;
            _classes = new List<ICourse>();
            InitDictionary();
            Dictionaries.AddTeacher(this);
        }

        public Teacher(string[] names, string surname, ITeacher.TeacherRankEnum rank, string code)
        {
            this._names = names.ToList();
            this._surname = surname;
            this._code = code;
            this._rank = rank;
            _classes = new List<ICourse>(); 
            InitDictionary();
            Dictionaries.AddTeacher(this);
        }

        public void AddCourse(Course c)
        {
            _classes.Add(c);
        }

        public override string ToString()
        {
            return $"[{string.Join(", ", Names)}]" +
                $", {Surname}, {TeacherRank}, {Code}, " +
                $"[{string.Join(", ", Courses.Select(course => course.Code))}]";
        }

    }
    public partial class Student : IStudent
    {
        private List<string> _names;
        private string _surname;
        private int _semester;
        private string _code;
        private List<ICourse> _classes;

        public List<string> Names { get => _names; }
        public string Surname { get => _surname; }
        public string Code { get => _code; }
        public int Semester { get => _semester; }
        public List<ICourse> Courses { get => _classes; }

        public Student(string name, string surname, int semester, string code)
        {
            _names = new List<string>();
            _names.Add(name);
            this._surname = surname;
            this._semester = semester;
            this._code = code;
            _classes = new List<ICourse>();
            InitDictionary();
            Dictionaries.AddStudent(this);
        }

        public Student(string[] names, string surname, int semester, string code)
        {
            this._names = names.ToList();
            this._surname = surname;
            this._semester = semester;
            this._code = code;
            _classes = new List<ICourse>();
            InitDictionary();
            Dictionaries.AddStudent(this);
        }

        public void AddCourse(Course c)
        {
            _classes.Add(c);
        }

        public override string ToString()
        {
            return $"[{string.Join(", ", Names)}]" +
                $", {Surname}, {Semester}, {Code}, " +
                $"[{string.Join(", ", Courses.Select(course => course.Code))}]";
        }
    }

   
}
