using System.Collections.Generic;
using UnityEngine;
using Utility_dotNET_Framework.Models;

namespace Assets.AreaServer.SkillSystem
{
    public class SkillList
    {
        public List<Skill> Skills = new List<Skill>();
        public GameObject Projectile;
        public GameObject AoE;

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
            Projectile = Resources.Load<GameObject>("Projectile");
            AoE = Resources.Load<GameObject>("AoE");
        }
    }
}
