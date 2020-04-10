using Assets.Scripts.UI;
using UnityEngine;

namespace Assets.Scripts.UI_Window
{
    class WindowSkillIventory : UIWindow
    {
        public GameObject PrefabSlot;
        public UISkillList SkillList;
        private void Start()
        {
            foreach (var item in SkillList.items)
            {
                GameObject obj = Instantiate(PrefabSlot);
                obj.transform.SetParent(SlotContainer);
                obj.GetComponent<WindowSkillItem>().uiItem = item;
                obj.GetComponent<WindowSkillItem>().Refresh();
            }
        }
    }
}
