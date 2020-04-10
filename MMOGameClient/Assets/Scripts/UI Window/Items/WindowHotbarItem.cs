using Assets.Scripts.SkillSystem.SkillSys;
using Assets.Scripts.UI;
using Assets.Scripts.UI.UIItems;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.UI_Window
{
    class WindowHotbarItem : WindowItem, IDropHandler, IBeginDragHandler,IEndDragHandler
    {
        public int Amount;
        public UIItem DefaultUISlot;

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!CallOnBeginDrag())
                eventData.pointerDrag = null;
        }
        public void OnEndDrag(PointerEventData eventData)
        {
            uiItem = DefaultUISlot;
            Refresh();
            CallOnEndDrag();
        }
        public override void CallOnEndDrag()
        {
            base.CallOnEndDrag();
        }
        public override bool CallOnBeginDrag()
        {
            if (uiItem.ID != 0)
            {
                origin = Instantiate(this.gameObject);
                origin.transform.SetParent(this.transform.parent.parent);
                origin.transform.SetAsLastSibling();
                origin.GetComponent<CanvasGroup>().blocksRaycasts = false;
                return true;
            }
            else
                return false;
        }
        public void OnDrop(PointerEventData eventData)
        {
            WindowItem draggedItem = eventData.pointerDrag.GetComponent<WindowItem>();
            if (draggedItem != null)
            {
                uiItem = draggedItem.uiItem;
                if (draggedItem.uiItem.ItemType != UI.UIItemType.Skill)
                {
                    if ((draggedItem as WindowInventoryItem) != null)
                        Amount = (draggedItem as WindowInventoryItem).Amount;
                    else
                        Amount = (draggedItem as WindowHotbarItem).Amount;
                }
                if (uiItem.ItemType == UI.UIItemType.Skill)
                {

                }
                else
                {
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
                originColor = new Color(1f,1f,1f,20f/255f);
                Amount = 0;
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
                Item.ItemAmount.text = Amount.ToString();
        }
    }
}