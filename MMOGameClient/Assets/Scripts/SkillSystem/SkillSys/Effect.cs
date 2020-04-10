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
        public EffectValue EffectType;
        public int Value;
        public float Multiplier;
        public int MinSkillLevel;

        public Effect(EffectValue id, int value, int minLevel, float multiplier)
        {
            this.EffectType = id;
            this.Value = value;
            this.Multiplier = multiplier;
            this.MinSkillLevel = minLevel;
        }
        public float GetValue()
        {
            return Value * Multiplier;
        }
        public override string ToString()
        {
            return EffectType + ": " + Value;
        }
    }
}
