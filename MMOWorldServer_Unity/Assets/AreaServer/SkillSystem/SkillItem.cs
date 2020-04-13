using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Utility_dotNET_Framework.Models;

namespace Assets.AreaServer.SkillSystem
{
    public class SkillItem
    {
        const float lagCompensation = 0.1f;
        public int skillID;
        public int Level;
        public float cooldown;
        public SkillType skillType = SkillType.Projectile;

        public int ManaCost;
        public float ManaCostMultiplier;
        public int GoldCost;
        public float GoldCostMultiplier;

        public float CastTime;

        public Dictionary<int, Effect> effects = new Dictionary<int, Effect>();

        public SkillItem(Skill skill, int Level)
        {
            skillID = skill.ID;
            skillType = (SkillType)SkillLibrary.Instance.Skills[skillID].SkillType;
            this.Level = Level;
            ManaCost = skill.ManaCost;
            ManaCostMultiplier = skill.ManaCostMultiplier;
            GoldCost = skill.GoldCost;
            GoldCostMultiplier = skill.GoldCostMultiplier;
            foreach (var effect in SkillLibrary.Instance.Skills[skillID].Effects.Values)
            {
                Effect newEffect = new Effect();
                newEffect.EffectID = effect.EffectID;
                newEffect.Value = effect.Value;
                newEffect.Multiplier = effect.Multiplier;
                newEffect.MinLevel = effect.MinLevel;

                effects.Add(effect.EffectID, newEffect);
            }
        }
        public bool IsReady()
        {
            return cooldown <= Time.time ? true : false;
        }
        public void SetCooldown()
        {
            cooldown = Time.time + GetCooldown();
        }
        public float GetCooldown()
        {
            if (effects.ContainsKey((int)EffectType.Cooldown))
                return effects[(int)EffectType.Cooldown].Value * Mathf.Pow(effects[130].Multiplier, Level - 1);
            return 0;
        }
        internal bool IsMaxLevel()
        {
            return Level >= 4 ? true : false;
        }

        internal int GetManaCost()
        {

            return (int)(ManaCost * (ManaCostMultiplier * (Level - 1)));
        }

        internal int GetLevel()
        {
            return Level;
        }

        internal void LevelUp()
        {
            if (Level < 4)
            {
                Level++;
            }
        }
    }
}
