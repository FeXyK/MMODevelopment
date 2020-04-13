using System.Collections.Generic;

namespace Utility_dotNET_Framework.Models
{
    public class Item
    {
        public int ID;
        public string Name;
        public Dictionary<int, Effect> Effects = new Dictionary<int, Effect>();
        public int Level;

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
