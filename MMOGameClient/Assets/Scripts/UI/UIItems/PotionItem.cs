using Assets.Scripts.SkillSystem.SkillSys;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.UI.UIItems
{
    [CreateAssetMenu(fileName = "New Potion", menuName = "Potion")]
    class PotionItem : UIItem
    {
        private void Awake()
        {
            CallOnAwake();
        }
        public override void CallOnAwake()
        {
            base.CallOnAwake();

            ItemType = UIItemType.Food;
            effects.Add(new Effect("Heal", EffectValue.RestoreHealth, 50, 0, 1));
            Name = "Small Healing Potion";

        }
    }
}
