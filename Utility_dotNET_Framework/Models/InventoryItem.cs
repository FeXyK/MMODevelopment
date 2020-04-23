
namespace Utility.Models
{
    public class InventoryItem : Item
    {
        public EItemType ItemType;
        public int RequiredLevel;
        public int MinDurability;
        public int MaxDurability;
        public int Amount;
        public int ItemID;
    }
}
