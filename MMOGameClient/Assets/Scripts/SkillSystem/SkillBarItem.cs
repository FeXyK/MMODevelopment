using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.SkillSystem
{
    public class SkillBarItem : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
    {
        float cooldown;
        public string hotkey = "1";
        public Image artImage;
        public Image cooldownArtImage;
        public TMP_Text cooldownText;
        public TMP_Text costText;
        public TMP_Text hotkeyText;

        public Image MouseOverImage;

        SkillBarController skillController;
        SkillItemDrag skillItem;
        public Tooltip Tooltip;
        internal void Drop(SkillItem value)
        {
            this.skillItem.skill = value;
            this.skillItem.name = value.name;
            RefreshUI();
        }
        private void Start()
        {
            RefreshUI();
        }
        private void Awake()
        {
            skillItem = this.GetComponent<SkillItemDrag>();
        }
        public void RefreshUI()
        {
            hotkeyText.text = hotkey;
            costText.text = skillItem.skill.Cost.ToString();
            cooldownText.text = skillItem.skill.CooldownTime.ToString();

            artImage.sprite = skillItem.skill.ArtSprite;
            cooldownArtImage.sprite = skillItem.skill.ArtSprite;
            cooldownArtImage.color = new Color(87 / 255, 87 / 255, 87 / 255, 87 / 255);
            Tooltip = FindObjectOfType<Tooltip>();
            Tooltip.Set(skillItem);
        }
        private void Update()
        {
            if (cooldown > 0)
            {
                cooldownArtImage.gameObject.SetActive(true);
                cooldown -= Time.deltaTime;
                cooldownText.text = Math.Round(cooldown, 1).ToString();
            }
            else
            {
                cooldownArtImage.gameObject.SetActive(false);
            }
        }
        public void OnDrop(PointerEventData eventData)
        {
            Drop(eventData.pointerDrag.GetComponent<SkillItemDrag>().skill);
            skillController = FindObjectOfType<SkillBarController>();
            skillController.Set(skillItem.skill, hotkey);
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            MouseOverImage.gameObject.SetActive(true);
            Tooltip.Set(skillItem);
            if (skillItem.skill.name != "_Empty")
                Tooltip.Show();
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            MouseOverImage.gameObject.SetActive(false);
            Tooltip.Hide();
        }

    }
}
