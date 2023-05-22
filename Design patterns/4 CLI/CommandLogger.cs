using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjOb
{
    public static class CommandLogger
    {
        public static void WriteLine(string? str = "", ConsoleColor color = ConsoleColor.Gray)
        {
            using ((ConsoleColorScope)color)
                Console.WriteLine(str);
        }
    }

    public class ConsoleColorScope : IDisposable
    {
        private readonly ConsoleColor previousColor;

        public ConsoleColorScope(ConsoleColor color)
        {
            previousColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
        }

        public static implicit operator ConsoleColorScope(ConsoleColor color)
        {
            return new ConsoleColorScope(color);
        }

        public void Dispose()
        {
            Console.ForegroundColor = previousColor;
        }
    }
}
