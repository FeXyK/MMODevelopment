using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.UI
{
    [CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory")]
    class UIInventory : ScriptableObject
    {
        public List<UIItem> items = new List<UIItem>();
    }
}
