
using Assets.Scripts.Handlers;
using Assets.Scripts.UI.UIItems;
using UnityEngine.EventSystems;

namespace Assets.Scripts.UI_Window.Items
{
    public class WindowCharacterEquippedItem : WindowItem, IDropHandler
    {
        private void Awake()
        {
            SetDefault();
        }
        public void OnDrop(PointerEventData eventData)
        {
            WindowItem draggedItem = eventData.pointerDrag.GetComponent<WindowItem>();
            if (draggedItem != null)
            {
                if ((draggedItem.Container.Item as EquippableItem) != null)
                {
                    EArmorPiece piece = (draggedItem.Container.Item as EquippableItem).ArmorPiece;
                    if (piece == (Container.Item as EquippableItem).ArmorPiece)
                    {
                        GameMessageSender.Instance.Equip(draggedItem.Container);
                    }
                    Refresh();
                }
            }
        }
        public override void Refresh()
        {
            Item.ItemImage.sprite = Container.Item.GetSprite();
        }
        public override void Use()
        {
            GameMessageSender.Instance.Unequip(Container);
        }
        internal override void SetDefault()
        {
            Container = DefaultContainer;
            Refresh();
        }
    }
}
