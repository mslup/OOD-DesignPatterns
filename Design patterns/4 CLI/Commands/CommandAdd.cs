using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjOb;

namespace ProjOb
{
    public class CommandAdd : CommandLogic, ICommand
    {
        public string Arguments { get; set; }
        private string Representation;
        public CommandAdd()
        {
            Arguments = "";
            Representation = "";
        }

        private IBuilder? builder;
        public bool Preprocess()
        {
            string[] tokens = Arguments.Split(' ');
            if (tokens.Length > 2)
            {
                Console.WriteLine("Invalid arguments");
                return false;
            }

            Representation = tokens.Length == 2 ? tokens[1] : "base";
            if (!Dictionaries.Representations.Contains(Representation))
            {
                Console.WriteLine($"Unrecognized representation: {Representation}." +
                    $"Available represenations: {string.Join(", ", Dictionaries.Representations)}");
                return false;
            }

            if (!GetBuilder(tokens[0], out builder))
                return false;

            return FillBuilder(ref builder, BuilderType.Create);
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
            return "add " + Arguments;
        }
    }
}
