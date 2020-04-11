using Assets.Scripts.Handlers;
using Assets.Scripts.UI;
using Assets.Scripts.UI.UIItems;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.UI_Window
{
    class WindowSkillItem : WindowItem
    {

        public override void CallOnBeginDrag(PointerEventData eventData)
        {
            if (uiItem.Level != 0)
            {
                base.CallOnBeginDrag(eventData);
            }
            else
                eventData.pointerDrag = null;
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
        public override void Use()
        {
            GameMessageSender.Instance.SkillLevelUp(uiItem);
        }
        public override void LoadTooltip(UIItem item)
        {
            base.LoadTooltip(item);

            tooltip.ManaCost.text = "Mana: "+((SkillItem)item).ManaCost.ToString();
            tooltip.Range.text = "Range: " + ((SkillItem)item).Range.ToString();
            tooltip.NextLevelCost.text = "Gold: " + ((SkillItem)item).GoldCost.ToString();
        }
    }
}