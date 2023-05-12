namespace ProjOb
{
    public class CommandFactory
    {
        public static readonly Dictionary<string, Func<ICommand>> CommandDictionary
            = new Dictionary<string, Func<ICommand>>
        {
            { "list", () => new CommandList() },
            { "find", () => new CommandFind() },
            { "add", () => new CommandAdd() },
            { "edit", () => new CommandEdit() }
        };

        public ICommand? BuildCommand(string arg)
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

            return null;
        }
    }

    public static class CLI
    {
        public static string Prompt { get => "> "; }
        public static void RunApp()
        {
            ByTE.ConstructByTE();
            Console.WriteLine("Application ByTE");

            var queue = new CommandQueue();
            var factory = new CommandFactory();

            while (true)
            {
                Console.Write(Prompt);
                string? arg = Console.ReadLine();

                if (arg == null || arg.Length == 0)
                    continue;

                if (arg.ToLower() == "exit")
                    break;
                
                var command = factory.BuildCommand(arg);

                if (arg.StartsWith("queue"))
                {
                    queue.HandleQueueCommand(arg);
                    continue;
                }

                if (command != null)
                {
                    if (command.Preprocess())
                        queue.Push(command);
                }
                else
                    Console.WriteLine($"Unrecognized command: {arg.Trim().Split(' ')[0]}");
            }
        }
    }
}
