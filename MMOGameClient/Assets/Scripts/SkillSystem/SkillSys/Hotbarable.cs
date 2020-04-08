using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.SkillSystem.SkillSys
{
    class Hotbarable : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler

    {
        public void OnBeginDrag(PointerEventData eventData)
        {
            RectTransform obj = Instantiate(this.gameObject).GetComponent<RectTransform>();
            obj.transform.SetParent(this.transform);
            obj.position = this.transform.position;


        }

        public void OnDrag(PointerEventData eventData)
        {
            throw new NotImplementedException();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            throw new NotImplementedException();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            throw new NotImplementedException();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            throw new NotImplementedException();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            throw new NotImplementedException();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            throw new NotImplementedException();
        }
    }
}
