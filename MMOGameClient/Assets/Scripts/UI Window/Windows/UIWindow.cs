
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.UI_Window
{
    class UIWindow : MonoBehaviour, IBeginDragHandler, IDragHandler
    {
        public Transform SlotContainer;
        Vector2 size;
        Vector3 shift;
        public void OnBeginDrag(PointerEventData eventData)
        {
            size = GetComponent<RectTransform>().sizeDelta;
            shift = Input.mousePosition - this.transform.position;
            this.transform.SetAsLastSibling();
        }
        public void OnDrag(PointerEventData eventData)
        {

            float X = Input.mousePosition.x - shift.x;
            float Y = Input.mousePosition.y - shift.y;
            if (X + size.x / 2 > Screen.width)
                X = Screen.width - size.x / 2;
            if (X - size.x / 2 < 0)
                X = size.x / 2;

            if (Y + size.y / 2 > Screen.height)
                Y = Screen.height - size.y / 2;
            if (Y - size.y / 2 < 0)
                Y = size.y / 2;
            this.transform.position = new Vector3(X, Y, 0);
        }
    }
}
