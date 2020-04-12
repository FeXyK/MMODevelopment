using Assets.Scripts.SkillSystem.SkillSys;
using UnityEngine;

namespace Assets.Scripts.UI.UIItems
{
    public enum IWeaponType
    {
        OneHandedSword,
        TwoHandedSword,
        Staff,
        Bow,
        Shield,
        Dagger,
        Crossbow
    }
    [CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon")]
    public class WeaponItem : UIItem
    {
        public IWeaponType WeaponType;

        public GearPiece WeaponPiece;

        public override void CallOnAwake()
        {
            base.CallOnAwake();
            ID = 5001;
            WeaponType = IWeaponType.OneHandedSword;
            effects.Add(new Effect(EffectValue.AttackDamage, 10, 0, 1));
            Level = 1;
            MaxAmount = 1;
            Name = "Small Sword";
        }
    }
}
