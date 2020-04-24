using Assets.Scripts.Handlers;
using Assets.Scripts.SkillSystem.SkillSys;
using Assets.Scripts.UI;
using Assets.Scripts.UI.UIItems;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.UI_Window
{
    public class WindowHotbarItem : WindowItem, IDropHandler
    {
        public int Hotkey;
        public string HotkeyText;
        public float CooldownTime;

        public WindowHotbar Hotbar;

        private void Update()
        {
            if (CooldownTime > 0)
            {
                CooldownTime -= Time.deltaTime;
                Item.ItemCooldown.text = CooldownTime.ToString("F1");
            }
            else
                Item.ItemCooldown.text = "";
            if (HotkeyText.Length > 0)
                if (Input.GetKeyDown(HotkeyText))
                {
                    Use();
                }
        }
        public override void CallOnBeginDrag(PointerEventData eventData)
        {
            base.CallOnBeginDrag(eventData);
            if (clone != null)
            {
                if (Hotbar == null)
                    Hotbar = FindObjectOfType<WindowHotbar>();
                clone.transform.SetParent(Hotbar.transform);
                clone.GetComponent<RectTransform>().sizeDelta = this.GetComponent<RectTransform>().sizeDelta;
            }
        }
        public override void CallOnEndDrag(PointerEventData eventData)
        {
            if (eventData.pointerDrag != null)
            {
                Container = DefaultContainer;
                Hotbar.Modify(Hotkey, DefaultContainer);
            }
            Refresh();
            base.CallOnEndDrag(eventData);
        }
        public void OnDrop(PointerEventData eventData)
        {
            WindowItem draggedItem = eventData.pointerDrag.GetComponent<WindowItem>();
            if (draggedItem != null)
            {
                if (Hotbar == null)
                    Hotbar = FindObjectOfType<WindowHotbar>();

                if ((draggedItem.Container.Item as EquippableItem) == null)
                {
                    Container = new UIContainer(draggedItem.Container);
                    Hotbar.Modify(Hotkey, Container);
                    CooldownTime = 0;
                }
                if (eventData.pointerDrag == this.gameObject)
                {
                    eventData.pointerDrag = null;
                    CallOnEndDrag(eventData);
                }
                Refresh();
            }
        }
        internal void SetCooldown()
        {
            Debug.Log(Container.Item.GetCooldown());
            CooldownTime = Container.Item.GetCooldown();
        }
        public override void Refresh()
        {
            Item.ItemImage.sprite = Container.Item.GetSprite();
            if (Item.ItemImage.sprite != null)
            {
                originColor = Color.white;
                Item.ItemImage.color = Color.white;
            }
            else
            {
                originColor = new Color(1f, 1f, 1f, 20f / 255f);
                Container.Amount = 0;
                CooldownTime = 0;
                Item.ItemAmount.text = "";
                Item.ItemCooldown.text = "";
                Item.ItemManaCost.text = "";
                Item.ItemAmount.text = "";

            }
            Item.ItemHotkey.text = HotkeyText.ToString();
            foreach (var effect in Container.Item.effects)
            {
                if (effect.EffectType == EffectValue.Cooldown)
                {
                    Item.ItemCooldown.text = effect.Value.ToString();
                }
            }
            if (Container.Item.ItemType == UI.EItemType.Skill)
            {
                Item.ItemManaCost.text = (Container.Item as SkillItem).ManaCost.ToString();
            }
            else
            {
                Item.ItemManaCost.text = "";
            }
            if (Container.Amount > 1)
            {
                Item.ItemAmount.text = Container.Amount.ToString();
            }
        }
        public override void LoadTooltip(UIContainer container)
        {
            base.LoadTooltip(container);

            tooltip.ManaCost.text = ((SkillItem)Container.Item).ManaCost.ToString();
            tooltip.Range.text = ((SkillItem)Container.Item).Range.ToString();
            tooltip.NextLevelCost.text = ((SkillItem)Container.Item).GoldCost.ToString();
        }
        public override void Use()
        {
            if (Container.Item.ID >= 0)
            {
                switch (Container.Item.ItemType)
                {
                    case EItemType.Skill:
                        if (UIManager.Instance.ManaBar.value < (Container.Item as SkillItem).GetManaCost())
                            UIManager.Instance.SetFloatingNotification("Not enough mana");
                        else if (CooldownTime > 0)
                            UIManager.Instance.SetFloatingNotification("Skill on cooldown");
                        else
                            GameMessageSender.Instance.SendSkillCast(Container.Item as SkillItem);
                        break;
                    case EItemType.Potion:
                    case EItemType.Food:
                        Debug.Log(Container.SlotID);
                        base.Use();
                        break;
                }
            }
        }
    }
}