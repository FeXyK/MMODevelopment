using Assets.Scripts.UI;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.InventorySystem
{
    public class ItemLibrary
    {
        private static ItemLibrary instance;
        private static Dictionary<int, UIItem> items = new Dictionary<int, UIItem>();
        public static Dictionary<int, UIItem> Items()
        {
            if (instance == null)
            {
                instance = new ItemLibrary();
                foreach (var item in Resources.LoadAll<UIItem>("ItemObjects/Equippable"))
                {
                    items.Add(item.ID, item);
                }
                foreach (var item in Resources.LoadAll<UIItem>("ItemObjects/Potion"))
                {
                    items.Add(item.ID, item);
                }
                foreach (var item in Resources.LoadAll<UIItem>("ItemObjects/Food"))
                {
                    items.Add(item.ID, item);
                }
            }
            return items;
        }
    }
}
