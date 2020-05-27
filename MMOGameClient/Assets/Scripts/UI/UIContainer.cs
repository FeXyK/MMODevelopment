using Assets.Scripts.UI.Enums;
using System;

namespace Assets.Scripts.UI.UIItems
{
    [Serializable]
    public class UIContainer
    {
        public int UniqueID;
        public int SlotID;
        public int Amount;
        public int Level;
        public int Durability;
        public ESlotType SlotType;
        public UIItem Item;

        public UIContainer()
        {

        }
        public UIContainer(UIContainer container)
        {
            UniqueID = container.UniqueID;
            SlotID = container.SlotID;
            Amount = container.Amount;
            Level = container.Level;
            Durability = container.Durability;
            SlotType = container.SlotType;
            Item = container.Item;
        }
        public UIContainer(int level, int amount, UIItem item)
        {
            this.Level = level;
            this.Amount = amount;
            this.Item = item;
        }
        public UIContainer(int slotID, int uniqueID, int level, int durability, int amount, int slotType, UIItem item)
        {
            SlotID = slotID;
            UniqueID = uniqueID;
            Level = level;
            Durability = durability;
            Amount = amount;
            SlotType = (ESlotType)slotType;
            Item = item;
        }
        public override string ToString()
        {
            return Amount + " V: " + Item.ID;
        }
    }
}
