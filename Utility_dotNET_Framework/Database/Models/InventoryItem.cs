
namespace Utility_dotNET_Framework.Models
{
    public enum EItemType
    {
        Potion = 0,
        Food = 1,
        Weapon = 2,
        Head = 3,
        Arms = 4,
        Chest = 5,
        Legs = 6,
        Feet = 7,
        Necklace = 8,
        Ring = 9
    }
    public class InventoryItem : Item
    {
        public int Durability;
        public EItemType ItemType;
        public int RequiredLevel;
    }
}
