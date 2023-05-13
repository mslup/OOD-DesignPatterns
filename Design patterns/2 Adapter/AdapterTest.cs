using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjOb
{
    public static class AdapterTest
    {
        public static ByTEPartialTxt ConstructByTEPartialTxt()
        { 
            ByTE Uni = ByTE.ConstructByTE();

            var UniPartialTxt = new ByTEPartialTxt();
            var Converter = new ByTEPartialTxtConverter(UniPartialTxt);
            Converter.AddRooms(Uni.Rooms.Select(x => x as Room).ToArray() as Room[]);
            Converter.AddCourses(Uni.Courses.Select(x => x as Course).ToArray() as Course[]);
            Converter.AddStudents(Uni.Students.Select(x => x as Student).ToArray() as Student[]);
            Converter.AddTeachers(Uni.Teachers.Select(x => x as Teacher).ToArray() as Teacher[]);

            return UniPartialTxt;
        }

        public static void Project2_Test()
        {
            ByTEPartialTxt Uni = ConstructByTEPartialTxt();
            var Adapter = new ByTEPartialTxtAdapter(Uni);
            Console.WriteLine(Adapter);

            Console.WriteLine("===");
            Console.WriteLine("Wypisać zajęcia na które uczęszcza przynajmniej jeden student " +
                "mający 2 imiona oraz nauczyciel mający 2 imiona (nazwa, kod, czas " +
                "trwania ten student i nauczyciel)");

            IStudent? TwoNameStudent = null;
            ITeacher? TwoNameTeacher = null;

            foreach (var course in Adapter.Courses)
            {
                TwoNameStudent = null;
                TwoNameTeacher = null;

                foreach (var student in course.Students)
                {
                    if (student.Names.Count == 2)
                    {
                        TwoNameStudent = student;
                        break;
                    }
                }

                foreach (var teacher in course.Teachers)
                {
                    if (teacher.Names.Count == 2)
                    {
                        TwoNameTeacher = teacher;
                        break;
                    }
                }

                if (TwoNameStudent != null && TwoNameTeacher != null)
                    Console.WriteLine($"{course.Name}, {course.Code}, " +
                        $"{course.Duration}, " +
                        $"{string.Join(" ", TwoNameStudent.Names)} " +
                        $"{TwoNameStudent.Surname}, " +
                        $"{string.Join(" ", TwoNameTeacher.Names)} " +
                        $"{TwoNameTeacher.Surname}");
            }
        }
    }
}
