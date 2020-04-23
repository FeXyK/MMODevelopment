using System;
using UnityEngine;

namespace Assets.Scripts.UI.UIItems
{
    [Serializable]
    public class UIItemContainer
    {
        public int UniqueID;
        public int SlotID;
        public int Amount;
        public int Level;
        public int Durability;
        public int SlotType;
        public UIItem Item;

        public UIItemContainer()
        {

        }

        public UIItemContainer(UIItemContainer container)
        {
            UniqueID = container.UniqueID;
            SlotID = container.SlotID;
            Amount = container.Amount;
            Level = container.Level;
            Durability = container.Durability;
            SlotType = container.SlotType;
            Item = container.Item;
        }

        public UIItemContainer(int level, int amount, UIItem item)
        {
            this.Level = level;
            this.Amount = amount;
            this.Item = item;
        }

        public UIItemContainer(int slotID, int uniqueID, int level, int durability, int amount, int slotType, UIItem item)
        {
            SlotID = slotID;
            UniqueID = uniqueID;
            Level = level;
            Durability = durability;
            Amount = amount;
            SlotType = slotType;
            Item = item;
        }
        public override string ToString()
        {
            return Amount + " V: " + Item.ID;
        }
    }
}
