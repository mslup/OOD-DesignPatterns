using System.Security.Cryptography.X509Certificates;

namespace ProjOb
{
    public class RoomBuilder
    {
        private int Number;
        private IRoom.RoomTypeEnum RoomType;
        public readonly Dictionary<string, Action<string>> fieldSetterPairs;           

        public RoomBuilder() 
        {
            Number = 0;
            RoomType = IRoom.RoomTypeEnum.other;
            fieldSetterPairs = new Dictionary<string, Action<string>>
            {
                { "number", new Action<string>(SetNumber) },
                { "type", new Action<string>(SetRoomType) },
            };
        }

        public Room Build()
        {
            return new Room(Number, RoomType);
        }

        public void SetNumber(string value)
        {
            if (value == null)
                throw new ArgumentException();

            if (!int.TryParse(value, out Number))
                throw new ArgumentException();
        }

        public void SetRoomType(string value)
        {
            if (value == null)
                throw new ArgumentException();

            // unwanted behaviour when value > 4
            if (!Enum.TryParse(value, out RoomType))
                throw new ArgumentException();
        }
    }
}
