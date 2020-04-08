using Assets.Scripts.SkillSystem.SkillSys;
using UnityEngine;

namespace Assets.Scripts.UI.UIItems
{
    [CreateAssetMenu(fileName = "New Food", menuName = "Food")]
    class FoodItem : UIItem
    {
        private void Awake()
        {
            CallOnAwake();
        }
        public override void CallOnAwake()
        {
            base.CallOnAwake();

            ItemType = UIItemType.Food;
            effects.Add(new Effect("Heal", EffectValue.RestoreHealth, 20, 0, 1));
            Name = "Bread";
        }
    }
}
