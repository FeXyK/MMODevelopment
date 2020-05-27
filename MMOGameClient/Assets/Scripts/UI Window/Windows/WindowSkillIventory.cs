using Assets.Scripts.Character;
using Assets.Scripts.UI;
using System;
using UnityEngine;

namespace Assets.Scripts.UI_Window
{
    public class WindowSkillIventory : UIWindow
    {
        public GameObject PrefabSlot;
        public UISkillList SkillList;

        private void Start()
        {
            Refresh();
        }
        public override void Refresh()
        {
            foreach (Transform child in SlotContainer.transform)
            {
                GameObject.Destroy(child.gameObject);
            }

            Player = GetPlayer();
            if (Player != null)
                foreach (var skill in Player.skills)
                {
                    foreach (var uiskill in SkillList.items)
                    {
                        if (uiskill.ID == skill.Key)
                        {
                            uiskill.Level = skill.Value;
                            break;
                        }
                    }
                }
            foreach (var item in SkillList.items)
            {
                GameObject obj = Instantiate(PrefabSlot);
                obj.transform.SetParent(SlotContainer);
                obj.GetComponent<WindowSkillItem>().Container.Item = item;
                obj.GetComponent<WindowSkillItem>().Refresh();
            }
        }
    }
}
