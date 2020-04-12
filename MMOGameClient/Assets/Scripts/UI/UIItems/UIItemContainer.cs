using System;
using UnityEngine;

namespace Assets.Scripts.UI.UIItems
{
    [Serializable]
    public class UIItemContainer
    {
        public int Key;
        public UIItem Value;
        public UIItemContainer(int Key, UIItem Value)
        {
            this.Key = Key;
            this.Value = Value;
        }
    }
}
