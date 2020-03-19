using Assets.AreaServer.SkillSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.AreaServer.Entity
{
    class Entity : MonoBehaviour
    {
        public string EntityName;

        public int EntityID;
        public int EntityInventoryID;
        public int EntitySkillTreeID;
        public int EntityLevel;
        public int EntityGold;

        public float EntityMaxHealth;
        public float EntityHealth;
        public float EntityMaxMana;
        public float EntityMana;

        public float EntityAttackRange;
        public float EntityBaseArmor;
        public float EntityBaseMagicResist;

        List<SkillBuff> buffs = new List<SkillBuff>();

        public virtual void Update()
        {
            ApplyBuffEffects();
        }
        public void BasicAttack(Entity Target)
        {

        }
        public void CastSkill(Entity Target)
        {

        }
        public void ApplySkill(SkillBase skill)
        {

        }
        private void ApplyBuffEffects()
        {
            foreach (var buff in buffs)
            {
                buff.ApplyEffect();
            }
        }
       
    }
}