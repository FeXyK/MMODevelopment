using UnityEngine;

namespace Assets.Scripts.UI.UIItems
{
    [CreateAssetMenu(fileName = "New Skill", menuName = "Skill")]
    class SkillItem : UIItem
    {
        public UISkillType SkilType;
        [SerializeField]
        public int ManaCost;
        [SerializeField]
        public int Range;
        [SerializeField]
        public int GoldCost;
        //public override void CallOnAwake()
        //{
        //    base.CallOnAwake();
        //    ID = 0;
        //    MaxAmount = 1;
        //    ItemType = UIItemType.Skill;
        //}
    }
}
