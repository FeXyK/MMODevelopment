using System.Collections.Generic;
using UnityEngine;
using Utility.Models;

namespace Assets.AreaServer.InventorySystem
{
    public class SlotItem
    {
        public int ID;
        public int ItemID;
        public int SlotID;
        public int Amount;
        public int Level;
        public int Durability;
        public int MaxDurability;
        public int RequieredLevel;
        public EItemType ItemType;
        public ESlotType SlotType;

        public Dictionary<int, Effect> effects = new Dictionary<int, Effect>();
        public SlotItem(CharacterItem item)
        {
            ID = item.ID;
            ItemID = item.ItemID;
            SlotID = item.SlotID;
            SlotType = item.SlotType;
            Level = item.Level;
            Durability = item.Durability;
            MaxDurability = item.MaxDurability;
            Amount = item.Amount;
            RequieredLevel = ItemLibrary.Instance.Items[ItemID].RequiredLevel;
            ItemType = ItemLibrary.Instance.Items[ItemID].ItemType;

            foreach (var effect in ItemLibrary.Instance.Items[ItemID].Effects.Values)
            {
                Effect newEffect = new Effect();
                newEffect.EffectID = effect.EffectID;
                newEffect.Value = effect.Value;
                newEffect.Multiplier = effect.Multiplier;
                newEffect.MinLevel = effect.MinLevel;

                effects.Add(effect.EffectID, newEffect);
            }
        }
        public SlotItem(InventoryItem item)
        {
            ID = item.ID;
            ItemID = item.ItemID;
            SlotID = item.SlotID;
            RequieredLevel = item.RequieredLevel;
            ItemType = item.ItemType;
            foreach (var effect in item.Effects.Values)
            {
                Effect newEffect = new Effect();
                newEffect.EffectID = effect.EffectID;
                newEffect.Value = effect.Value;
                newEffect.Multiplier = effect.Multiplier;
                newEffect.MinLevel = effect.MinLevel;

                effects.Add(effect.EffectID, newEffect);
            }
        }

        internal int InventorySpace()
        {
            if (ItemType == EItemType.Potion || ItemType == EItemType.Food)
            {
                return Amount / 100;
            }
            return Amount / 1;
        }
    }
}
