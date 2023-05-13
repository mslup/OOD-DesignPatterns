using System.Collections;
using System.Runtime.InteropServices.ObjectiveC;
using System.Runtime.Serialization;

namespace ProjOb
{
    [DataContract, KnownType(typeof(CommandEdit))]
    public class CommandEdit : AbstractCommand, ICommand
    {
        [DataMember] public string Arguments { get; set; }
        [DataMember] private string objectType;
        [DataMember] private IBuilder? builder;
        [DataMember] private List<Predicate>? predicates;
        [DataMember] private IFilterable? found;
        private IDictionary? iteratedObjects;
        public CommandEdit()
        {
            objectType = "";
            Arguments = "";
        }

        public bool Preprocess()
        {
            string[] tokens = Arguments.Split(' ', 2);

            objectType = tokens[0];

            if (!FindCollection(objectType, out iteratedObjects))
                return false;

            if (!ParseRequirements(tokens.Length == 2 ? tokens[1] : "",
                objectType, out predicates))
                return false;

            var foundVector = RunPredicates(iteratedObjects, predicates, true);

            if (foundVector == null)
                return false;

            if (foundVector.Count == 0)
            {
                Console.WriteLine("No objects matching requirements found.");
                return false;
            }

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
            {
                if (iteratedObjects == null)
                {
                    if (!FindCollection(objectType, out iteratedObjects))
                        return;
                }
                var foundVector = RunPredicates(iteratedObjects, predicates, true);
                if (foundVector == null)
                    return;
                found = foundVector[0];
            }
            

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
