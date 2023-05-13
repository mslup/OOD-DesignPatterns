using System;
using System.Collections;
using System.Text.RegularExpressions;

namespace ProjOb
{
    public class CommandFind : CommandLogic, ICommand
    {
        public string Arguments { get; set; }
        public CommandFind()
        {
            Arguments = "";
            EmptyRequirements = true;
        }

        private Vector<IFilterable>? foundObjectsCollection;
        private IDictionary? iteratedObjects;
        private List<Predicate>? predicates;
        private bool EmptyRequirements;

        public bool Preprocess()
        {
            string[] tokens = Arguments.Split(' ', 2);
            string objectType = tokens[0];

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

        public void Execute()
        {
            if (iteratedObjects == null)
                return;

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
