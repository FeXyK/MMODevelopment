using Assets.Scripts.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.UI_Window
{
    class WindowItem : MonoBehaviour, IDragHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public Color originColor;
        public GameObject origin;
        public Image image;
        public CanvasGroup canvasGroup;
        public UIItem uiItem;
        public Item Item;
        private void Awake()
        {
            image = this.GetComponent<Image>();
            canvasGroup = this.GetComponent<CanvasGroup>();
            originColor = image.color;
        }

        public virtual void CallOnEndDrag()
        {
            image.color = originColor;
            Destroy(origin);
        }
        public virtual bool CallOnBeginDrag()
        {
            if (uiItem.ID != 0)
            {
                origin = Instantiate(this.gameObject);

                origin.transform.SetParent(this.transform.parent);
                origin.transform.SetAsLastSibling();
                origin.GetComponent<CanvasGroup>().blocksRaycasts = false;
                return true;
            }
            else
                return false;
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            image.color = new Color(1, 1, 1, 0.5f);
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            image.color = originColor;
        }
        public virtual void Refresh() { }

        public void OnDrag(PointerEventData eventData)
        {
            origin.transform.position = Input.mousePosition;
            image.color = new Color(1, 1, 1, 0.1f);
        }
    }
}
