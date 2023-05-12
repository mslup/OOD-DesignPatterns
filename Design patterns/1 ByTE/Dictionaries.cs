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
                { "room", RoomDict },
                { "rooms", RoomDict },
                { "course", CourseDict },
                { "courses", CourseDict },
                { "teacher", TeacherDict },
                { "teachers", TeacherDict },
                { "student", StudentDict },
                { "students", StudentDict },
            };

        static public void AddRoom(IRoom room)
        {
            RoomDict[room.Number.ToString()] = room;
        }

        static public void AddCourse(ICourse course)
        {
            CourseDict[course.Code] = course;
        }

        static public void AddTeacher(ITeacher teacher)
        {
            TeacherDict[teacher.Code] = teacher;
        }

        static public void AddStudent(IStudent student)
        {
            StudentDict[student.Code] = student;
        }
    }
}
