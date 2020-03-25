using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.AreaServer.SkillSystem
{
    public class SkillItem
    {
        const float lagCompensation = 0.1f;
        int skillID;
        float cooldown;
        float cooldownByDefault;
        float cooldownByLevel;
        int damage;
        int baseDamage = 21;
        int level;
        //Effects effects;

        public SkillItem(int skillID, int baseDamage, float cooldownByDefault, int level)
        {
            this.skillID = skillID;
            this.baseDamage = baseDamage;
            this.cooldownByDefault = cooldownByDefault;
            SetLevel(level);
        }
        public void SetLevel(int level)
        {
            if (level <= 4)
            {
                this.level = level;
                this.damage = level * baseDamage;
                this.cooldownByLevel = (cooldownByDefault / (float)level) - 1f;
            }
        }
        public bool IsReady()
        {
            return cooldown <= Time.time ? true : false;
        }
        public void SetCooldown()
        {
            cooldown = Time.time + cooldownByLevel;
        }
        public float GetCooldown()
        {
            return cooldown -Time.time ;
        }

        internal int GetDamage()
        {
            return damage;
        }

        internal void LevelUp()
        {
            SetLevel(level + 1);
        }

        internal bool IsMaxLevel()
        {
            return level >= 4 ? true : false; 
        }

        internal int GetLevel()
        {
            return level;
        }
    }
}
