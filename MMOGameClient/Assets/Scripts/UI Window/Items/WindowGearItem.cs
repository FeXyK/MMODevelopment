
using Assets.Scripts.UI;
using Assets.Scripts.UI.UIItems;
using UnityEngine.EventSystems;

namespace Assets.Scripts.UI_Window.Items
{
    class WindowGearItem : WindowItem, IDropHandler
    {
        public UIItem DefaultUISlot;
        private void Start()
        {
            uiItem = DefaultUISlot;
            Refresh();
        }
        public void OnDrop(PointerEventData eventData)
        {
            WindowItem draggedItem = eventData.pointerDrag.GetComponent<WindowItem>();
            if (draggedItem != null)
            {
                uiItem = draggedItem.uiItem;

                if (draggedItem.uiItem.ItemType == UI.UIItemType.Armor)
                {

                }
                if (draggedItem.uiItem.ItemType == UI.UIItemType.Weapon)
                {

                }
                Refresh();
            }
        }
        public override void Refresh()
        {
            Item.ItemImage.sprite = uiItem.GetSprite();
        }
    }
}
