using Assets.Scripts.UI;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.UI_Window
{
    class WindowTooltip : UIWindow
    {
        public TMP_Text Name;
        public TMP_Text Details;
        public List<TMP_Text> Effects = new List<TMP_Text>();
        public TMP_Text Range;
        public TMP_Text ManaCost;
        public TMP_Text Cooldown;
        public TMP_Text Level;
        public TMP_Text NextLevelCost;
        private void Start()
        {
            CallOnStart();
            Hide();
        }
        private void Update()
        {
            CallOnDrag();
        }
        public void Show()
        {
            SlotContainer.gameObject.SetActive(true);
            transform.SetAsLastSibling();
        }
        public void Hide()
        {
            SlotContainer.gameObject.SetActive(false);
        }
        internal void Clear()
        {
            foreach (var effect in Effects)
            {
                effect.text = "";
            }
        }
        public void SetHeight(int count)
        {
            CallOnStart();
            float y = 250;
            y += count * 25;
            size = new Vector2(size.x, y);
            this.GetComponent<RectTransform>().sizeDelta = new Vector2(size.x, y);

        }
        internal void Load(UIItem item)
        {
            float cd = item.GetCooldown();
            string details = "" + item.Details;
            for (int i = 0; i < item.effects.Count; i++)
            {
                if (item.effects[i].MinSkillLevel >= item.Level)
                    Effects[i].text = item.effects[i].ToString();
            }
            SetHeight(item.effects.Count);
            Name.text = item.ID + " " + item.Name + " Lv." + item.Level;
            Details.text = details.Length > 0 ? details : "Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Ut imperdiet massa quis orci tempus elementum.";
            Cooldown.text = cd != 0 ? "Cooldown: " + cd.ToString() + " s" : "";
            Level.text = item.Level > 0 ? "Lv." + item.Level.ToString() : "";
        }
        public override void CallOnStart()
        {
            size = GetComponent<RectTransform>().sizeDelta;
            this.GetComponent<RectTransform>().pivot = Vector2.zero;
            base.CallOnStart();
        }
        public override void CallOnDrag()
        {
            float X = Input.mousePosition.x;
            float Y = Input.mousePosition.y;
            if (X + size.x > Screen.width)
                X = Screen.width - size.x;
            if (X < 0)
                X = 0;

            if (Y + size.y > Screen.height)
                Y = Screen.height - size.y;
            if (Y < 0)
                Y = 0;
            this.transform.position = new Vector3(X, Y, 0);
        }
    }
}
