using System.Collections;

namespace ProjOb
{
    public class CommandList : CommandLogic, ICommand
    {
        public string Arguments { get; set; }

        public CommandList() => Arguments = "";

        private IDictionary? objectsToList;

        public bool Preprocess()
        {
            return FindCollection(Arguments, out objectsToList);
        }

        public void Execute()
        {
            PrintCollection(objectsToList);
        }

        public override string ToString()
        {
            return "list " + Arguments;
        }
    }
}
