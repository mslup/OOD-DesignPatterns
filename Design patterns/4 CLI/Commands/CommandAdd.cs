using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ProjOb;

namespace ProjOb
{
    [DataContract, KnownType(typeof(CommandAdd)),
        KnownType(typeof(AbstractCommand))]
    public class CommandAdd : AbstractCommand, IUndoable
    {
        [DataMember] override public string Arguments { get; set; }
        [DataMember] private string Representation;
        [DataMember] private AbstractBuilder? builder;
        [DataMember] private IFilterable built;

        public CommandAdd()
        {
            Arguments = "";
            Representation = "";
        }

        public override bool Preprocess()
        {
            string[] tokens = Arguments.Split(' ');
            if (tokens.Length > 2)
            {
                Console.WriteLine($"Unrecognized argument: '{tokens[2]}'." +
                    "Usage: add {class_name} [base|secondary]");
                return false;
            }

            Representation = tokens.Length == 2 ? tokens[1] : "base";

            if (!CheckRepresentation(Representation))
                return false;

            if (!GetBuilder(tokens[0], out builder))
                return false;

            return FillBuilder(ref builder, BuilderType.Create);
        }

        public override bool PreprocessFromFile(StreamReader reader)
        {
            string[] tokens = Arguments.Split(' '); 

            Representation = tokens.Length == 2 ? tokens[1] : "base";

            if (!CheckRepresentation(Representation, true))
                return false;

            if (!GetBuilder(tokens[0], out builder, true))
                return false;

            return FillBuilder(ref builder, BuilderType.Create, true, reader);
        }

        public override void Execute()
        {
            if (builder == null)
                return;

            Console.WriteLine("Added object:");
            builder.SetBuildMethods();
            built = (IFilterable)builder.BuildMethods[Representation]();
            Console.WriteLine(built);
        }

        public void Undo()
        {
            Dictionaries.Remove(built);
            Console.WriteLine($"Deleted object: \n{built}");
        }

        public void Redo()
        {
            Dictionaries.Add(built);
            Console.WriteLine($"Created object: \n{built}");
        }

        public override string ToString()
        {
            return $"add {Arguments} \n{builder?.ToString() ?? ""}";
        }
    }
}
