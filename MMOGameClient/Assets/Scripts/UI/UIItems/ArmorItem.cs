using Assets.Scripts.SkillSystem.SkillSys;
using UnityEngine;

namespace Assets.Scripts.UI.UIItems
{
  
    [CreateAssetMenu(fileName = "New Armor", menuName = "Armor")]
    class ArmorItem : EquippableItem
    {
        public override void CallOnAwake()
        {
            base.CallOnAwake();
            ID = 15001;
            effects.Add(new Effect(EffectValue.Armor, 0, 0, 1));
            Level = 1;
            MaxAmount = 1;
        }
    }
}