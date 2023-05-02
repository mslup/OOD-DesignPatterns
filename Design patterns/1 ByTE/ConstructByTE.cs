using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjOb
{
    public partial class ByTE
    {
        public static ByTE ConstructByTE()
        {
            //Construct students
            Student S1 = new Student("Robert", "Kielbica", 3, "S1");
            Student S2 = new Student(new string[] { "Archibald", "Agapios" }, "Linux", 7, "S2");
            Student S3 = new Student("Angrboða", "Kára", 1, "S3");
            Student S4 = new Student("Olympos", "Andronikos", 5, "S4");
            Student S5 = new Student(new string[] { "Mac", "Rhymes" }, "Pickuppicker", 6, "S5");

            // Construct teachers
            Teacher P1 = new Teacher("Tomas", "Cherrmann", ITeacher.TeacherRankEnum.MiB, "P1");
            Teacher P2 = new Teacher("Jon", "Tron", ITeacher.TeacherRankEnum.TiB, "P2");
            Teacher P3 = new Teacher(new string[] { "William", "Joseph" }, "Blazkowicz", ITeacher.TeacherRankEnum.GiB, "P3");
            Teacher P4 = new Teacher(new string[] { "Arkadiusz", "Amadeusz" }, "Kamiński", ITeacher.TeacherRankEnum.KiB, "P4");
            Teacher P5 = new Teacher("Cooking Mama", "GiB", ITeacher.TeacherRankEnum.GiB, "P5");

            // Construct courses
            Course MD2 = new Course("Diabolical Mathematics 2", "MD2", 2);
            MD2.LinkStudents(S1, S2, S5);
            MD2.LinkTeachers(P2);
            Course RD = new Course("Routers descriptions", "RD", 1);
            RD.LinkStudents(S3, S4);
            RD.LinkTeachers(P3);
            Course WDK = new Course("Introduction to cables", "WDK", 5);
            WDK.LinkStudents(S1, S2, S3, S4, S5);
            WDK.LinkTeachers(P4, P3);
            Course AC3 = new Course("Advanced Cooking 3", "AC3", 3);
            AC3.LinkStudents(S2, S4, S5);
            AC3.LinkTeachers(P5, P1);

            // Construct rooms
            Room R107 = new Room(107, IRoom.RoomTypeEnum.lecture);
            R107.AddCourses(MD2, RD, WDK, AC3);
            Room R204 = new Room(204, IRoom.RoomTypeEnum.tutorials);
            R204.AddCourses(WDK, AC3);
            Room R21 = new Room(21, IRoom.RoomTypeEnum.lecture);
            R21.AddCourses(RD);
            Room R123 = new Room(123, IRoom.RoomTypeEnum.laboratory);
            R123.AddCourses(RD, WDK);
            Room R404 = new Room(404, IRoom.RoomTypeEnum.lecture);
            R404.AddCourses(MD2, WDK, RD);
            Room R504 = new Room(504, IRoom.RoomTypeEnum.tutorials);
            R504.AddCourses(MD2);
            Room R73 = new Room(73, IRoom.RoomTypeEnum.laboratory);
            R73.AddCourses(AC3);

            ByTE Uni = new ByTE();
            Uni.AddRooms(R107, R204, R21, R123, R404, R504, R73);
            Uni.AddCourses(MD2, RD, WDK, AC3);
            Uni.AddStudents(S1, S2, S3, S4, S5);
            Uni.AddTeachers(P1, P2, P3, P4, P5);

            return Uni;
        }
    }
}
