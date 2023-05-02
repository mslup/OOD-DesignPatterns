using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjOb;

namespace ProjOb
{
    // Converts ByTE object (base representation (0)) to a ByTEPartialTxt object (partial text representation (3)).
    public class ByTEPartialTxtConverter
    {
        private ByTEPartialTxt convertee;

        public ByTEPartialTxtConverter(ByTEPartialTxt convertee)
        { this.convertee = convertee; }

        public RoomPartialTxt ConvertRoom(Room room)
        {
            return new RoomPartialTxt
                (room.Number, room.RoomType.ToString(),
                string.Join(",", room.Courses.Select(course => $"({course.Code})")));
        }

        public CoursePartialTxt ConvertCourse(Course course)
        {
            return new CoursePartialTxt
                (course.Name, course.Code, course.Duration,
                string.Join(",", course.Teachers.Select(teacher => teacher.Code)) +
                "$" +
                string.Join(",", course.Students.Select(student => student.Code)));
        }

        public TeacherPartialTxt ConvertTeacher(Teacher teacher)
        {
            return new TeacherPartialTxt
                (teacher.Surname + "," + string.Join(",", teacher.Names),
                teacher.TeacherRank.ToString(), teacher.Code,
                string.Join(",", teacher.Courses.Select(course => course.Code)));
        }

        public StudentPartialTxt ConvertStudent(Student student)
        {
            return new StudentPartialTxt
                (student.Surname + "," + string.Join(",", student.Names),
                student.Semester, student.Code,
                string.Join(",", student.Courses.Select(course => course.Code)));
        }

        public void AddRooms(params Room[] rooms)
        {
            foreach (var room in rooms)
                convertee.AddRoom(ConvertRoom(room));
        }

        public void AddCourses(params Course[] courses)
        {
            foreach (var course in courses)
                convertee.AddCourse(ConvertCourse(course));
        }

        public void AddTeachers(params Teacher[] teachers)
        {
            foreach (var teacher in teachers)
                convertee.AddTeacher(ConvertTeacher(teacher));
        }

        public void AddStudents(params Student[] students)
        {
            foreach (var student in students)
                convertee.AddStudent(ConvertStudent(student));
        }


    }
}
