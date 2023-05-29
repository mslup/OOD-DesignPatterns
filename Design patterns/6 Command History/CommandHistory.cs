using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ProjOb
{
    public static class CommandHistory
    {
        [DataMember] static private Stack<IUndoable> done = new();
        [DataMember] static private Stack<IUndoable> undone = new();

        private static readonly Type[] knownSerializableTypes = new Type[]
        {
            typeof(CommandAdd),
            typeof(CommandDelete),
            typeof(CommandEdit),
            typeof(Room),
            typeof(RoomPartialTxtAdapter),
            typeof(Course),
            typeof(CoursePartialTxtAdapter),
            typeof(Student),
            typeof(StudentPartialTxtAdapter),
            typeof(Teacher),
            typeof(TeacherPartialTxtAdapter)
        };

        public static readonly Dictionary<string, Action<string>>
            HistoryCommandDictionary = new()
            {
                ["history"] = Print,
                ["undo"] = Undo,
                ["redo"] = Redo,
                ["export"] = Export,
                ["load"] = Load
            };

        private static readonly Dictionary<string, Action<string>>
            exportOptions = new()
            {
                ["xml"] = ExportXml,
                ["plaintext"] = ExportPlaintext
            };

        static public bool HandleHistoryCommands(string arg)
        {
            string[] args = arg.Split(' ', 2);

            if (HistoryCommandDictionary.TryGetValue(args[0],
                out Action<string>? action))
            {
                action(args.Length == 2 ? args[1] : "");
            }
            else
                return false;

            return true;
        }

        static public void Print(string arg)
        {
            if (done.Count == 0)
            {
                Console.WriteLine("History empty.");
                return;
            }

            foreach (var command in done.Reverse())
                Console.WriteLine(command);
        }

        static public void Undo(string arg)
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

        static public void Redo(string arg)
        {
            if (undone.Count == 0)
            {
                Console.WriteLine("Nothing to redo.");
                return;
            }

            var cmd = undone.Pop();
            using ((ConsoleColorScope)ConsoleColor.White)
                Console.WriteLine(cmd);
            cmd.Redo();
            done.Push(cmd);
        }

        static public void Export(string arg)
        {
            string[] args = arg.Split(" ", 3);
            Action<string>? export = ExportXml;
            string filename = "";

            if (args.Length < 1 || arg.Length == 0)
            {
                Console.WriteLine("No argument given. " +
                    "Usage: export {filename} [format]");
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
                    "Usage: export {filename} [format]");
                return;
            }

            if (export == null)
            {
                Console.WriteLine("Null reference");
                return;
            }

            export(filename);
        }

        static private void ExportXml(string filename)
        {
            string cwd = Directory.GetCurrentDirectory();
            string path = Path.Combine(cwd, filename + ".xml");

            var serializer = new DataContractSerializer(
                typeof(List<IUndoable>),
                knownSerializableTypes);

            var settings = new XmlWriterSettings();
            settings.Indent = true;
            using (XmlWriter writer = XmlWriter.Create(path, settings))
            {
                serializer.WriteObject(writer, done.Reverse().ToList());
            }

            OpenFile(path);
        }

        static private void ExportPlaintext(string filename)
        {
            string cwd = Directory.GetCurrentDirectory();
            string path = Path.Combine(cwd, filename + ".txt");

            using StreamWriter writer = new StreamWriter(path);
            foreach (IUndoable command in done.Reverse())
                writer.WriteLine(command.ToString());

            OpenFile(path);
        }

        static private void OpenFile(string path)
        {
            using Process fileopener = new Process();

            fileopener.StartInfo.FileName = "explorer";
            fileopener.StartInfo.Arguments = "\"" + path + "\"";
            fileopener.Start();
        }

        static private void Load(string arg)
        {
            Action<string> load;

            if (arg.Length == 0)
            {
                Console.WriteLine("No argument given. " +
                    "Usage: export {filename}");
                return;
            }

            string ext = Path.GetExtension(arg);
            switch (ext)
            {
            case ".txt":
                load = LoadPlaintext;
                break;
            case ".xml":
                load = LoadXml;
                break;
            default:
                Console.WriteLine($"Unrecognized extension: `{ext}`. " +
                    $"Possible extensions: txt, xml");
                return;
            }

            load(arg);

            foreach (var command in done.Reverse())
                (command as AbstractCommand)!.Execute();
            
            //var tmpStack = new Stack<IUndoable>();
            //foreach (var command in done)
            //    tmpStack.Push(command);
            //done = tmpStack;
        }

        static private void LoadXml(string filename)
        {
            string cwd = Directory.GetCurrentDirectory();
            string path = Path.Combine(cwd, filename);

            var serializer = new DataContractSerializer(
                typeof(List<IUndoable>),
                knownSerializableTypes);

            FileStream fs;
            try
            {
                fs = new FileStream(path, FileMode.Open);
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"Could not find file '{filename}'.");
                return;
            }

            List<IUndoable> list;

            using (XmlReader reader = XmlReader.Create(fs))
            {
                list = (List<IUndoable>)serializer.ReadObject(reader);
            }

            if (list == null || list.Count == 0)
                return;

            foreach (var element in list)
            {
                done.Push(element);
            }
        }

        static private void LoadPlaintext(string filename)
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
                            done.Push((IUndoable)command); // yikes
                    }

                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"Could not find file '{filename}'.");
            }
        }


        static public void Register(IUndoable command)
        {
            done.Push(command);
            undone.Clear();
        }
    }
}
