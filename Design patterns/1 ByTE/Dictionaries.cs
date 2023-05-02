using System.Collections;

namespace ProjOb
{
    public static class Dictionaries
    {
        public static Dictionary<string, IRoom> RoomDict
            = new Dictionary<string, IRoom>();
        public static Dictionary<string, ICourse> CourseDict
            = new Dictionary<string, ICourse>();
        public static Dictionary<string, ITeacher> TeacherDict
            = new Dictionary<string, ITeacher>();
        public static Dictionary<string, IStudent> StudentDict
            = new Dictionary<string, IStudent>();

        public static readonly Dictionary<string, IDictionary?>
            objectDictionary = new Dictionary<string, IDictionary?> 
            { 
                { "rooms", RoomDict }, 
                { "courses", CourseDict },
                { "teachers", TeacherDict },
                { "students", StudentDict },
            };

        static public void AddRooms(params Room[] rooms)
        {
            foreach (var room in rooms)
                RoomDict[room.Number.ToString()] = room;
        }

        static public void AddCourses(params Course[] courses)
        {
            foreach (var course in courses)
                CourseDict[course.Code] = course;
        }

        static public void AddTeachers(params Teacher[] teachers)
        {
            foreach (var teacher in teachers)
                TeacherDict[teacher.Code] = teacher;
        }

        static public void AddStudents(params Student[] students)
        {
            foreach (var student in students)
                StudentDict[student.Code] = student;
        }
    }
}
