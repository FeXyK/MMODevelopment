using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.UI_Window
{
    class UIWindow : MonoBehaviour, IBeginDragHandler, IDragHandler, IPointerDownHandler
    {
        public Transform SlotContainer;
        public Vector2 size;
        Vector3 pivotShift;

        public virtual void CallOnStart()
        {
            size = GetComponent<RectTransform>().sizeDelta;
            pivotShift = Input.mousePosition - this.transform.position;
            this.transform.SetAsLastSibling();
        }
        public virtual void CallOnDrag()
        {
            float X = Input.mousePosition.x - pivotShift.x;
            float Y = Input.mousePosition.y - pivotShift.y;
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
        public void OnBeginDrag(PointerEventData eventData)
        {
            CallOnStart();
        }
        public void OnDrag(PointerEventData eventData)
        {
            CallOnDrag();
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            SetFormAsLastSibling();
        }
        public void SetFormAsLastSibling()
        {
            this.transform.SetAsLastSibling();
        }
    }
}
