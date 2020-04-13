using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility_dotNET_Framework.Models;

namespace Assets.AreaServer.Entity
{
    public class CharacterItem
    {
        public int ID;
        public EItemType ItemType;
        public int Level;
        public int Durability;
        public int RequieredLevel;
        public int Amount;
        public Dictionary<int, Effect> effects = new Dictionary<int, Effect>();
        public CharacterItem(Utility_dotNET_Framework.Models.CharacterItem item)
        {
            ID = item.ID;
            Level = item.Level;
            Durability = item.Durability;
            Amount = item.Amount;
            RequieredLevel = ItemLibrary.Instance.Items[ID].RequiredLevel;
            ItemType = ItemLibrary.Instance.Items[ID].ItemType;

            foreach (var effect in ItemLibrary.Instance.Items[ID].Effects.Values)
            {
                Effect newEffect = new Effect();
                newEffect.EffectID = effect.EffectID;
                newEffect.Value = effect.Value;
                newEffect.Multiplier = effect.Multiplier;
                newEffect.MinLevel = effect.MinLevel;

                effects.Add(effect.EffectID, newEffect);
            }
        }

    }
}
