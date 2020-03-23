using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.SkillSystem
{
    public class SkillTreeItem : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public string Name;
        public int SkillID;
        public int RequiredSkillPoint;
        public int RequiredLevel;
        public SkillRank Rank = SkillRank.NotAvailable;
        public List<SkillTreeItem> PreconditionOfSkill = new List<SkillTreeItem>();
        public Image Art;
        public Image Border;
        public Tooltip Tooltip;
        SkillItemDrag skillItem;
        void Start()
        {
            skillItem = GetComponent<SkillItemDrag>();
            SkillItem skillRef = Resources.Load<SkillItem>("SkillItems/" + Name);
            skillItem.skill = Instantiate(skillRef);
            skillItem.skill.name = skillRef.name;
            Tooltip = FindObjectOfType<Tooltip>();
            Tooltip.Set(skillItem);
        }

        float clicked = 0;
        float clicktime = 0;
        float clickdelay = 0.5f;

        public void OnPointerDown(PointerEventData data)
        {
            clicked++;
            if (clicked == 1) clicktime = Time.time;

            if (clicked > 1 && Time.time - clicktime < clickdelay)
            {
                clicked = 0;
                clicktime = 0;
                Debug.Log("Double CLick: " + this.GetComponent<RectTransform>().name);

            }
            else if (clicked > 2 || Time.time - clicktime > 1) clicked = 0;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Border.gameObject.SetActive(true);
            Tooltip.Set(skillItem);
            Tooltip.Show();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Border.gameObject.SetActive(false);
            Tooltip.Hide();
        }
    }
}