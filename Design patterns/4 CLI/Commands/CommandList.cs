using System.Collections;
using System.Runtime.Serialization;

namespace ProjOb
{
    [DataContract, KnownType(typeof(CommandList))]
    public class CommandList : AbstractCommand, ICommand
    {
        [DataMember] public string Arguments { get; set; }
        private IDictionary? objectsToList;

        public CommandList() => Arguments = "";


        public bool Preprocess()
        {
            return FindCollection(Arguments, out objectsToList);
        }

        public void Execute()
        {
            if (objectsToList == null) 
                FindCollection(Arguments, out objectsToList);

            PrintCollection(objectsToList);
        }

        public override string ToString()
        {
            return "list " + Arguments;
        }
    }
}
