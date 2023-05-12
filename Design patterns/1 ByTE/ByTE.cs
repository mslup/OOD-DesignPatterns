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
        public int Number { get; set; }
        public IRoom.RoomTypeEnum RoomType { get; set; }
        public List<ICourse> Courses { get; set; }

        public string Representation { get => "base"; }

        public Room(int number, IRoom.RoomTypeEnum type)
        {
            Number = number;
            RoomType = type;
            Courses = new List<ICourse>();
            InitDictionary();
            Dictionaries.AddRoom(this);
        }

        public void AddCourses(params Course[] courses)
        {
            foreach (var c in courses)
                AddCourse(c);
        }
        public void AddCourse(Course c) { Courses.Add(c); }

        public override string ToString()
        {
            return $"{Number}, {RoomType}, " +
                $"[{string.Join(", ", Courses.Select(course => course.Code))}]";
        }
    }
    public partial class Course : ICourse
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public int Duration { get; set; }
        public List<ITeacher> Teachers { get; set; }
        public List<IStudent> Students { get; set; }
        public string Representation { get => "base"; }

        public Course(string name, string code, int duration)
        {
            Name = name;
            Code = code;
            Duration = duration;
            Teachers = new List<ITeacher>();
            Students = new List<IStudent>();
            InitDictionary();
            Dictionaries.AddCourse(this);
        }

        public void LinkTeachers(params Teacher[] teachers)
        {
            foreach (var teacher in teachers)
            {
                Teachers.Add(teacher);
                teacher.AddCourse(this);
            }
        }
        public void LinkStudents(params Student[] students)
        {
            foreach (var student in students)
            {
                Students.Add(student);
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
        public List<string> Names { get; set; }
        public string Surname { get; set; }
        public string Code { get; set; }
        public ITeacher.TeacherRankEnum TeacherRank { get; set; }
        public List<ICourse> Courses { get; set; }
        public string Representation { get => "base"; }

        public Teacher(string name, string surname, ITeacher.TeacherRankEnum rank, string code)
        {
            Names = new List<string> { name };
            Surname = surname;
            Code = code;
            TeacherRank = rank;
            Courses = new List<ICourse>();
            InitDictionary();
            Dictionaries.AddTeacher(this);
        }

        public Teacher(string[] names, string surname, ITeacher.TeacherRankEnum rank, string code)
        {
            Names = names.ToList();
            Surname = surname;
            Code = code;
            TeacherRank = rank;
            Courses = new List<ICourse>();
            InitDictionary();
            Dictionaries.AddTeacher(this);
        }

        public void AddCourse(Course c)
        {
            Courses.Add(c);
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
        public List<string> Names { get; set; }
        public string Surname { get; set; }
        public string Code { get; set; }
        public int Semester { get; set; }
        public List<ICourse> Courses { get; set; }
        public string Representation { get => "base"; }

        public Student(string name, string surname, int semester, string code)
        {
            Names = new List<string>() { name };
            Surname = surname;
            Semester = semester;
            Code = code;
            Courses = new List<ICourse>();
            InitDictionary();
            Dictionaries.AddStudent(this);
        }

        public Student(string[] names, string surname, int semester, string code)
        {
            Names = names.ToList();
            Surname = surname;
            Semester = semester;
            Code = code;
            Courses = new List<ICourse>();
            InitDictionary();
            Dictionaries.AddStudent(this);
        }

        public void AddCourse(Course c)
        {
            Courses.Add(c);
        }

        public override string ToString()
        {
            return $"[{string.Join(", ", Names)}]" +
                $", {Surname}, {Semester}, {Code}, " +
                $"[{string.Join(", ", Courses.Select(course => course.Code))}]";
        }
    }


}
