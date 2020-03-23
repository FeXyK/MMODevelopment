using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.AreaServer.SkillSystem
{
    class SkillList
    {
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
