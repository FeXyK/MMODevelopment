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
        private static GameObject explosion;
        private static GameObject aoe;
        private static GameObject manawave;
        private static GameObject essenceoflife;
        private static GameObject grabbingshock;
        private static GameObject shock;
        private static GameObject sunfire;
        private static GameObject fireclaw;
        private static GameObject fireshock;
        private static GameObject shockwave;
        private static GameObject basicattack;

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
                if (explosion == null)
                    explosion = Resources.Load<GameObject>("VFX/Fireshock");
                return explosion;
            }
            set { }
        }
        public static GameObject Shock
        {
            get
            {
                if (shock == null)
                    shock= Resources.Load<GameObject>("VFX/Shock");
                return shock;
            }
            set { }
        }

        public static GameObject AoE
        {
            get
            {
                if (aoe == null)
                    aoe = Resources.Load<GameObject>("VFX/Singularity");
                return aoe;
            }
            set { }
        }

        public static GameObject EssenceOfLife
        {
            get
            {
                if (essenceoflife == null)
                    essenceoflife = Resources.Load<GameObject>("VFX/Essence of Life");
                return essenceoflife;
            }
            set => essenceoflife = value;
        }
        public static GameObject Manawave
        {
            get
            {
                if (manawave == null)
                    manawave = Resources.Load<GameObject>("VFX/Manawave");
                return manawave;
            }
            set => manawave = value;
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
