using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.SkillSystem.SkillSys
{
    public class SkillLibrary
    {
        private static SkillLibrary instance;
        public static SkillLibrary Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SkillLibrary();

                }
                return instance;
            }
        }
        List<Skill> skills = new List<Skill>();
        private static GameObject projectile;
        private static GameObject instant;
        private static GameObject aoe;

        public static GameObject Projectile
        {
            get
            {
                if (projectile == null)
                    projectile = Resources.Load<GameObject>("VFX/Fireball");
                return projectile;
            }
            set { }
        }

        public static GameObject Instant
        {
            get
            {
                if (instant == null)
                    instant = Resources.Load<GameObject>("VFX/Fireshock");
                return instant;
            }
            set { }
        }

        public static GameObject AoE
        {
            get
            {
                if (aoe== null)
                    aoe = Resources.Load<GameObject>("VFX/Fireball");
                return aoe;
            }
            set { }
        }
        public static List<Skill> Skills()
        {
            if (instance == null)
            {
                instance = new SkillLibrary();
            }
            return instance.skills;
        }
    }
}
