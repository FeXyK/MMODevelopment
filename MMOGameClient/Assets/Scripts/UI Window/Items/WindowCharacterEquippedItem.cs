
using Assets.Scripts.UI.UIItems;
using UnityEngine.EventSystems;

namespace Assets.Scripts.UI_Window.Items
{
    public class WindowCharacterEquippedItem : WindowItem, IDropHandler
    {
        private void Start()
        {
            Container = DefaultContainer;
            Refresh();
        }
        public void OnDrop(PointerEventData eventData)
        {
            WindowItem draggedItem = eventData.pointerDrag.GetComponent<WindowItem>();
            if (draggedItem != null)
            {
                if ((draggedItem.DefaultContainer.Item as EquippableItem) != null)
                {
                    Container = draggedItem.Container;

                    if (draggedItem.Container.Item.ItemType == UI.EItemType.Armor)
                    {

                    }
                    if (draggedItem.Container.Item.ItemType == UI.EItemType.Weapon)
                    {

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
            UIManager.Instance.wCharacter.Unequip(Container.Item.ID, 30);
        }
    }
}
