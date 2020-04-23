using Assets.Scripts.InventorySystem;
using Assets.Scripts.UI;
using Assets.Scripts.UI.UIItems;
using System;
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

        public int MaxInventorySize;
        public int CurrentInventorySize;

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
                if (item.Item.ID == newItem.Item.ID && item.Amount != item.Item.MaxAmount)
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
                        if (newItem.Item.MaxAmount >= newItem.Amount)
                        {
                            Inventory.items[i] = newItem;
                            break;
                        }
                        else
                        {
                            Inventory.items[i].Amount = newItem.Item.MaxAmount;
                            Inventory.items[i].Level = newItem.Level;
                            Inventory.items[i].Item = newItem.Item;
                            newItem.Amount -= newItem.Item.MaxAmount;
                            AddItem(newItem);
                        }
                    }
                }
            Refresh();
        }
        public void RemoveItem(int ID, int amount)
        {
            foreach (var item in Inventory.items)
            {
                if (item.Item.ID == ID)
                {
                    item.Amount--;
                    if (item.Amount == 0)
                    {
                        item.Item = DefaultUIItem;
                    }
                    break;
                }
            }
            Refresh();
        }
        public void SortItems()
        {

        }
        private void Start()
        {
            Initialize();
            Upload();
            Refresh();
            this.gameObject.SetActive(false);
        }
        public void Initialize()
        {
            Inventory.items.Clear();
            for (int i = 0; i < MaxInventorySize; i++)
            {
                Inventory.items.Add(new UIItemContainer(0, 0, DefaultUIItem));
            }
        }
        public void SaveState()
        {
            if (InventoryObjects.Count == Inventory.items.Count)
                for (int i = 0; i < MaxInventorySize; i++)
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
            CurrentInventorySize = 0;
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
                if (item.Item.ID > 0)
                    CurrentInventorySize++;

                InventoryObjects.Add(obj);
                obj.Refresh();
            }
        }
        private void Upload()
        {
            Player = GetPlayer();
            if (Player != null)
                foreach (var item in Player.inventory)
                {
                    AddItemToSlot(item.Key, new UIItemContainer(
                        item.Key,
                        item.Value[0],
                        item.Value[2],
                        item.Value[3],
                        item.Value[4],
                        item.Value[5],
                        ItemLibrary.Items()[item.Value[1]]));
                }
        }
        private void AddItemToSlot(int key, UIItemContainer container)
        {
            Inventory.items[key] = new UIItemContainer(container);
            Debug.Log(key);
            Debug.Log(Inventory.items[key].Item.ID);
        }
    }
}
