using System.Text;

namespace ProjOb
{
    public static class CommandFactory
    {
        public static readonly Dictionary<string, Func<ICommand>> CommandDictionary
            = new Dictionary<string, Func<ICommand>>
        {
            { "list", () => new CommandList() },
            { "find", () => new CommandFind() },
            { "add", () => new CommandAdd() },
            { "edit", () => new CommandEdit() },
            { "delete", () => new CommandDelete() }
        };

        public static ICommand? BuildCommand(string arg)
        {
            arg = arg.Trim();
            string[] tokens = arg.Split(' ', 2);

            if (CommandDictionary.TryGetValue(tokens[0], out Func<ICommand>? commandConstructor))
            {
                ICommand command = commandConstructor();
                if (tokens.Length > 1)
                    command.Arguments = tokens[1];
                else
                    command.Arguments = "";

                return command;
            }

            Console.WriteLine($"Unrecognized command: '{arg.Trim().Split(' ')[0]}'");
            return null;
        }
    }

    public static class CLI
    {
        public static void RunApp()
        {
            ByTE.ConstructByTE();

            Console.OutputEncoding = Encoding.UTF8;
            using ((ConsoleColorScope)ConsoleColor.Cyan)
                Console.WriteLine("=== Application ByTE ===\n");

            var queue = new CommandQueue();

            while (true)
            {
                using ((ConsoleColorScope)ConsoleColor.White)
                    Console.Write("> ");

                string? arg = Console.ReadLine();

                if (arg == null || arg.Length == 0)
                    continue;
                arg = arg.Trim();

                if (arg.ToLower() == "exit")
                    break;

                if (arg.StartsWith("queue"))
                {
                    queue.HandleQueueCommand(arg);
                    continue;
                }

                var command = CommandFactory.BuildCommand(arg);

                if (command == null)
                    continue;

                if (command.Preprocess())
                    queue.Push(command);

            }
        }
    }
}
