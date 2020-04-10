using Assets.Scripts.SkillSystem.SkillSys;
using Assets.Scripts.UI;
using Assets.Scripts.UI.UIItems;
using UnityEngine;

namespace Assets.Scripts.UI_Window
{
    class WindowHotbar : UIWindow
    {
        public int HotbarSize;

        public UIHotbar Hotbar;
        public GameObject PrefabSlot;
        public UIItem DefaultUIItem;

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
                if (item.Value == null)
                    obj.GetComponent<WindowHotbarItem>().uiItem = DefaultUIItem;
                else
                {
                    obj.GetComponent<WindowHotbarItem>().uiItem = item.Value;
                    obj.GetComponent<WindowHotbarItem>().ListNumber = item.Key;
                    obj.GetComponent<WindowHotbarItem>().Item.ItemHotkey.text = item.Key.ToString();
                    obj.GetComponent<WindowHotbarItem>().Hotkey = item.Key.ToString();
                    foreach (var effect in item.Value.effects)
                    {
                        if (effect.EffectType == EffectValue.Cooldown)
                            obj.GetComponent<WindowHotbarItem>().Item.ItemCooldown.text = effect.Value.ToString();
                    }
                    if ((item.Value as SkillItem) != null)
                    {
                        obj.GetComponent<WindowHotbarItem>().Item.ItemManaCost.text = (item.Value as SkillItem).ManaCost.ToString();
                    }
                }
                obj.GetComponent<WindowHotbarItem>().Refresh();
            }
        }
        public void Modify(int key, UIItem item)
        {
            Hotbar.Modify(key, item);
        }
        public void Upload()
        {
            if (Hotbar.items.Count == 0)
                for (int i = 0; i < HotbarSize; i++)
                {
                    Hotbar.items.Add(new UIItemContainer(i, DefaultUIItem));
                }
        }

    }
}
