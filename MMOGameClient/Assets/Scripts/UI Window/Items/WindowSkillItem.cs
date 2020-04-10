
using Assets.Scripts.UI;
using UnityEngine.EventSystems;

namespace Assets.Scripts.UI_Window
{
    class WindowSkillItem : WindowItem, IBeginDragHandler,IEndDragHandler
    {
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
        public override void Refresh()
        {
            Item.ItemImage.sprite = uiItem.GetSprite();
            switch ((UIItemRarity)uiItem.Level)
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
                    Item.ItemBorder.color = UIRarityColors.Scrap;
                    break;
            }
        }
    }
}