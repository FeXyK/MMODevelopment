using Assets.Scripts.SkillSystem.SkillSys;
using UnityEngine;

namespace Assets.Scripts.UI.UIItems
{
    public enum GearPiece
    {
        Helmet,
        Gauntlets,
        Bracers,
        Chestplate,
        Pauldron,
        Cape,
        Leggings,
        Boots,
        Necklace,
        Ring,
        Earring,
        MainHand,
        OffHand,
    }
    [CreateAssetMenu(fileName = "New Armor", menuName = "Armor")]
    class ArmorItem : UIItem
    {
        public GearPiece ArmorPiece;
        public override void CallOnAwake()
        {
            base.CallOnAwake();
            ID = 15001;
            ItemType = UIItemType.Armor;
            effects.Add(new Effect(EffectValue.Armor, 20, 0, 1));
            Level = 1;
            MaxAmount = 1;
            Name = "Cloth";
        }
    }
}