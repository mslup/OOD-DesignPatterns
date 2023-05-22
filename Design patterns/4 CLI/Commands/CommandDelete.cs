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
    public class CommandDelete : AbstractCommand, ICommand
    {
        [DataMember] public string Arguments { get; set; }
        [DataMember] private string objectType;
        [DataMember] private List<Predicate>? predicates;
        private IDictionary? iteratedObjects;

        public CommandDelete()
        {
            objectType = "";
            Arguments = "";
        }

        public bool Preprocess()
        {
            string[] tokens = Arguments.Split(' ', 2);

            objectType = tokens[0];

            if (!ParseRequirements(tokens.Length == 2 ? tokens[1] : "",
                objectType, out predicates))
                return false;

            return true;
        }

        public bool PreprocessFromFile(StreamReader reader)
        {
            string[] tokens = Arguments.Split(' ', 2);

            objectType = tokens[0];

            if (!ParseRequirements(tokens.Length == 2 ? tokens[1] : "",
                objectType, out predicates, true))
                return false;

            return true;
        }

        public void Execute()
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

            Console.WriteLine("Deleted object:");
            Console.WriteLine(iteratedObjects[foundKey]);
            
            iteratedObjects.Remove(foundKey);
        }

        public override string ToString()
        {
            return $"delete {Arguments}";
        }
    }
}
