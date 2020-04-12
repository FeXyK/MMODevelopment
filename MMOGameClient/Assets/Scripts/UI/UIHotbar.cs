using Assets.Scripts.UI.UIItems;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.UI
{

    [CreateAssetMenu(menuName = "Hotbar", fileName = "New Hotbar")]
    public class UIHotbar : ScriptableObject
    {
        public List<UIItemContainer> items = new List<UIItemContainer>();
        public void Modify(int key, UIItem item)
        {
            items[key] = new UIItemContainer(key, item);
        }
    }
}
