using Assets.Scripts.UI.UIItems;
using Assets.Scripts.UI_Window;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.UI
{
    [CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory")]
    [SerializeField]
    class UIInventory : ScriptableObject
    {
        [SerializeField]
        public List<KeyValuePair<int,  UIItem>> items = new List<KeyValuePair<int, UIItem>>();
    }
}
