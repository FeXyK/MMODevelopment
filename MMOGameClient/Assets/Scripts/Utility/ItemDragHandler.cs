using Assets.Scripts.SkillSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Utility
{
    class ItemDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        GameObject Clone;
        CanvasGroup canvasGroup;
        Transform MainGameUICanvas;
        private bool draggable;

        private void Awake()
        {
            MainGameUICanvas = GameObject.FindGameObjectWithTag("Game UI Base").transform;
            canvasGroup = GetComponent<CanvasGroup>();
        }
        public void OnBeginDrag(PointerEventData eventData)
        {
            SkillItemDrag draggedSkill;
            draggedSkill = eventData.pointerDrag.GetComponent<SkillItemDrag>();
            if (draggedSkill != null)
            {
                if (draggedSkill.skill.SkillID == -1)
                    eventData.pointerDrag = null;
                else
                {
                    Clone = Instantiate(this.gameObject);
                    Clone.transform.SetParent(this.transform.parent);
                    Clone.name = this.name;
                    Clone.GetComponent<RectTransform>().anchoredPosition = this.GetComponent<RectTransform>().anchoredPosition;
                    if (this.gameObject.tag == "SkillBar")
                    {
                        Clone.GetComponent<SkillItemDrag>().skill = Instantiate(Resources.Load<SkillItem>("SkillItems/_Empty"));
                    }
                    else
                        Clone.GetComponent<SkillItemDrag>().skill = (this.GetComponent<SkillItemDrag>().skill);
                    this.transform.SetParent(MainGameUICanvas);
                    canvasGroup.blocksRaycasts = false;
                    canvasGroup.ignoreParentGroups = true;
                    canvasGroup.interactable = false;
                    canvasGroup.alpha = 0.6f;
                }
            }
        }
        public void OnDrag(PointerEventData eventData)
        {
            transform.position = Input.mousePosition;
        }
        public void OnEndDrag(PointerEventData eventData)
        {
            canvasGroup.blocksRaycasts = false;
            canvasGroup.ignoreParentGroups = true;
            canvasGroup.interactable = false;

            //if (eventData.hovered!= null)
            //    foreach (var item in eventData.hovered)
            //    {
            //        Debug.Log("end: " + item.name);
            //    }

            Destroy(this.gameObject);
        }
    }
}
