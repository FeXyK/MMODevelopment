using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.SkillSystem.SkillSys
{
    [Serializable]
    public class Effect
    {
        public string Name;
        public EffectValue ID;
        public int Value;
        public float Multiplier;
        public int MinSkillLevel;
         
        public Effect(string name, EffectValue id, int value,int minLevel, float multiplier)
        {
            this.Name = name;
            this.ID = id;
            this.Value = value;
            this.Multiplier = multiplier;
            this.MinSkillLevel = minLevel;
        }
        public float GetValue()
        {
            return Value * Multiplier;
        }
    }
}
