namespace Utility_dotNET_Framework.Models
{
    public class Effect
    {
        public Effect(string name, int id, int value, int minLevel, float multiplier)
        {
            this.EffectID = id;
            this.Value = value;
            this.Multiplier = multiplier;
            this.MinLevel = minLevel;
            this.Name = name;
        }
        public Effect()
        {

        }
        public string Name { get;  set; }
        public int EffectID { get; set; }
        public int Value { get; set; }
        public int MinLevel { get;  set; }
        public float Multiplier { get; set; }
    }
}
