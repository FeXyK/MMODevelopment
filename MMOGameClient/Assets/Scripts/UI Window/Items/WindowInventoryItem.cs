using Assets.Scripts.UI;
using Assets.Scripts.UI.UIItems;
using UnityEngine.EventSystems;

namespace Assets.Scripts.UI_Window
{
    class WindowInventoryItem : WindowItem, IDropHandler
    {
        public void OnDrop(PointerEventData eventData)
        {
            WindowInventoryItem draggedItem = eventData.pointerDrag.GetComponent<WindowInventoryItem>();
            if (draggedItem != null)
            {
                if (draggedItem.Container.Item.ID != 0 && draggedItem != this)
                {
                    if (Container.Item.ID == draggedItem.Container.Item.ID)
                    {
                        if (Container.Amount + draggedItem.Container.Amount <= Container.Item.MaxAmount)
                        {
                            Container.Amount += draggedItem.Container.Amount;
                            draggedItem.SetDefault();
                        }
                        else
                        {
                            draggedItem.Container.Amount -= Container.Item.MaxAmount - Container.Amount;
                            Container.Amount = Container.Item.MaxAmount;
                        }
                    }
                    else
                    {
                        UIContainer tempContainer = this.Container;

                        this.Container = draggedItem.Container;
                        draggedItem.Container = tempContainer;
                    }
                    draggedItem.Refresh();
                    Refresh();
                }
            }
        }
        public void SetDefault()
        {
            Container = DefaultContainer;
            Refresh();
        }
        public override void Refresh()
        {
            Item.ItemImage.sprite = Container.Item.GetSprite();
            if (Container.Amount > 1)
                Item.ItemAmount.text = Container.Amount.ToString();
            else
                Item.ItemAmount.text = "";
            switch (Container.Item.Rarity)
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
                    Item.ItemBorder.color = ERarityColors.Common;
                    break;
            }
        }
        public override void LoadTooltip(UIContainer container)
        {
            base.LoadTooltip(container);
            tooltip.ManaCost.text = "";
            tooltip.Range.text = "";
            tooltip.NextLevelCost.text = "Value: " + container.Item.Value.ToString();
        }
        public override void Use()
        {
            if (Container.Item.ItemType == EItemType.Potion)
                base.Use();
            else
                UIManager.Instance.wCharacter.Equip(Container.SlotID);
        }
    }
}
