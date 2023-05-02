using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ProjObDebug
{
    public class ByTETxt
    {
        private List<string> rooms;
        private List<string> courses;
        private List<string> teachers;
        private List<string> students;

        public List<string> Rooms { get => rooms; }
        public List<string> Courses { get => courses; }
        public List<string> Teachers { get => teachers; }
        public List<string> Students { get => students; }

        public ByTETxt()   
        {
            rooms = new List<string>();
            courses = new List<string>();
            teachers = new List<string>();
            students = new List<string>();
        }

        public void AddRoom(Room room)
        {
            string str = "";
            str += $"{room.Number}({room.RoomType}),";
            str += string.Join(",", room.Courses.Select
                (course => $"({course.course.Code};{course.date})"));
            rooms.Add(str);
        } 

        public void AddCourse(Course course)
        {
            string str = "";
            str += $"{course.Name}#{course.Code}({course.Duration})";
            str += "^";
            str += string.Join(",", course.Teachers.Select(teacher => teacher.Code));
            str += "$";
            str += string.Join(",", course.Students.Select(student => student.Code));
            courses.Add(str);
        }        
        
        public void AddTeacher(Teacher teacher)
        {
            string str = "";
            str += $"{teacher.Surname},";
            str += string.Join(",", teacher.Names);
            str += $"*{teacher.TeacherRank}({teacher.Code})^";
            str += string.Join(",", teacher.Courses.Select(course => course.Code));
            teachers.Add(str);
        }        
        
        public void AddStudent(Student student)
        {
            string str = "";
            str += $"{student.Surname},";
            str += string.Join(",", student.Names);
            str += $"@{student.Semester}({student.Code})^";
            str += string.Join(",", student.Courses.Select(course => course.Code));
            students.Add(str);
        }
    }
    
    public class ByTETxtAdapter : IByTE
    {
        public ByTETxt adaptee;

        public void PrintRooms()
        {
            var regex = new Regex(@"([0-9]+)\(([a-z]+)\),(\(([a-zA-Z0-9]+;[a-z0-9:-]+)\),?)+");

            foreach (string room in adaptee.Rooms)
            {
                foreach (var match in regex.Matches(room))
                {

                }

            }
        }

        public void Print()
        {
            PrintRooms();
        }
    }
    
}