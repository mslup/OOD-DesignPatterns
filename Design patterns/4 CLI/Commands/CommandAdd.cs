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
    public class CommandAdd : AbstractCommand, ICommand
    {
        [DataMember] public string Arguments { get; set; }
        [DataMember] private string Representation;
        [DataMember] private IBuilder? builder;

        public CommandAdd()
        {
            Arguments = "";
            Representation = "";
        }

        public bool Preprocess()
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

        public bool PreprocessFromFile(StreamReader reader)
        {
            string[] tokens = Arguments.Split(' '); 

            Representation = tokens.Length == 2 ? tokens[1] : "base";

            if (!CheckRepresentation(Representation, true))
                return false;

            if (!GetBuilder(tokens[0], out builder, true))
                return false;

            return FillBuilder(ref builder, BuilderType.Create, true, reader);
        }

        public void Execute()
        {
            if (builder == null)
                return;

            Console.WriteLine("Added object:");
            Console.WriteLine(builder.BuildMethods[Representation]());
        }

        public override string ToString()
        {
            return $"add {Arguments} \n{builder?.ToString() ?? ""}";
        }
    }
}
