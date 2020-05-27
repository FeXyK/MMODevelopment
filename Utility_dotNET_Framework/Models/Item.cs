using System.Collections.Generic;

namespace Utility.Models
{
    public class Item
    {
        public string Name;
        public int ID;
        public int SlotID;
        public ESlotType SlotType;
        public int Level;
        public int RequieredLevel;

        public Dictionary<int, Effect> Effects = new Dictionary<int, Effect>();

        public override string ToString()
        {
            string msg = "";
            msg += ID + " " + Name + " " + "\n";
            foreach (var effect in Effects)
            {
                msg += effect.Value.EffectID + ":  " + effect.Value.Value + "\n";
            }
            return msg;
        }
    }
}
