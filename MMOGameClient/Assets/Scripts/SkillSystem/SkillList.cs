using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.SkillSystem
{
    public class SkillList
    {
        public  Dictionary<int, GameObject> skill = new Dictionary<int, GameObject>();
        public List<Sprite> skillLevels = new List<Sprite>();
        //public List<SkillItem> skillBarItems = new List<SkillItem>();

        private static SkillList instance;
        public static SkillList Instance
        {
            get
            {
                if (instance == null)
                    instance = new SkillList();
                return instance;
            }
        }

        public SkillList()
        {
            skill.Add(0, Resources.Load<GameObject>("VFX/Fireshock"));
            skill.Add(1, Resources.Load<GameObject>("VFX/Fireball"));
            skill.Add(2, Resources.Load<GameObject>("VFX/Fireshock"));
            skill.Add(3, Resources.Load<GameObject>("VFX/Fireshock"));
            skill.Add(4, Resources.Load<GameObject>("VFX/Fireshock"));
            skill.Add(5, Resources.Load<GameObject>("VFX/Fireshock"));
            skill.Add(6, Resources.Load<GameObject>("VFX/Fireshock"));
            skill.Add(7, Resources.Load<GameObject>("VFX/Fireshock"));
            skill.Add(8, Resources.Load<GameObject>("VFX/Fireshock"));
            skill.Add(9, Resources.Load<GameObject>("VFX/Fireshock"));

            skillLevels.Add(Resources.Load<Sprite>("SkillLevelBorder/Level0_Border"));
            skillLevels.Add(Resources.Load<Sprite>("SkillLevelBorder/Level1_Border"));
            skillLevels.Add(Resources.Load<Sprite>("SkillLevelBorder/Level2_Border"));
            skillLevels.Add(Resources.Load<Sprite>("SkillLevelBorder/Level3_Border"));
        }
    }
}
