using Assets.Scripts.UI;
using Assets.Scripts.UI.UIItems;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.UI_Window
{
    public class WindowTooltip : UIWindow
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
        internal void Load(UIContainer container)
        {
            float cd = container.Item.GetCooldown();
            string details = "" + container.Item.Details;
            for (int i = 0; i < container.Item.effects.Count; i++)
            {
                if (container.Item.effects[i].MinSkillLevel <= container.Item.Level)
                    Effects[i].text = container.Item.effects[i].ToString();
            }
            SetHeight(container.Item.effects.Count);
            Name.text = container.Item.ID + " " + container.Item.Name + " Lv." + container.Item.Level;
            Details.text = details.Length > 0 ? details : "Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Ut imperdiet massa quis orci tempus elementum.";
            Cooldown.text = cd != 0 ? "CD: " + cd.ToString() + " s" : "";
            Level.text = container.Item.Level > 0 ? "Lv." + container.Item.Level.ToString() : "";
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
