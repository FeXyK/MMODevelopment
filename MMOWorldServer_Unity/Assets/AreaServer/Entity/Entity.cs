using Assets.AreaServer.SkillSystem;
using Assets.Scripts.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.AreaServer.Entity
{
    public class Entity : MonoBehaviour
    {
        public string EntityName;

        public int EntityID;
        public int EntityInventoryID;
        public int EntitySkillTreeID;
        public int EntityLevel;
        public int EntityGold;

        public int EntityMaxHealth;
        private int entityHealth;

        public int EntityHealth
        {
            get { return entityHealth; }
            set
            {
                entityHealth = value;

                AreaMessageSender.Instance.SendEntityUpdate(this);
            }
        }
        public int EntityMaxMana;
        public int EntityMana;

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

        internal void ApplyDamage(int dmg)
        {
            EntityHealth -= dmg;
        }
    }
}