using Assets.Scripts.SkillSystem.SkillSys;
using UnityEngine;

namespace Assets.Scripts.UI.UIItems
{
    [CreateAssetMenu(fileName = "New Potion", menuName = "Potion")]
    class PotionItem : UIItem
    {
        public override void CallOnAwake()
        {
            base.CallOnAwake();
            ID = 1001;
            ItemType = EItemType.Potion;
        }
    }
}
