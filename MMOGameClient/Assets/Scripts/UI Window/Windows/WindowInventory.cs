using Assets.Scripts.InventorySystem;
using Assets.Scripts.UI;
using Assets.Scripts.UI.UIItems;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.UI_Window
{
    public class WindowInventory : UIWindow
    {
        public UIInventory Inventory;

        public UIItem DefaultUIItem;
        public GameObject PrefabSlot;

        public int InventorySize;
        public void AddItem(UIItem item)
        {

        }
        public void RemoveItem(UIItem item)
        {

        }
        public void SortItems()
        {

        }
        private void Start()
        {
            DrawItems();
        }
        public void DrawItems()
        {
            for (int i = 0; i < InventorySize; i++)
            {
                Inventory.items.Add(new KeyValuePair<int, UIItem>(0, DefaultUIItem));
            }
            Upload();
            foreach (var item in Inventory.items)
            {
                GameObject obj = Instantiate(PrefabSlot);
                obj.transform.SetParent(SlotContainer);
                if (item.Value == null)
                    obj.GetComponent<WindowInventoryItem>().uiItem = DefaultUIItem;
                else
                {
                    obj.GetComponent<WindowInventoryItem>().uiItem = item.Value;
                        obj.GetComponent<WindowInventoryItem>().Amount = item.Key;
                }
                obj.GetComponent<WindowInventoryItem>().Refresh();
            }
        }

        private void Upload()
        {
            Player = GetPlayer();
            int i = 0;
            if (Player != null)
                foreach (var item in Player.inventory)
                {
                    Debug.Log(item.Key);

                    Inventory.items[i] = new KeyValuePair<int, UIItem>(item.Value[2], ItemLibrary.Items()[item.Key]);
                    i++;
                }
            Debug.Log("Inv size: " + Player.inventory.Count);
            //for (int i = 0; i < InventorySize; i++)
            //{
            //    if (UnityEngine.Random.Range(0, 10) > 2)
            //        if (UnityEngine.Random.Range(0, 10) > 5)
            //            Inventory.items[i] = (new KeyValuePair<int, UIItem>(UnityEngine.Random.Range(0, 100), Resources.Load<PotionItem>("ItemObjects/Potion/Tier1 Healing Potion")));
            //        else
            //            Inventory.items[i] = (new KeyValuePair<int, UIItem>(UnityEngine.Random.Range(0, 100), Resources.Load<PotionItem>("ItemObjects/Potion/Tier1 Mana Potion")));
            //}
            //Inventory.items.Add(new KeyValuePair<int, UIItem>(2, Resources.Load<PotionItem>("ItemObjects/Potion/Tier2 Healing Potion")));
            //Inventory.items.Add(new KeyValuePair<int, UIItem>(2, Resources.Load<PotionItem>("ItemObjects/Potion/Tier2 Mana Potion")));
            //Inventory.items.Add(new KeyValuePair<int, UIItem>(2, Resources.Load<PotionItem>("ItemObjects/Potion/Tier3 Healing Potion")));
            //Inventory.items.Add(new KeyValuePair<int, UIItem>(2, Resources.Load<PotionItem>("ItemObjects/Potion/Tier3 Mana Potion")));
            //Inventory.items.Add(new KeyValuePair<int, UIItem>(2, Resources.Load<PotionItem>("ItemObjects/Potion/Tier4 Healing Potion")));
            //Inventory.items.Add(new KeyValuePair<int, UIItem>(2, Resources.Load<PotionItem>("ItemObjects/Potion/Tier4 Mana Potion")));
            //Inventory.items.Add(new KeyValuePair<int, UIItem>(2, Resources.Load<PotionItem>("ItemObjects/Potion/Tier5 Healing Potion")));
            //Inventory.items.Add(new KeyValuePair<int, UIItem>(2, Resources.Load<PotionItem>("ItemObjects/Potion/Tier5 Mana Potion")));

        }
    }
}
