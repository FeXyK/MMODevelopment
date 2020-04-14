using System;
using UnityEngine;

namespace Assets.Scripts.UI.UIItems
{
    [Serializable]
    public class UIItemContainer
    {
        public int Amount;
        public int Level;
        public UIItem Item;


        public UIItemContainer(int Amount, int Level, UIItem Item)
        {
            this.Amount = Amount;
            this.Level = Level;
            this.Item = Item;
        }
        public override string ToString()
        {
            return Amount + " V: " + Item.ID;
        }
    }
}
