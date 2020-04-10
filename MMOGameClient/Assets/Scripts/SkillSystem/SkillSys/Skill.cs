using System.Collections.Generic;

namespace Assets.Scripts.SkillSystem.SkillSys
{
    public class Skill
    {
        //public SkillAsset SkillAsset;
        private float[] range = new float[3];
        private float[] levelingCost = new float[3];
        private float[] useCost = new float[3];

        private int[] requierdLevel = new int[3];

        public int level;

        public List<Effect> effects = new List<Effect>();

        public float Range { get => range[level]; }
        public float LevelingCost { get => levelingCost[level]; }
        public float UseCost { get => useCost[level]; }
        public int RequierdLevel { get => requierdLevel[level]; }

        public int RequiredSkillID { get; internal set; }
        public string Name { get; internal set; }
        public int ID { get; internal set; }
        public int SkillType { get; internal set; }

        public Skill()
        {
        }
        private void ArrayCopy<T>(T[] l, T[] r)
        {
            for (int i = 0; i < r.Length; i++)
            {
                l[i] = r[i];
            }
        }
        public Skill(Skill other)
        {
            ArrayCopy(this.range, other.range);
            ArrayCopy(this.levelingCost, other.levelingCost);
            ArrayCopy(this.useCost, other.useCost);
            ArrayCopy(this.requierdLevel, other.requierdLevel);
            this.level = other.level;
            this.ID = other.ID;
            this.SkillType = other.SkillType;
            this.Name = other.Name;

            for (int i = 0; i < other.effects.Count; i++)
            {
                this.effects.Add(other.effects[i]);
            }
        }
        public Effect GetEffect(int i)
        {
            if (effects.Count - 1 >= i)
                return effects[i];
            return null;
        }
        public void SetRange(float value, float multiplier)
        {
            for (int i = 0; i < 3; i++)
            {
                range[i] = value * multiplier * i;
            }
        }
        public void SetLevelingCost(float value, float multiplier)
        {

            for (int i = 0; i < 3; i++)
            {
                levelingCost[i] = value * multiplier * i;
            }
        }
        public void SetUseCost(float value, float multiplier)
        {

            for (int i = 0; i < 3; i++)
            {
                useCost[i] = value * multiplier * i;
            }
        }
        public void SetRequiredLevel(int l1, int l2, int l3)
        {
            requierdLevel[0] = l1;
            requierdLevel[1] = l2;
            requierdLevel[2] = l3;
        }
        public override string ToString()
        {
            string skillString = "";
            skillString += Name + " " + ID + " " + SkillType + " " + effects.Count;
            return skillString;
        }
    }
}
