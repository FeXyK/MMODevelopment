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
            foreach (var slot in this.SlotContainer.GetComponentsInChildren<WindowCharacterEquippedItem>())
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

                    EquipAtomic(piece, item);

                    Inventory.items[slotID] = DefaultContainer;
                    UIManager.Instance.wInvertory.Refresh();

                    break;
                }
            }
        }
        private void EquipAtomic(EArmorPiece piece, UIContainer item)
        {
            EquippedContainers[piece] = item;
            EquippedObjects[piece].Container = item;
            EquippedObjects[piece].Refresh();
        }
        public void Unequip(int eSlotID, int iSlotID)
        {
            if (iSlotID > 0)
                foreach (var item in EquippedContainers)
                {
                    if (item.Value.SlotID == eSlotID)
                    {
                        UIContainer Temp = Inventory.items[iSlotID];
                        if (Inventory.items[iSlotID].Item.ID <= 0)
                        {
                            Inventory.items[iSlotID] = item.Value;
                            EquippedObjects[item.Key].SetDefault();
                            EquippedContainers.Remove(item.Key);
                        }
                        else
                        {
                            EquipAtomic(item.Key, Temp);
                        }

                        UIManager.Instance.wInvertory.Refresh();
                        break;
                    }
                }
        }
    }
}
