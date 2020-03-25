using Assets.Scripts.Handlers;
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
        public List<Image> levels = new List<Image>();

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

        public void SetLevel(SkillRank rank)
        {
            Debug.LogWarning("INCREASED TO: " + (int)rank);
            Rank = rank;
            switch (rank)
            {
                case SkillRank.NotAvailable:
                    ActivateLevels(0, 0);
                    break;
                case SkillRank.Available:
                    ActivateLevels(1, 1);
                    break;
                case SkillRank.Apprentice:
                    ActivateLevels(2, 2);
                    break;
                case SkillRank.Advanced:
                    ActivateLevels(2, 3);
                    break;
                case SkillRank.Master:
                    ActivateLevels(2, 4);
                    break;
            }
        }
        private void ActivateLevels(int r1, int r2)
        {
            for (int i = 0; i < levels.Count; i++)
            {
                if (i >= r1 && i <= r2)
                {
                    levels[i].gameObject.SetActive(true);
                }
                else
                {
                    levels[i].gameObject.SetActive(false);
                }
            }
        }
        public void OnPointerDown(PointerEventData data)
        {
            clicked++;
            if (clicked == 1) clicktime = Time.time;

            if (clicked > 1 && Time.time - clicktime < clickdelay)
            {
                clicked = 0;
                clicktime = 0;
                Debug.Log("Double CLick: " + this.GetComponent<RectTransform>().name);
                OnDoubleClick();
            }
            else if (clicked > 2 || Time.time - clicktime > 1) clicked = 0;
        }
        public void OnDoubleClick()
        {
            if ((int)Rank < 4)
            {
                Rank = Rank + 1;
                SetLevel(Rank);
                skillItem.skill.IncreaseLevel((int)Rank);
                GameMessageSender.Instance.LevelUpSkill(skillItem.skill.SkillID, skillItem.skill.Level);
                Debug.Log(skillItem.skill.Level);
            }
            //GameMessageSender.Instance.LevelUpSkill(skillItem.skill.Level);
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