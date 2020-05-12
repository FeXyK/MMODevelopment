using Assets.Scripts.SkillSystem.SkillSys;
using UnityEngine;

namespace Assets.Scripts.UI.UIItems
{
    [CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon")]
    public class WeaponItem : EquippableItem
    {
        public EWeaponType WeaponType;

        public override void CallOnAwake()
        {
            base.CallOnAwake();
            ArmorPiece = EArmorPiece.MainHand;
            ID = 5001;
            effects.Add(new Effect(EffectValue.AttackDamage, 10, 0, 1));
            Level = 1;
            MaxAmount = 1;
        }
    }
}
