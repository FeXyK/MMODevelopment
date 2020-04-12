using UnityEngine;

namespace Assets.Scripts.UI.UIItems
{
    [CreateAssetMenu(fileName = "New Skill", menuName = "Skill")]
    public class SkillItem : UIItem
    {
        public UISkillType SkilType;
        [SerializeField]
        public int ManaCost;
        [SerializeField]
        public float ManaCostMultiplier;
        [SerializeField]
        public int Range;
        [SerializeField]
        public float RangeMultiplier;
        [SerializeField]
        public int GoldCost;
        [SerializeField]
        public float GoldCostMultiplier;

        public float GetManaCost()
        {
            return ManaCost * (Mathf.Pow(ManaCostMultiplier, Level - 1));
        }
        public float GetGoldCost()
        {
            return GoldCost * (Mathf.Pow(GoldCostMultiplier, Level - 1));
        }
        public float GetRange()
        {
            return Range * (Mathf.Pow(RangeMultiplier, Level - 1));
        }
    }
}
