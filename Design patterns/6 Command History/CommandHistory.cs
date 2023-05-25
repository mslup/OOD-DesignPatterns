using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjOb
{
    public static class CommandHistory
    {
        static private Stack<AbstractCommand> done = new();
        static private Stack<AbstractCommand> undone = new();

        public static readonly Dictionary<string, Action> HistoryCommandDictionary
         = new()
         {
             ["history"] = Print,
             ["undo"] = Undo,
             ["redo"] = Redo,
             ["export"] = Export,
             ["load"] = Load
         };

        static public bool HandleHistoryCommands(string arg)
        {
            Action action;
            if (!HistoryCommandDictionary.TryGetValue(arg, out action))
                return false;

            action();
            return true;
        }

        static public void Print()
        {
            if (done.Count == 0)
            {
                Console.WriteLine("History empty.");
                return;
            }
               
            foreach (var command in done.Reverse())
                Console.WriteLine(command);
        }

        static public void Undo()
        {
            if (done.Count == 0)
            {
                Console.WriteLine("Nothing to undo.");
                return;
            }

            var cmd = done.Pop();
            cmd.Undo();
            undone.Push(cmd);
            Console.Write("Undone: '");
            using ((ConsoleColorScope)ConsoleColor.White)
                Console.Write(cmd);
            Console.Write("'\n");
        }

        static public void Redo()
        {
            if (undone.Count == 0)
            {
                Console.WriteLine("Nothing to redo.");
                return;
            }

            var cmd = undone.Pop();
            using((ConsoleColorScope) ConsoleColor.White)
                Console.WriteLine(cmd);
            cmd.Redo();
            done.Push(cmd);
        }

        static public void Export()
        {

        }

        static public void Load() 
        {
        
        }


        static public void Register(AbstractCommand command)
        {
            done.Push(command);
            undone.Clear();
        }
    }
}
