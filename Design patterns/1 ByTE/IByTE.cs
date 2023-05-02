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
        public int Number { get; }
        public RoomTypeEnum RoomType { get; }
        public List<ICourse> Courses { get; }
        public string ToString();
    }

    public interface ICourse : IFilterable
    {
        public string Name { get; }
        public string Code { get; }
        public int Duration { get; }
        public List<ITeacher> Teachers { get; }
        public List<IStudent> Students { get; }
        public string ToString();
    }

    public interface ITeacher : IFilterable
    {
        public enum TeacherRankEnum { KiB, MiB, GiB, TiB };
        public List<string> Names { get; }
        public string Surname { get; }
        public string Code { get; }
        public TeacherRankEnum TeacherRank { get; }
        public List<ICourse> Courses { get; }
        public string ToString();
    }

    public interface IStudent : IFilterable
    {
        public List<string> Names { get; }
        public string Surname { get; }
        public string Code { get; }
        public int Semester { get; }
        public List<ICourse> Courses { get; }
        public string ToString();
    }
}
