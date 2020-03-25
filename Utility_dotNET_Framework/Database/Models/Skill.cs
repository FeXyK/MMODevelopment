using System.Collections.Generic;

namespace Utility_dotNET_Framework.Models
{
    public class Skill
    {
        public string Name { get; set; }
        public int Level { get; set; }
        public int SkillID { get; set; }
        public float Cooldown { get; set; }

        public Dictionary<int, Effect> Effects = new Dictionary<int, Effect>();
        public override string ToString()
        {
            string msg = "";
            msg += SkillID + " " + Name + " " + "\n";
            foreach (var effect in Effects)
            {
                msg += effect.Value.EffectID + ":  " + effect.Value.Value + "\n";
            }
            return msg;
        }
    }
}
