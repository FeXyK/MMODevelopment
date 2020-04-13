using System;

namespace Utility_dotNET_Framework.Models
{
    public class Effect
    {
        public Effect(int id, int value, int minLevel, float multiplier)
        {
            this.EffectID = id;
            this.Value = value;
            this.Multiplier = multiplier;
            this.MinLevel = minLevel;
        }
        public Effect()
        {

        }
        public float LeveledValue(int Level)
        {
            return Value * Pow(Multiplier, Level);
        }
        private float Pow(float value, int p)
        {
            float result = 1;
            for (int i = 0; i < p - 1; i++)
            {
                result *= value;
            }
            return result;
        }
        public int EffectID { get; set; }
        public int Value { get; set; }
        public int MinLevel { get; set; }
        public float Multiplier { get; set; }
    }
}
