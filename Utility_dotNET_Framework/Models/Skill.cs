namespace Utility.Models
{
    public class Skill : Item
    {
        public int SkillType { get; set; }
        public float Cooldown { get; set; }
        public int ManaCost { get; set; }
        public float ManaCostMultiplier { get; set; }
        public int GoldCost { get; set; }
        public float GoldCostMultiplier { get; set; }
        public float Range { get; set; }
        public float RangeMultiplier { get; set; }
        public int RequiredSkillID { get; internal set; }
    }
}
