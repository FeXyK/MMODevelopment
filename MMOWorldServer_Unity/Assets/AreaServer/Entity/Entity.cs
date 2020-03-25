using Assets.AreaServer.SkillSystem;
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

        public Dictionary<int, SkillItem> Skills = new Dictionary<int, SkillItem>();

        //List<SkillBuff> buffs = new List<SkillBuff>();
        private void Start()
        {
            //for (int i = 0; i < 10; i++)
            //{
            //    Skills.Add(i, new SkillItem(i, 21, 4, i));
            //}
        }
        public void ApplyCD(int skillID)
        {
            Skills[skillID].SetCooldown();// = Time.time + cooldown;
            Debug.Log("COOLDOWN: " + Skills[skillID].GetCooldown());
        }
        public bool SkillReady(int skillID)
        {
            if (Skills[skillID].IsReady())
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
        public void ApplyDamage(int dmg)
        {
            EntityHealth -= dmg;
        }
        public int GetDamage(int skillID)
        {
            return Skills[skillID].GetDamage();
        }
    }
}