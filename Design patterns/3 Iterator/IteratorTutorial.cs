using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ProjOb
{
    public class DLList<T> : INewCollection<T>
    {
        private int _count;
        private DLLNode? _head;
        private DLLNode? _tail;

        public int Count => _count;
        internal DLLNode? Head => _head;
        internal DLLNode? Tail => _tail;
            
        public DLList()
        {
            _head = null;
            _tail = null;
            _count = 0;
        }

        public void Add(T item)
        {
            DLLNode node = new DLLNode(item);
            _count++;

            if (_head == null && _tail == null)
            {
                _head = _tail = node;
                return;
            }

            _tail!.Next = node;
            node.Prev = _tail;
            node.Next = null;
            _tail = node;
        }

        public void Clear()
        {
            while (Count > 0)
                RemoveLast();
        }

        public void RemoveLast()
        {
            if (_tail == null)
                return;

            DLLNode node = _tail;
            _tail = _tail.Prev;
            node.Next = null;
            node.Prev = null;

            _count--;
        }

        internal class DLLNode
        {
            public T value;
            public DLLNode? Next;
            public DLLNode? Prev;

            public DLLNode(T value)
            {
                this.value = value;
                Next = null;
                Prev = null;
            }
        }

        public IIterator<T> GetForwardsIterator()
        {
            return new ListForwardsIterator<T>(this);
        }

        public IIterator<T> GetBackwardsIterator()
        {
            return new ListBackwardsIterator<T>(this);
        }
    }

    public class ListForwardsIterator<T> : IIterator<T>
    {
        private DLList<T> list;
        private DLList<T>.DLLNode? state;

        public ListForwardsIterator(DLList<T> list)
        {
            this.list = list;
            this.state = null;
        }

        public bool HasNext()
            => list.Head != null &&
            (state == null || 
            (state != null && state.Next != null));

        public T GetNext()
        {
            if (!HasNext())
                throw new IndexOutOfRangeException();

            if (state == null)
                state = list.Head;
            else
                state = state!.Next;

            return state!.value;
        }
    }

    public class ListBackwardsIterator<T> : IIterator<T>
    {
        private DLList<T> list;
        private DLList<T>.DLLNode? state;

        public ListBackwardsIterator(DLList<T> list)
        {
            this.list = list;
            this.state = null;
        }

        public bool HasNext()
            => list.Tail != null &&
            (state == null ||
            (state != null && state.Prev != null));

        public T GetNext()
        {
            if (!HasNext())
                throw new IndexOutOfRangeException();

            if (state == null)
                state = list.Tail;
            else
                state = state!.Prev;

            return state!.value;
        }
    }

    public class Vector<T> : INewCollection<T>
    {
        private int _count;
        static private int defaultCapacity = 100;
        private int capacity;
        private T[] content;

        public Vector()
        {
            capacity = defaultCapacity;
            content = new T[capacity];
            _count = 0;
        }

        public int Count => _count;
        public int Capacity => capacity;

        public void Add(T item)
        {
            if (_count == capacity)
            {
                T[] tmp = new T[capacity + defaultCapacity];
                content.CopyTo(tmp, 0);
                content = tmp;
            }
            content[_count] = item;
            _count++;
        }

        public void Clear()
        {
            while (Count > 0)
                RemoveLast();
        }

        public void RemoveLast()
        {
            content[--_count] = default(T)!;
        }

        public T this[int index]
        {
            get => content[index];
            set => content[index] = value;
        }

        public IIterator<T> GetForwardsIterator()
        {
            return new VectorForwardsIterator<T>(this);
        }

        public IIterator<T> GetBackwardsIterator()
        {
            return new VectorBackwardsIterator<T>(this);
        }
    }
    
    public class VectorForwardsIterator<T> : IIterator<T>
    {
        private Vector<T> vector;
        private int state;

        public VectorForwardsIterator(Vector<T> collection)
        {
            this.vector = collection;
            this.state = -1;
        }

        public bool HasNext()
            => vector.Count != 0 && state < vector.Count - 1;

        public T GetNext()
        {
            if (!HasNext())
                throw new IndexOutOfRangeException();

            state++;
            return vector[state];
        }
    }

    public class VectorBackwardsIterator<T> : IIterator<T>
    {
        private Vector<T> vector;
        private int state;

        public VectorBackwardsIterator(Vector<T> collection)
        {
            this.vector = collection;
            this.state = vector.Count;
        }

        public bool HasNext()
            => vector.Count != 0 && state > 0;

        public T GetNext()
        {
            if (!HasNext())
                throw new IndexOutOfRangeException();

            state--;
            return vector[state];
        }
    }

}
