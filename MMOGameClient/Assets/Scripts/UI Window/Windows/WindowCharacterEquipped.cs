using Assets.Scripts.UI;
using Assets.Scripts.UI.UIItems;
using Assets.Scripts.UI_Window.Items;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.UI_Window
{
    public class WindowCharacterEquipped : UIWindow
    {
        public Dictionary<EArmorPiece, UIContainer> EquippedContainers = new Dictionary<EArmorPiece, UIContainer>();
        public Dictionary<EArmorPiece, WindowCharacterEquippedItem> EquippedObjects = new Dictionary<EArmorPiece, WindowCharacterEquippedItem>();

        public UIInventory Inventory;

        private void Start()
        {
            foreach (var slot in this.GetComponentsInChildren<WindowCharacterEquippedItem>())
            {
                EquippedObjects.Add(((EquippableItem)slot.GetComponent<WindowCharacterEquippedItem>().Container.Item).ArmorPiece, slot);
                EquippedContainers.Add(((EquippableItem)slot.GetComponent<WindowCharacterEquippedItem>().Container.Item).ArmorPiece, slot.GetComponent<WindowCharacterEquippedItem>().Container);
            }
        }
        public void Equip(int slotID)
        {
            foreach (var item in Inventory.items)
            {
                if (item.SlotID == slotID)
                {
                    EArmorPiece piece = ((EquippableItem)item.Item).ArmorPiece;

                    EquippedContainers[piece] = item;
                    EquippedObjects[piece].Container = item;
                    EquippedObjects[piece].Refresh();

                    Inventory.items[slotID] = DefaultContainer;
                    UIManager.Instance.wInvertory.Refresh();

                    break;
                }
            }
        }
        public void Unequip(int itemID, int inventorySlotID)
        {
            foreach (var item in EquippedContainers)
            {
                if (item.Value.Item.ID == itemID)
                {
                    Inventory.items[inventorySlotID] = item.Value;
                    EquippedObjects[item.Key].Refresh();
                    UIManager.Instance.wInvertory.Refresh();
                    break;
                }
            }
        }
    }
}
