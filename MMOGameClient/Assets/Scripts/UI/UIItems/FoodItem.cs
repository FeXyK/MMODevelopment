using Assets.Scripts.SkillSystem.SkillSys;
using UnityEngine;

namespace Assets.Scripts.UI.UIItems
{
    [CreateAssetMenu(fileName = "New Food", menuName = "Food")]
    class FoodItem : UIItem
    {
        public override void CallOnAwake()
        {
            base.CallOnAwake();
            ID = 3001;
            ItemType = UIItemType.Food;
            effects.Add(new Effect(EffectValue.RestoreHealth, 20, 0, 1));
            Name = "Bread";
        }
    }
}
