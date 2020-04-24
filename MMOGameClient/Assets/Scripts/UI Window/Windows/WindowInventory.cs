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
        public GameObject PrefabSlot;

        public int MaxInventorySize;
        public int CurrentInventorySize;

        public void AddItem(UIContainer newItem)
        {
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
        public void RemoveItem(int slotID, int amount)
        {
            Inventory.items[slotID].Amount--;
            if (Inventory.items[slotID].Amount == 0)
            {
                Inventory.items[slotID] = DefaultContainer;
            }
            Refresh();
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
                InventoryObjects.Add(Instantiate(PrefabSlot, SlotContainer).GetComponent<WindowInventoryItem>());
                Inventory.items.Add(DefaultContainer);
            }
        }
        public override void Refresh()
        {
            for (int i = 0; i < MaxInventorySize; i++)
            {
                InventoryObjects[i].Container = Inventory.items[i];
                InventoryObjects[i].Refresh();
            }
        }
        private void Upload()
        {
            Player = GetPlayer();
            if (Player != null)
                foreach (var item in Player.inventory)
                {
                    AddItemToSlot(item.Key, new UIContainer(
                        item.Key,
                        item.Value[0],
                        item.Value[2],
                        item.Value[3],
                        item.Value[4],
                        item.Value[5],
                        ItemLibrary.Items()[item.Value[1]]));
                }
        }
        private void AddItemToSlot(int key, UIContainer container)
        {
            Inventory.items[key] = new UIContainer(container);
        }
    }
}
