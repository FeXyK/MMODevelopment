using Assets.Scripts.SkillSystem.SkillSys;
using Assets.Scripts.UI;
using Assets.Scripts.UI.UIItems;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.UI_Window
{
    public class WindowHotbar : UIWindow
    {
        public int HotbarSize;

        public UIHotbar Hotbar;
        public GameObject PrefabSlot;
        public UIItem DefaultUIItem;
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
                if (item.Item == null)
                    obj.GetComponent<WindowHotbarItem>().uiItem = DefaultUIItem;
                else
                {
                    obj.GetComponent<WindowHotbarItem>().uiItem = item.Item;
                    obj.GetComponent<WindowHotbarItem>().ListNumber = item.Amount;
                    obj.GetComponent<WindowHotbarItem>().Item.ItemHotkey.text = item.Amount.ToString();
                    obj.GetComponent<WindowHotbarItem>().Hotkey = item.Amount.ToString();
                    hotbarSlots.Add(obj.GetComponent<WindowHotbarItem>());
                    foreach (var effect in item.Item.effects)
                    {
                        if (effect.EffectType == EffectValue.Cooldown)
                            obj.GetComponent<WindowHotbarItem>().Item.ItemCooldown.text = effect.Value.ToString();
                    }
                    if ((item.Item as SkillItem) != null)
                    {
                        obj.GetComponent<WindowHotbarItem>().Item.ItemManaCost.text = (item.Item as SkillItem).ManaCost.ToString();
                    }
                }
                obj.GetComponent<WindowHotbarItem>().Refresh();
            }
        }
        internal void SetCooldown(int skillID)
        {
            foreach (var slot in hotbarSlots)
            {
                if (slot.uiItem.ID == skillID)
                {
                    slot.SetCooldown();
                }
            }
        }
        public void Modify(int key, int amount, UIItem item)
        {
            Hotbar.Modify(key, amount, item);
        }
        public void Upload()
        {
            Hotbar.items.Clear();
            for (int i = 0; i < HotbarSize; i++)
                {
                    Hotbar.items.Add(new UIItemContainer(i, 0, DefaultUIItem));
                }
        }

    }
}
