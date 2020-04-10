﻿using Assets.Scripts.SkillSystem.SkillSys;
using Assets.Scripts.UI;
using Assets.Scripts.UI.UIItems;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.UI_Window
{
    class WindowHotbarItem : WindowItem, IDropHandler
    {
        public int Amount;
        public int ListNumber;
        public float CooldownTime;

        public UIItem DefaultUISlot;
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
        }
        public override void CallOnBeginDrag(PointerEventData eventData)
        {
            base.CallOnBeginDrag(eventData);
            if (clone != null)
            {
                clone.transform.SetParent(Hotbar.transform);
                clone.GetComponent<RectTransform>().sizeDelta = this.GetComponent<RectTransform>().sizeDelta;
            }
        }
        public override void CallOnEndDrag(PointerEventData eventData)
        {
            if (eventData.pointerDrag != null)
            {
                uiItem = DefaultUISlot;
                Hotbar.Modify(ListNumber, DefaultUISlot);
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

                uiItem = draggedItem.uiItem;
                Hotbar.Modify(ListNumber, uiItem);
                CooldownTime = uiItem.GetCooldown();

                if (draggedItem.uiItem.ItemType != UI.UIItemType.Skill)
                {
                    if ((draggedItem as WindowInventoryItem) != null)
                        Amount = (draggedItem as WindowInventoryItem).Amount;
                    else
                        Amount = (draggedItem as WindowHotbarItem).Amount;
                }
                if (eventData.pointerDrag == this.gameObject)
                {
                    eventData.pointerDrag = null;
                    CallOnEndDrag(eventData);
                }
                Refresh();
            }
        }
        public override void Refresh()
        {
            Item.ItemImage.sprite = uiItem.GetSprite();
            if (Item.ItemImage.sprite != null)
            {
                originColor = Color.white;
                Item.ItemImage.color = Color.white;
            }
            else
            {
                originColor = new Color(1f, 1f, 1f, 20f / 255f);
                Amount = 0;
                CooldownTime = 0;
                Item.ItemAmount.text = "";
                Item.ItemCooldown.text = "";
                Item.ItemManaCost.text = "";
                Item.ItemAmount.text = "";
            }
            foreach (var effect in uiItem.effects)
            {
                if (effect.EffectType == EffectValue.Cooldown)
                {
                    Item.ItemCooldown.text = effect.Value.ToString();
                }
            }
            if (uiItem.ItemType == UI.UIItemType.Skill)
            {
                Item.ItemManaCost.text = (uiItem as SkillItem).ManaCost.ToString();
            }
            else
            {
                Item.ItemManaCost.text = "";
            }
            if (Amount > 1)
            {
                Item.ItemAmount.text = Amount.ToString();
            }
        }
        public override void LoadTooltip(UIItem item)
        {
            base.LoadTooltip(item);
            
            tooltip.ManaCost.text = ((SkillItem)item).ManaCost.ToString();
            tooltip.Range.text = ((SkillItem)item).Range.ToString();
            tooltip.NextLevelCost.text = ((SkillItem)item).GoldCost.ToString();
        }
    }
}