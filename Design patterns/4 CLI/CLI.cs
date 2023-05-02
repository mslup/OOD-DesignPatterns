namespace ProjOb
{
    public class CommandFactory
    {
        public static readonly Dictionary<string, ICommand> CommandDictionary
            = new Dictionary<string, ICommand>
        {
            { "list", new CommandList() },
            { "find", new CommandFind() },
            { "add",  new CommandAdd() }
        };

        //private CommandBuilder builder = new CommandBuilder();

        public ICommand? BuildCommand(string? arg)
        {
            if (arg == null) return null;

            arg = arg.Trim();
            string[] tokens = arg.Split(' ', 2);

            if (CommandDictionary.TryGetValue(tokens[0], out ICommand? command))
            {
                if (tokens.Length > 1)
                    command.Argument = tokens[1];
                else
                    command.Argument = "";

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

            var factory = new CommandFactory();

            while (true)
            {
                Console.Write(Prompt);
                string? arg = Console.ReadLine();

                if (arg == "exit")
                    break;

                var command = factory.BuildCommand(arg);
                if (command != null)
                    command.Execute();
                else
                    Console.WriteLine("No such command");
            }

        }


    }
}
