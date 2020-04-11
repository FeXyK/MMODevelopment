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
        public int EntityMaxMana;

        private int entityHealth;
        private int entityMana;
        public int EntityHealth
        {
            get { return entityHealth; }
            set
            {
                entityHealth = value;
                AreaMessageSender.Instance.SendEntityUpdate(this);
            }
        }
        public int EntityMana
        {
            get { return entityMana; }
            set
            {
                entityMana = value;
                AreaMessageSender.Instance.SendEntityUpdate(this);
            }
        }
        public float EntityAttackRange;
        public float EntityBaseArmor;
        public float EntityBaseMagicResist;

        public Dictionary<int, SkillItem> Skills = new Dictionary<int, SkillItem>();
        internal Vector3 position;


        public void ApplyCD(int skillID)
        {
            Skills[skillID].SetCooldown();// = Time.time + cooldown;
        }
        public bool Cast(int skillID)
        {
            if (Skills.ContainsKey(skillID))
            {
                if (Skills[skillID].IsReady() && Skills[skillID].GetManaCost() <= EntityMana)
                {
                    EntityMana -= Skills[skillID].GetManaCost();
                    return true;
                }
            }
            else
                Debug.LogWarning("No skillID found: " + skillID);
            return false;
        }
        public void BasicAttack(Entity Target)
        {

        }
        public void CastSkill(Entity Target)
        {

        }
        public void ApplyEffects(SkillItem skill)
        {
            foreach (var effect in skill.effects)
            {
                Debug.Log("Value: " + effect.Value.Value);
                Debug.Log("Multiplier: " + effect.Value.Multiplier);
                Debug.Log("MinLevel: " + effect.Value.MinLevel);
                Debug.Log("Level: " + skill.Level);
                Debug.Log((EffectType)effect.Value.EffectID + " " + (int)(effect.Value.Value * (effect.Value.Multiplier * skill.Level)));
                if (skill.Level >= effect.Value.MinLevel)
                    switch ((EffectType)effect.Key)
                    {
                        case EffectType.Damage:
                            EntityHealth -= (int)(effect.Value.LeveledValue(skill.Level));
                            break;
                        case EffectType.RestoreHealth:
                            EntityHealth += (int)(effect.Value.LeveledValue(skill.Level));
                            break;
                        case EffectType.RestoreMana:
                            EntityMana += (int)(effect.Value.LeveledValue(skill.Level));
                            break;
                        case EffectType.AttackDamage:
                            EntityHealth -= (int)(effect.Value.LeveledValue(skill.Level));
                            break;
                        case EffectType.SpellDamage:
                            EntityHealth -= (int)(effect.Value.LeveledValue(skill.Level));
                            break;
                        case EffectType.MagicResist:
                            break;
                        case EffectType.Armor:
                            break;
                        case EffectType.Duration:
                            break;
                        case EffectType.Health:
                            break;
                        case EffectType.Mana:
                            break;
                        case EffectType.MoveSpeed:
                            break;
                        case EffectType.MoveJump:
                            break;
                        case EffectType.CooldownReduction:
                            break;
                        case EffectType.Cooldown:
                            break;
                        case EffectType.AttrStrength:
                            break;
                        case EffectType.AttrIntelligence:
                            break;
                        case EffectType.AttrDexterity:
                            break;
                        case EffectType.AttrConstitution:
                            break;
                        case EffectType.AttrKnowledge:
                            break;
                        case EffectType.AttrLuck:
                            break;
                        default:
                            break;
                    }
            }
        }
    }
}