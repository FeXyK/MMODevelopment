using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.SkillSystem.SkillSys
{
    class SkillTree
    {
        internal List<Skill> skills = new List<Skill>();
        private Dictionary<int, int> knownSkills;

        public SkillTree(Dictionary<int, int> knownSkills)
        {
            this.knownSkills = knownSkills;

            foreach (var skill in SkillLibrary.Skills())
            {
                skills.Add(new Skill(skill));
            }
            foreach (var skill in knownSkills)
            {
                skills[skill.Key].level = skill.Value;
            }
        }
    }
}
