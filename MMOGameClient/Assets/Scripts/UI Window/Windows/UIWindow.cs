using Assets.Scripts.Character;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.UI_Window
{
    public class UIWindow : MonoBehaviour, IBeginDragHandler, IDragHandler, IPointerDownHandler
    {
        public Transform SlotContainer;
        public Vector2 size;
        Vector3 pivotShift;

        WindowTooltip tooltip;

        public Entity Player;
        private bool dirtyFlag;

        public virtual void CallOnStart()
        {

            Player = GetPlayer();
            size = GetComponent<RectTransform>().sizeDelta;
            pivotShift = Input.mousePosition - this.transform.position;
            this.transform.SetAsLastSibling();
        }
        public Entity GetPlayer()
        {

            if (Player == null)
            {
                GameObject PlayerObject = GameObject.FindGameObjectWithTag("PlayerCharacter");
                if (PlayerObject != null)
                    return GameObject.FindGameObjectWithTag("PlayerCharacter").GetComponent<EntityContainer>().entity;
            }
            return Player;
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
        private void OnDisable()
        {
            if (tooltip == null)
            {
                tooltip = FindObjectOfType<WindowTooltip>();
            }
            if (tooltip != null)
                tooltip.Hide();
        }
        private void OnEnable()
        {
            if (dirtyFlag)
            {
                Refresh();
                dirtyFlag = false;
            }
        }

        public virtual void Refresh()
        {

        }

        internal virtual void Dirty()
        {
            dirtyFlag = true;
        }

    }
}
