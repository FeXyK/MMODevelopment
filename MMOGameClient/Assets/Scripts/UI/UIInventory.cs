using Assets.Scripts.UI.UIItems;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.UI
{
    [CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory")]
    [SerializeField]
    public class UIInventory : ScriptableObject
    {
        [SerializeField]
        public List<UIContainer> items = new List<UIContainer>();
    }
}
