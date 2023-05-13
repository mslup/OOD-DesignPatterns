using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ProjOb;
using static ProjOb.CommandLogic;

namespace ProjOb
{
    public class CommandEdit : CommandLogic, ICommand
    {
        public string Arguments { get; set; }
        private IBuilder? builder;
        private IDictionary? iteratedObjects;
        private List<Predicate>? predicates;
        private IFilterable? found;
        public CommandEdit() => Arguments = "";


        public bool Preprocess()
        {
            string[] tokens = Arguments.Split(' ', 2);
            string objectType = tokens[0];

            if (!FindCollection(objectType, out iteratedObjects))
                return false;

            if (!ParseRequirements(tokens[1], objectType, out predicates))
                return false;

            var foundVector = RunPredicates(iteratedObjects, predicates, true);
            if (foundVector == null)        
                return false;
            found = foundVector[0];

            if (!GetBuilder(tokens[0], out builder))
                return false;

            return FillBuilder(ref builder, BuilderType.Edit);
        }

        public void Execute()
        {
            if (builder == null)
                return;
            if (found == null)
                return;

            Console.WriteLine("Found object:");
            Console.WriteLine(found);
            builder.Update(found);
            Console.WriteLine("editted to:");
            Console.WriteLine(found);
        }

        public override string ToString()
        {
            return "edit " + Arguments;
        }
    }
}
