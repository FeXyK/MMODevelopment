namespace Utility.Models
{
    public class CharacterItem
    {
        public int ID;
        public int ItemID;
        public int SlotID;
        public ESlotType SlotType;
        public int Amount;
        public int Level;
        public int Durability;
        public int MaxDurability;
        public CharacterItem()
        {

        }
        public CharacterItem(int iD, int itemID, int slotID, ESlotType slotType, int amount, int level, int durability, int maxDurability)
        {
            ID = iD;
            ItemID = itemID;
            SlotID = slotID;
            SlotType = slotType;
            Amount = amount;
            Level = level;
            Durability = durability;
            MaxDurability = maxDurability;
        }
        public CharacterItem(int itemID, int durability, int amount, int level)
        {
            ID = itemID;
            Durability = durability;
            Amount = amount;
            Level = level;
        }
    }
}
