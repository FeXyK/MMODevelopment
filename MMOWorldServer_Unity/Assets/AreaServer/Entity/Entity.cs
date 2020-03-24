using Assets.Scripts.Handlers;
using System.Collections.Generic;
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

        public Dictionary<int, float> skillCooldown = new Dictionary<int, float>();

        //List<SkillBuff> buffs = new List<SkillBuff>();
        private void Start()
        {
            for (int i = 0; i < 10; i++)
            {
                skillCooldown.Add(i, Time.time);
            }
        }
        public virtual void Update()
        {
        }
        public void ApplyCD(int skillID, float cooldown)
        {
            skillCooldown[skillID] = Time.time + cooldown;
            Debug.Log("COOLDOWN: " + Time.time + "       Time: " + skillCooldown[skillID]);
        }
        public bool SkillReady(int skillID)
        {
            if (skillCooldown[skillID] < Time.time)
            {
                return true;
            }
            return false;
        }
        public void BasicAttack(Entity Target)
        {

        }
        public void CastSkill(Entity Target)
        {

        }
        internal void ApplyDamage(int dmg)
        {
            EntityHealth -= dmg;
        }
    }
}