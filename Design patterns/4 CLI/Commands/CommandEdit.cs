using System.Collections;
using System.Runtime.InteropServices.ObjectiveC;
using System.Runtime.Serialization;

namespace ProjOb
{
    [DataContract, KnownType(typeof(CommandEdit)),
        KnownType(typeof(AbstractCommand))]
    public class CommandEdit : AbstractCommand
    {
        [DataMember] override public string Arguments { get; set; }
        [DataMember] private string objectType;
        [DataMember] private AbstractBuilder? builder;
        [DataMember] private AbstractBuilder? memento;
        [DataMember] private List<Requirement>? predicates;
        private IFilterable? found;
        private IDictionary? iteratedObjects;
        public CommandEdit()
        {
            objectType = "";
            Arguments = "";
        }

        public override bool Preprocess()
        {
            string[] tokens = Arguments.Split(' ', 2);

            objectType = tokens[0];

            if (!ParseRequirements(tokens.Length == 2 ? tokens[1] : "",
                objectType, out predicates))
                return false;

            if (!GetBuilder(tokens[0], out builder))
                return false;

            return FillBuilder(ref builder, BuilderType.Edit);
        }

        public override bool PreprocessFromFile(StreamReader reader)
        {
            string[] tokens = Arguments.Split(' ', 2);

            objectType = tokens[0];

            if (!ParseRequirements(tokens.Length == 2 ? tokens[1] : "",
                objectType, out predicates, true))
                return false;

            if (!GetBuilder(tokens[0], out builder, true))
                return false;

            return FillBuilder(ref builder, BuilderType.Edit);
        }


        public override void Execute()
        {
            if (builder == null)
                return;

            if (!FindCollection(objectType, out iteratedObjects))
                return;

            var foundVector = RunPredicates(iteratedObjects, predicates, true);

            if (foundVector == null)
                return;

            if (foundVector.Count == 0)
            {
                Console.WriteLine("No objects matching requirements found.");
                return;
            }

            found = foundVector[0];

            FillMemento(builder, found, objectType, out memento);

            Console.WriteLine("Found object:");
            Console.WriteLine(found);
            builder.Update(found);
            Console.WriteLine("editted to:");
            Console.WriteLine(found);
        }

        public override void Undo()
        {
            Console.WriteLine("Object:");
            Console.WriteLine(found);
            memento.Update(found);
            Console.WriteLine("editted to:");
            Console.WriteLine(found);
        }

        public override void Redo()
        {
            Console.WriteLine("Object:");
            Console.WriteLine(found);
            builder.Update(found);
            Console.WriteLine("editted to:");
            Console.WriteLine(found);
        }

        public override string ToString()
        {
            return $"edit {Arguments} \n{builder?.ToString() ?? ""}";
        }
    }
}
