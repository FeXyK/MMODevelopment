
using Assets.Scripts.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.UI_Window
{
    class WindowInventoryItem : WindowItem, IDropHandler, IBeginDragHandler, IEndDragHandler
    {
        public int Amount;
        public UIItem DefaultUISlot;

        public void OnDrop(PointerEventData eventData)
        {
            WindowInventoryItem draggedItem = eventData.pointerDrag.GetComponent<WindowInventoryItem>();
            if (draggedItem != null)
            {
                if (draggedItem.uiItem.ID != 0 && draggedItem != this)
                {
                    if (uiItem.ID == draggedItem.uiItem.ID)
                    {
                        if (Amount + draggedItem.Amount <= uiItem.MaxAmount)
                        {
                            Amount += draggedItem.Amount;
                            draggedItem.SetDefault();
                        }
                        else
                        {
                            draggedItem.Amount -= uiItem.MaxAmount - Amount;
                            Amount = uiItem.MaxAmount;
                        }
                    }
                    else
                    {
                        UIItem tempItem = this.uiItem;
                        int tempAmount = draggedItem.Amount;

                        uiItem = draggedItem.uiItem;
                        draggedItem.Amount = Amount;
                        Amount = tempAmount;
                        draggedItem.uiItem = tempItem;

                    }
                    draggedItem.Refresh();
                    Refresh();
                }
                else
                {
                    //TODO
                }
            }
        }
        public void SetDefault()
        {
            uiItem = DefaultUISlot;
            Amount = 0;
            Refresh();
        }
        public override void Refresh()
        {
            Item.ItemImage.sprite = uiItem.GetSprite();
            if (Amount != 0)
                Item.ItemAmount.text = Amount.ToString();
            else
                Item.ItemAmount.text = "";
            switch (uiItem.Rarity)
            {
                case UIItemRarity.Scrap:
                    Item.ItemBorder.color = UIRarityColors.Scrap;
                    break;
                case UIItemRarity.Common:
                    Item.ItemBorder.color = UIRarityColors.Common;
                    break;
                case UIItemRarity.Uncommon:
                    Item.ItemBorder.color = UIRarityColors.Uncommon;
                    break;
                case UIItemRarity.Rare:
                    Item.ItemBorder.color = UIRarityColors.Rare;
                    break;
                case UIItemRarity.Epic:
                    Item.ItemBorder.color = UIRarityColors.Epic;
                    break;
                case UIItemRarity.Legendary:
                    Item.ItemBorder.color = UIRarityColors.Legendary;
                    break;
                default:
                    Item.ItemBorder.color = UIRarityColors.Common;
                    break;
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!CallOnBeginDrag())
                eventData.pointerDrag = null;
        }
        public override void CallOnEndDrag()
        {
            base.CallOnEndDrag();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            CallOnEndDrag();
        }
    }
}
