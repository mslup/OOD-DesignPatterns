using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ProjOb
{
    [DataContract, KnownType(typeof(CommandDelete)),
        KnownType(typeof(AbstractCommand))]
    public class CommandDelete : AbstractCommand
    {
        [DataMember] public override string Arguments { get; set; }
        [DataMember] private string objectType;
        [DataMember] private List<Requirement>? predicates;
        private IDictionary? iteratedObjects;
        private IFilterable deleted;

        public CommandDelete()
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

            return true;
        }

        public override bool PreprocessFromFile(StreamReader reader)
        {
            string[] tokens = Arguments.Split(' ', 2);

            objectType = tokens[0];

            if (!ParseRequirements(tokens.Length == 2 ? tokens[1] : "",
                objectType, out predicates, true))
                return false;

            return true;
        }

        public override void Execute()
        {
            if (!FindCollection(objectType, out iteratedObjects))
                return;

            
            string? foundKey = RunPredicatesGetKey
                (iteratedObjects, predicates, true);

            if (foundKey == null)
            { 
                Console.WriteLine("No objects matching requirements found.");
                return;
            }

            deleted = (IFilterable)iteratedObjects[foundKey];

            Console.WriteLine("Deleted object:");
            Console.WriteLine(deleted);

            //iteratedObjects.Remove(foundKey);
            Dictionaries.Remove(deleted);
        }

        public override void Undo()
        {
            Dictionaries.Add(deleted);
            Console.WriteLine("Added object: ");
            Console.WriteLine(deleted);
        }

        public override void Redo()
        {
            Dictionaries.Remove(deleted);
            Console.WriteLine("Deleted object: ");
            Console.WriteLine(deleted);
        }

        public override string ToString()
        {
            return $"delete {Arguments}";
        }
    }
}
