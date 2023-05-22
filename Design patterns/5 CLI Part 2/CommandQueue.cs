using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Enumeration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace ProjOb
{
    public class CommandQueue
    {
        private Queue<AbstractCommand> queue;
        private Dictionary<string, Action<string>> subcommands;
        private readonly Dictionary<string, Action<string>> exportOptions;
        public CommandQueue()
        {
            queue = new();
            subcommands = new()
            {
                ["print"] = Print,
                ["export"] = Export,
                ["commit"] = Commit,
                ["dismiss"] = Dismiss,
                ["load"] = Load
            };
            exportOptions = new()
            {
                ["xml"] = ExportXml,
                ["plaintext"] = ExportPlaintext
            };
        }

        public void HandleQueueCommand(string arg)
        {
            string[] args = arg.Split(' ', 3);
            if (args.Length == 1)
            {
                Console.WriteLine("No subcommand given. " +
                    "Available queue commands: " +
                    string.Join(", ", subcommands.Keys));
                return;
            }

            if (subcommands.TryGetValue(args[1], out Action<string>? act))
            {
                act(args.Length == 3 ? args[2] : "");
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
            queue.Enqueue((AbstractCommand)command);
        }

        public void Commit(string arg)
        {
            if (arg != "")
            {
                Console.WriteLine($"Unrecognized argument: '{arg}'. " +
                    $"Usage: queue commit");
            }

            if (queue.Count == 0)
            {
                Console.WriteLine("Command queue is empty.");
                return;
            }

            while (queue.Count > 0)
            {
                Console.WriteLine();
                ICommand command = (ICommand)queue.Dequeue();
                using ((ConsoleColorScope)ConsoleColor.White)
                    Console.WriteLine(command.ToString());
                command.Execute();
            }
            Console.WriteLine();
        }

        public void Export(string arg)
        {
            string[] args = arg.Split(" ", 3);
            Action<string>? export = ExportXml;
            string filename = "";

            if (args.Length < 1 || arg.Length == 0)
            {
                Console.WriteLine("No argument given. " +
                    "Usage: queue export {filename} [format]");
                return;
            }
            if (args.Length >= 1)
            {
                filename = args[0];
            }
            if (args.Length >= 2)
            {
                if (!exportOptions.TryGetValue(args[1], out export))
                {
                    Console.WriteLine($"Unrecognized export option: '{args[1]}'. " +
                        $"Available options: {string.Join(", ", exportOptions)}");
                    return;
                }
            }
            if (args.Length >= 3)
            {
                Console.WriteLine($"Unrecognized argument: '{args[2]}'. " +
                    "Usage: queue export {filename} [format]");
                return;
            }

            if (export == null)
            {
                Console.WriteLine("Null reference");
                return;
            }

            export(filename);
        }

        private void ExportXml(string filename)
        {
            string cwd = Directory.GetCurrentDirectory();
            string path = Path.Combine(cwd, filename + ".xml");

            var serializer = new DataContractSerializer(typeof(List<AbstractCommand>));

            var settings = new XmlWriterSettings();
            settings.Indent = true;
            using (XmlWriter writer = XmlWriter.Create(path, settings))
            {
                serializer.WriteObject(writer, queue.ToList());
            }

            OpenFile(path);
        }

        private void ExportPlaintext(string filename)
        {
            string cwd = Directory.GetCurrentDirectory();
            string path = Path.Combine(cwd, filename + ".txt");

            using StreamWriter writer = new StreamWriter(path);
            foreach (ICommand command in queue)
                writer.WriteLine(command.ToString());

            OpenFile(path);
        }

        private void OpenFile(string path)
        {
            using Process fileopener = new Process();

            fileopener.StartInfo.FileName = "explorer";
            fileopener.StartInfo.Arguments = "\"" + path + "\"";
            fileopener.Start();
        }

        private void Print(string arg)
        {
            if (arg != "")
            {
                Console.WriteLine($"Unrecognized argument: '{arg}'. " +
                    $"Usage: queue commit");
                return;
            }

            foreach (ICommand command in queue)
            {
                Console.WriteLine(command.ToString());
            }
        }

        private void Dismiss(string arg)
        {
            Console.WriteLine("Queue cleared.");
            queue.Clear();
        }

        private void Load(string arg)
        {
            Action<string>? load = ExportXml;

            if (arg.Length == 0)
            {
                Console.WriteLine("No argument given. " +
                    "Usage: queue export {filename}");
                return;
            }

            string ext = Path.GetExtension(arg);
            switch (ext)
            {
            case ".txt":
                LoadPlaintext(arg);
                return;
            case ".xml":
                LoadXml(arg);
                return;
            default:
                Console.WriteLine($"Unrecognized extension: `{ext}`. " +
                    $"Possible extensions: txt, xml");
                return;
            }

        }

        private void LoadXml(string filename)
        {
            string cwd = Directory.GetCurrentDirectory();
            string path = Path.Combine(cwd, filename);

            var serializer = new DataContractSerializer(typeof(List<AbstractCommand>));

            FileStream fs;
            try
            {
                fs = new FileStream(path, FileMode.Open);
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine($"Could not find file '{filename}'.");
                return;
            }

            List<AbstractCommand> list;

            using (XmlReader reader = XmlReader.Create(fs))
            {
                list = (List<AbstractCommand>)serializer.ReadObject(reader);
            }

            foreach (var element in list)
            {
                queue.Enqueue(element);
            }
        }

        private void LoadPlaintext(string filename)
        {
            string cwd = Directory.GetCurrentDirectory();
            string path = Path.Combine(cwd, filename);

            try
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    string? line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        //Console.WriteLine(line);

                        var command = CommandFactory.BuildCommand(line);

                        if (command == null)
                            continue;

                        if (command.PreprocessFromFile(reader))
                            queue.Enqueue((AbstractCommand)command); // yikes
                    }

                }
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine($"Could not find file '{filename}'.");
            }
        }

    }
}
