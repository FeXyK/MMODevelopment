﻿using Assets.Scripts.Handlers;
using Assets.Scripts.UI;
using Assets.Scripts.UI.UIItems;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.UI_Window
{
    public class WindowItem : MonoBehaviour, IDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IBeginDragHandler, IEndDragHandler
    {
        public Color originColor;
        public GameObject clone;
        public Image image;
        public CanvasGroup canvasGroup;
        public UIContainer Container;
        public UIContainer DefaultContainer;
        public Item Item;
        public float showTooltipTime;
        public float timer;
        bool showTooltip = false;

        float clicked = 0;
        float clicktime = 0;
        float clickdelay = 0.5f;

        public WindowTooltip tooltip;
        private void Awake()
        {
            tooltip = FindObjectOfType<WindowTooltip>();
            image = this.GetComponent<Image>();
            canvasGroup = this.GetComponent<CanvasGroup>();
            originColor = image.color;
            timer = showTooltipTime;
        }

        internal virtual void SetDefault()
        {
            Container = new UIContainer(DefaultContainer);
        }

        private void Update()
        {
            if (Container.Item != null)
                if (showTooltip && Container.Item.ID >= 0)
                {
                    timer -= Time.deltaTime;
                    if (timer < 0)
                        ShowTooltip();
                }
        }
        public void OnBeginDrag(PointerEventData eventData)
        {
            CallOnBeginDrag(eventData);
        }
        public void OnDrag(PointerEventData eventData)
        {
            clone.transform.position = Input.mousePosition;
            image.color = new Color(1, 1, 1, 0.1f);
        }
        public void OnEndDrag(PointerEventData eventData)
        {
            CallOnEndDrag(eventData);
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            image.color = new Color(1, 1, 1, 0.5f);

            timer = showTooltipTime;
            showTooltip = true;
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            HideTooltip();
            image.color = originColor;

            showTooltip = false;
            timer = showTooltipTime;
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            SetFormAsLastSibling();
            if (OnDoubleClick() || Input.GetMouseButtonDown(1))
            {
                Use();
            }
        }
        public bool OnDoubleClick()
        {
            clicked++;
            if (clicked == 1)
                clicktime = Time.time;

            if (clicked > 1 && Time.time - clicktime < clickdelay)
            {
                clicked = 0;
                clicktime = 0;
                return true;
            }
            else if (clicked > 2 || Time.time - clicktime > 1)
                clicked = 0;
            return false;
        }
        public virtual void LoadTooltip(UIContainer container)
        {
            tooltip.Clear();
            tooltip.Load(container);
        }
        public virtual void ShowTooltip()
        {
            if (tooltip != null)
            {
                tooltip.Show();
                LoadTooltip(Container);
            }
        }
        public virtual void HideTooltip()
        {
            if (tooltip != null)
                tooltip.Hide();
        }
        public virtual void CallOnEndDrag(PointerEventData eventData)
        {
            image.color = originColor;
            Destroy(clone);
        }
        public virtual void CallOnBeginDrag(PointerEventData eventData)
        {
            if (Container.Item.ID >= 0)
            {
                clone = Instantiate(this.gameObject);

                clone.transform.SetParent(this.transform.parent);
                clone.transform.SetAsLastSibling();
                clone.GetComponent<CanvasGroup>().blocksRaycasts = false;
            }
            else
                eventData.pointerDrag = null;
        }
        public virtual void Use()
        {
            GameMessageSender.Instance.SendUseMessage(Container);
        }
        public virtual void Refresh()
        {
        }
        public void SetFormAsLastSibling()
        {
            this.transform.parent.parent.SetAsLastSibling();
        }
    }
}
