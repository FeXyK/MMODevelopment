using System.Collections.Generic;
using UnityEngine;
using Utility_dotNET_Framework.Models;

namespace Assets.AreaServer.SkillSystem
{
    public class SkillLibrary
    {
        public Dictionary<int, Skill> Skills = new Dictionary<int, Skill>();
        public GameObject Instant;
        public GameObject Projectile;
        public GameObject AoE;

        private static SkillLibrary instance;
        public static SkillLibrary Instance
        {
            get
            {
                if (instance == null)
                    instance = new SkillLibrary();
                return instance;
            }
        }
        public SkillLibrary()
        {
            //Instant = Resources.Load<GameObject>("Instant");
            Projectile = Resources.Load<GameObject>("Projectile");
            AoE = Resources.Load<GameObject>("AoE");
        }
    }
}
