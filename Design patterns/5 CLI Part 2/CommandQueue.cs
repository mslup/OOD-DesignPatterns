using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjOb
{
    public class CommandQueue
    {
        private Queue<ICommand> queue;
        private Dictionary<string, Action> subcommands;
        public CommandQueue()
        {
            queue = new Queue<ICommand>();
            subcommands = new Dictionary<string, Action>
            {
                ["print"] = Print,
                ["export"] = Export,
                ["commit"] = Commit
            };
        }

        public void HandleQueueCommand(string arg)
        {
            string[] args = arg.Split(' ', 2);
            if (subcommands.TryGetValue(args[1], out Action? act))
            {
                act();
            }
            else
            {
                Console.WriteLine($"Unrecognized command: {args[1]}. " +
                    "Available queue commands: " + 
                    string.Join(", ", subcommands.Keys));
            }
        }

        public void Push(ICommand command)
        {
            queue.Enqueue(command);
        }

        public void Commit()
        {
            while (queue.Count > 0)
            {
                Console.WriteLine();
                ICommand command = queue.Dequeue();
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(command.ToString());
                Console.ForegroundColor = ConsoleColor.Gray;
                command.Execute();
            }
            Console.WriteLine();
        }

        public void Export()
        {

        }

        public void Print()
        {
            foreach (ICommand command in queue)
            {
                Console.WriteLine(command.ToString());
            }
        }
    }
}
