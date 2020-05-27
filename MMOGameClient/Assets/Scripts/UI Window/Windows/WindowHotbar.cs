using Assets.Scripts.SkillSystem.SkillSys;
using Assets.Scripts.UI;
using Assets.Scripts.UI.UIItems;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.UI_Window
{
    public class WindowHotbar : UIWindow
    {
        public int HotbarSize;

        public UIHotbar Hotbar;
        public GameObject PrefabSlot;
        public List<WindowHotbarItem> hotbarSlots = new List<WindowHotbarItem>();
        private void Start()
        {
            DrawItems();
        }
        public void DrawItems()
        {
            Upload();
            foreach (var item in Hotbar.items)
            {
                GameObject obj = Instantiate(PrefabSlot);
                obj.transform.SetParent(SlotContainer);
                obj.GetComponent<WindowHotbarItem>().DefaultContainer = item;

                obj.GetComponent<WindowHotbarItem>().Container = item;
                obj.GetComponent<WindowHotbarItem>().HotkeyText = item.SlotID.ToString();
                obj.GetComponent<WindowHotbarItem>().Item.ItemHotkey.text = item.SlotID.ToString();

                hotbarSlots.Add(obj.GetComponent<WindowHotbarItem>());
                obj.GetComponent<WindowHotbarItem>().Refresh();
            }
        }
        internal void SetCooldown(int skillID)
        {
            foreach (var slot in hotbarSlots)
            {
                Debug.Log(slot.Container.Item.ID);
                if (slot.Container.Item.ID == skillID)
                {
                    slot.SetCooldown();
                }
            }
        }
        public void Modify(int hotkey, UIContainer container)
        {
            Hotbar.items[hotkey] = container;
        }
        public void Upload()
        {
            Hotbar.items.Clear();
            for (int i = 0; i < HotbarSize; i++)
            {
                Hotbar.items.Add(new UIContainer(DefaultContainer));
                Hotbar.items[i].SlotID = i;
            }
        }

    }
}
