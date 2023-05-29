using System;
using System.Collections;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace ProjOb
{
    [DataContract, KnownType(typeof(CommandFind)),
        KnownType(typeof(AbstractCommand))]
    public class CommandFind : AbstractCommand
    {
        [DataMember] override public string Arguments { get; set; }
        [DataMember] private List<Requirement>? predicates;
        [DataMember] private bool EmptyRequirements;
        [DataMember] private string objectType;
        private IDictionary? iteratedObjects;
        private Vector<IFilterable>? foundObjectsCollection;

        public CommandFind()
        {
            Arguments = "";
            objectType = "";
            EmptyRequirements = true;
        }

        public override bool Preprocess()
        {
            string[] tokens = Arguments.Split(' ', 2);
            objectType = tokens[0];

            if (!FindCollection(objectType, out iteratedObjects))
                return false;

            if (tokens.Length < 2)
            {
                EmptyRequirements = true;
                return true;
            }

            if (!ParseRequirements(tokens[1], objectType, out predicates))
                return false;

            EmptyRequirements = false;
            return true;
        }

        public override bool PreprocessFromFile(StreamReader reader)
        {
            string[] tokens = Arguments.Split(' ', 2);
            objectType = tokens[0];

            if (!FindCollection(objectType, out iteratedObjects, true))
                return false;

            if (tokens.Length < 2)
            {
                EmptyRequirements = true;
                return true;
            }

            if (!ParseRequirements(tokens[1], objectType, out predicates, true))
                return false;

            EmptyRequirements = false;
            return true;
        }

        public override void Execute()
        {
            if (iteratedObjects == null)
                FindCollection(objectType, out iteratedObjects);

            if (EmptyRequirements)
            {
                PrintCollection(iteratedObjects);
                return;
            }

            foundObjectsCollection = RunPredicates(iteratedObjects, predicates);

            if (foundObjectsCollection == null)
                return;

            PrintCollection(foundObjectsCollection);
        }


        public override string ToString()
        {
            return "find " + Arguments;
        }
    }
}
