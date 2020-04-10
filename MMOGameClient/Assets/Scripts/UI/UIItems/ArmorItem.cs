using Assets.Scripts.SkillSystem.SkillSys;
using UnityEngine;

namespace Assets.Scripts.UI.UIItems
{
    public enum ArmorPiece
    {
        Head,
        Arms,
        Chest,
        Legs,
        Feet,
    }
    [CreateAssetMenu(fileName = "New Armor", menuName = "Armor")]
    class ArmorItem : UIItem
    {
        public ArmorPiece Piece;
        private void Awake()
        {
        }
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