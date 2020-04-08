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
            CallOnAwake();
        }
        public override void CallOnAwake()
        {
            base.CallOnAwake();

            ItemType = UIItemType.Armor;
            effects.Add(new Effect("Armor", EffectValue.Armor, 20, 0, 1));
            Level = 1;
            Name = "Cloth";
        }
    }
}