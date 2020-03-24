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
