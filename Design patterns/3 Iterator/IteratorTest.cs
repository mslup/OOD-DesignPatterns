using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjOb
{
    static class IteratorTest
    {
        public static object? Find<T>(INewCollection<T> collection, Predicate<T> predicate)
        {
            IIterator<T> it = collection.GetForwardsIterator();
            T val;

            while (it.HasNext())
            {
                val = it.GetNext();
                if (predicate(val))
                    return val;
            }

            return null;
        }

        public static object? ForEach<T>(INewCollection<T> collection, Action<T> action)
        {
            IIterator<T> it = collection.GetForwardsIterator();
            T val;

            while (it.HasNext())
            {
                val = it.GetNext();
                action(val);
            }

            return null;
        }

        public static int CountIf<T>(INewCollection<T> collection, Predicate<T> predicate)
        {
            IIterator<T> it = collection.GetForwardsIterator();
            T val;
            int num = 0;

            while (it.HasNext())
            {
                val = it.GetNext();
                if (predicate(val))
                    num++;
            }

            return num;
        }

        public static void Project3_Test()
        {
            var arr = new SquareArray<int>();
            for (int i = 0; i < 20; i++)
                arr.Add(i);

            ForEach<int>(arr, (i => Console.Write($"{i} ")));
            Console.WriteLine();

            Console.WriteLine(Find<int>(arr, (d => d == 4)));
            Console.WriteLine(Find<int>(arr, (d => d == 0)));
            Console.WriteLine(Find<int>(arr, (d => d == 19)));
            Console.WriteLine(Find<int>(arr, (d => d == -1)) == null ? "null" : "sth");

            Console.WriteLine($"Numbers less than 5: " +
                $"{CountIf<int>(arr, (d => d < 5))}");

            for (int i = 0; i < 30; i++)
                arr.RemoveLast();
        }
    }
}
