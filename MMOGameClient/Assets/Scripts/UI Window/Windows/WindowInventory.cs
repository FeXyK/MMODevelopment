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
        List<WindowInventoryItem> InventoryObjects = new List<WindowInventoryItem>();
        public UIItem DefaultUIItem;
        public GameObject PrefabSlot;

        public int InventorySize;
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                AddItem(new UIItemContainer(1, 1, ItemLibrary.Items()[1001]));
            }
        }
        public void AddItem(UIItemContainer newItem)
        {
            SaveState();
            foreach (var item in Inventory.items)
            {
                if (item.Item.ID == newItem.Item.ID && item.Amount < item.Item.MaxAmount)
                {
                    if (item.Item.MaxAmount >= newItem.Amount + item.Amount)
                    {
                        item.Amount += newItem.Amount;
                    }
                    else
                    {
                        if (newItem.Amount > 0)
                        {
                            newItem.Amount = newItem.Amount - (item.Item.MaxAmount - item.Amount);
                            AddItem(newItem);
                            item.Amount = item.Item.MaxAmount;
                        }
                    }
                    newItem.Amount = 0;
                    break;
                }
            }
            if (newItem.Amount > 0)
                for (int i = 0; i < Inventory.items.Count; i++)
                {
                    if (Inventory.items[i].Item.ID == -1)
                    {
                        Inventory.items[i] = newItem;
                        break;
                    }
                }
            Refresh();
        }
        public void RemoveItem(UIItem item)
        {

        }
        public void SortItems()
        {

        }
        private void Start()
        {
            Initialize();
            Upload();
            Refresh();
        }
        public void Initialize()
        {
            Inventory.items.Clear();
            for (int i = 0; i < InventorySize; i++)
            {
                Inventory.items.Add(new UIItemContainer(0,0, DefaultUIItem));
            }
        }
        public void SaveState()
        {
            if (InventoryObjects.Count == Inventory.items.Count)
                for (int i = 0; i < InventorySize; i++)
                {
                    Inventory.items[i].Amount = InventoryObjects[i].Amount;
                    Inventory.items[i].Item = InventoryObjects[i].uiItem;
                }
        }
        public override void Refresh()
        {

            foreach (Transform child in SlotContainer.transform)
            {
                Destroy(child.gameObject);
            }
            InventoryObjects.Clear();
            foreach (var item in Inventory.items)
            {
                WindowInventoryItem obj = Instantiate(PrefabSlot).GetComponent<WindowInventoryItem>();
                obj.transform.SetParent(SlotContainer);

                if (item.Item == null)
                    obj.uiItem = DefaultUIItem;
                else
                {
                    obj.uiItem = item.Item;
                    obj.Amount = item.Amount;
                }
                InventoryObjects.Add(obj);
                obj.Refresh();
            }
        }
        private void Upload()
        {
            Player = GetPlayer();
            int i = 0;
            if (Player != null)
                foreach (var item in Player.inventory)
                {
                    Inventory.items[i] = new UIItemContainer(item.Value[2], item.Value[0], ItemLibrary.Items()[item.Key]);
                    i++;
                }
            Debug.Log("Inv size: " + Player.inventory.Count);
        }
    }
}
