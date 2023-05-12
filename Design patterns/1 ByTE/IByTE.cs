using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjOb
{
    public interface IByTE
    {
        public List<IRoom> Rooms { get; }
        public List<ICourse> Courses { get; }
        public List<ITeacher> Teachers { get; }
        public List<IStudent> Students { get; }

        public string ToString();
    }

    public interface IRoom : IFilterable
    {
        public enum RoomTypeEnum { laboratory, tutorials, lecture, other };
        public int Number { get; set; }
        public RoomTypeEnum RoomType { get; set; }
        public List<ICourse> Courses { get; }
        public string ToString();
    }

    public interface ICourse : IFilterable
    {
        public string Name { get; set;  }
        public string Code { get; set;  }
        public int Duration { get; set; }
        public List<ITeacher> Teachers { get; }
        public List<IStudent> Students { get; }
        public string ToString();
    }

    public interface ITeacher : IFilterable
    {
        public enum TeacherRankEnum { KiB, MiB, GiB, TiB };
        public List<string> Names { get; set;  }
        public string Surname { get; set;  }
        public string Code { get; set; }
        public TeacherRankEnum TeacherRank { get; set; }
        public List<ICourse> Courses { get; }
        public string ToString();
    }

    public interface IStudent : IFilterable
    {
        public List<string> Names { get; set; }
        public string Surname { get; set; }
        public string Code { get; set; }
        public int Semester { get; set; }
        public List<ICourse> Courses { get; }
        public string ToString();
    }
}
