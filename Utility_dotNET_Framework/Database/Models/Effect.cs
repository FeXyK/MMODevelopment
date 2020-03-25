namespace Utility_dotNET_Framework.Models
{
    public class Effect
    {
        public Effect(int id, int value)
        {
            this.EffectID= id;
            this.Value = value;
        }
        public Effect()
        {

        }
        public int EffectID { get; set; }
        public int Value { get; set; }
        public string Name { get; internal set; }
    }
}
