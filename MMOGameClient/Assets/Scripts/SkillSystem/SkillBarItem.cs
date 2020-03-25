using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.SkillSystem
{
    public class SkillBarItem : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public float cooldown = 10;
        public float cooldownDefault = 10;
        public string hotkey = "1";
        public Image artImage;
        public Image cooldownArtImage;
        public TMP_Text cooldownText;
        public TMP_Text costText;
        public TMP_Text hotkeyText;

        public Image MouseOverImage;

        SkillBarController skillController;
        public SkillItemDrag skillItem;
        public Tooltip Tooltip;
        internal void Drop(SkillItem value)
        {
            Debug.Log("Level after drop: "+value.Level);
            this.skillItem.skill = value;
            this.skillItem.name = value.name;
            Debug.Log("Level after drop2: "+this.skillItem.skill.Level);
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
            cooldownArtImage.color = new Color(0, 0, 0, 0.7f);
            cooldownArtImage.type = Image.Type.Filled;
            cooldownArtImage.fillMethod = Image.FillMethod.Radial360;

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
                cooldownArtImage.fillAmount = cooldown / cooldownDefault;
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
            skillController.Set(this, hotkey);
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
        public bool SetCooldown()
        {
            if (cooldown < 0)
            {
                cooldown = (skillItem.skill.CooldownTimeDefault / skillItem.skill.Level) - 1;
                cooldownDefault = cooldown;
                return true;
            }
            return false;
        }

    }
}
