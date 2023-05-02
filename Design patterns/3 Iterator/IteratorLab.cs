using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProjOb
{
    public interface INewCollection<T>
    {
        int Count { get; }
        void Add(T item);
        void RemoveLast();
        IIterator<T> GetForwardsIterator();
        IIterator<T> GetBackwardsIterator();
    }

    public interface IIterator<T>
    {
        bool HasNext();
        T GetNext();
    }

    public class SquareArray<T> : INewCollection<T>
    {
        private T[,] contents;
        public int Count { get; private set; }
        public int SquareSize { get; private set; }
        public (int i, int j) LastIndex { get; private set; }

        public T this[(int i, int j) t]
        {
            get { return contents[t.i, t.j]; }
        }

        public SquareArray()
        {
            SquareSize = 1;
            contents = new T[SquareSize, SquareSize];
            Count = 0;
            LastIndex = (0, 0);
        }

        private void Resize()
        {
            var tmp = new T[SquareSize + 1, SquareSize + 1];
            for (int i = 0; i < SquareSize; i++)
                for (int j = 0; j < SquareSize; j++)
                    tmp[i, j] = contents[i, j];

            contents = tmp;
            SquareSize++;
        }

        public void Add(T item)
        {
            contents[LastIndex.i, LastIndex.j] = item;

            if (LastIndex.i == LastIndex.j)
            {
                if (LastIndex.j == SquareSize - 1)
                    Resize();
                LastIndex = (0, LastIndex.j + 1);
            }
            else if (LastIndex.i > LastIndex.j)
            {
                LastIndex = (LastIndex.i, LastIndex.j + 1);
            }
            else if (LastIndex.i == LastIndex.j - 1)
            {
                LastIndex = (LastIndex.i + 1, 0);
            }
            else if (LastIndex.i < LastIndex.j - 1)
            {
                LastIndex = (LastIndex.i + 1, LastIndex.j);
            }

            Count++;
        }

        public void RemoveLast()
        {
            if (Count == 0)
                return;

            if (LastIndex.i == LastIndex.j)
            {
                LastIndex = (LastIndex.i, LastIndex.j - 1);
            }
            else if (LastIndex.i == 0)
            {
                LastIndex = (LastIndex.j - 1, LastIndex.j - 1);
            }
            else if (LastIndex.j == 0)
            {
                LastIndex = (LastIndex.i - 1, LastIndex.i);
            }
            else if (LastIndex.i < LastIndex.j)
            {
                LastIndex = (LastIndex.i - 1, LastIndex.j);
            }
            else if (LastIndex.i > LastIndex.j)
            {
                LastIndex = (LastIndex.i, LastIndex.j - 1);
            }

            contents[LastIndex.i, LastIndex.j] = default(T)!;

            Count--;
        }

        public IIterator<T> GetForwardsIterator()
        {
            return new SquareForwardsIterator<T>(this);
        }
        public IIterator<T> GetBackwardsIterator()
        {
            return new SquareBackwardsIterator<T>(this);
        }

    }

    public class SquareForwardsIterator<T> : IIterator<T>
    {
        private SquareArray<T> array;
        private (int i, int j) indices;
        private int elementNumber;

        public SquareForwardsIterator(SquareArray<T> array)
        {
            this.array = array;
            this.indices = (-1, -1);
            elementNumber = -1;
        }

        public bool HasNext()
        {
            return elementNumber != array.Count - 1;
        }

        public T GetNext()
        {
            if (!HasNext())
                throw new IndexOutOfRangeException();

            if (indices == (-1, -1))
                indices = (0, 0);
            else
            {
                if (indices.i == indices.j)
                {
                    indices = (0, indices.j + 1);
                }
                else if (indices.i > indices.j)
                {
                    indices = (indices.i, indices.j + 1);
                }
                else if (indices.i == indices.j - 1)
                {
                    indices = (indices.i + 1, 0);
                }
                else if (indices.i < indices.j - 1)
                {
                    indices = (indices.i + 1, indices.j);
                }
            }

            elementNumber++;
            return array[indices];
        }
    }

    public class SquareBackwardsIterator<T> : IIterator<T>
    {
        private SquareArray<T> array;
        private (int i, int j) indices;
        private int elementNumber;

        public SquareBackwardsIterator(SquareArray<T> array)
        {
            this.array = array;
            this.indices = array.LastIndex;
            elementNumber = array.Count - 1;
        }

        public bool HasNext()
        {
            return elementNumber != -1;
        }

        public T GetNext()
        {
            if (!HasNext())
                throw new IndexOutOfRangeException();

            if (indices.i == indices.j)
            {
                indices = (indices.i, indices.j - 1);
            }
            else if (indices.i == 0)
            {
                indices = (indices.j - 1, indices.j - 1);
            }
            else if (indices.j == 0)
            {
                indices = (indices.i - 1, indices.i);
            }
            else if (indices.i < indices.j)
            {
                indices = (indices.i - 1, indices.j);
            }
            else if (indices.i > indices.j)
            {
                indices = (indices.i, indices.j - 1);
            }

            elementNumber--;
            return array[indices];
        }

    }
}
