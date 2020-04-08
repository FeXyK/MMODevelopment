using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                    instance = new SkillLibrary();
                return instance;
            }
        }
        List<Skill> skills = new List<Skill>();
        public static List<Skill> Skills()
        {
            if (instance == null)
                instance = new SkillLibrary();
            return instance.skills;
        }
    }
}
