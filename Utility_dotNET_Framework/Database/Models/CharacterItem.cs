

namespace Utility_dotNET_Framework.Models
{
    public class CharacterItem
    {
        public int ID;
        public int Durability;
        public int Amount;
        public int Level;

        public CharacterItem(int itemID, int durability, int amount, int level)
        {
            ID = itemID;
            Durability = durability;
            Amount = amount;
            Level = level;
        }
    }
}
