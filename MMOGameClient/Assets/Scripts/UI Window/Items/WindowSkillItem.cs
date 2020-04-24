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
            if (Container.Item.Level != 0)
            {
                base.CallOnBeginDrag(eventData);
            }
            else
                eventData.pointerDrag = null;
        }
        public override void Refresh()
        {
            Item.ItemImage.sprite = Container.Item.GetSprite();
            switch ((EItemRarity)Container.Item.Level)
            {
                case EItemRarity.Scrap:
                    Item.ItemBorder.color = ERarityColors.Scrap;
                    break;
                case EItemRarity.Common:
                    Item.ItemBorder.color = ERarityColors.Common;
                    break;
                case EItemRarity.Uncommon:
                    Item.ItemBorder.color = ERarityColors.Uncommon;
                    break;
                case EItemRarity.Rare:
                    Item.ItemBorder.color = ERarityColors.Rare;
                    break;
                case EItemRarity.Epic:
                    Item.ItemBorder.color = ERarityColors.Epic;
                    break;
                case EItemRarity.Legendary:
                    Item.ItemBorder.color = ERarityColors.Legendary;
                    break;
                default:
                    Item.ItemBorder.color = ERarityColors.Scrap;
                    break;
            }
        }
        public override void Use()
        {
            GameMessageSender.Instance.SkillLevelUp(Container.Item);
        }
        public override void LoadTooltip(UIContainer container)
        {
            base.LoadTooltip(container);

            tooltip.ManaCost.text = "Mana: " + ((SkillItem)container.Item).ManaCost.ToString();
            tooltip.Range.text = "Range: " + ((SkillItem)container.Item).Range.ToString();
            tooltip.NextLevelCost.text = "Gold: " + ((SkillItem)container.Item).GoldCost.ToString();
        }
    }
}