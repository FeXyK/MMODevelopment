using Assets.AreaServer.InventorySystem;
using Assets.AreaServer.SkillSystem;
using Assets.Scripts.Handlers;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using UnityEngine;
using Utility.Models;

namespace Assets.AreaServer.Entity
{
    public class Entity : MonoBehaviour
    {
        public NetConnection Connection;

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
                if (entityHealth < 0)
                {
                    entityHealth = 0;
                }
                if (entityHealth > EntityMaxHealth)
                {
                    entityHealth = EntityMaxHealth;
                }
                AreaMessageSender.Instance.SendEntityUpdate(this);
            }
        }
        public int EntityMana
        {
            get { return entityMana; }
            set
            {
                entityMana = value;
                if (entityMana < 0)
                    entityMana = 0;
                if (entityMana > EntityMaxMana)
                    entityMana = EntityMaxMana;
                AreaMessageSender.Instance.SendEntityUpdate(this);
            }
        }

        public int MaxInventorySize = 32;
        public int MaxStorageSize = 32;
        public int CurrentInventorySize = 0;
        public int CurrentStorageSize = 0;

        public Vector3 position;

        public Dictionary<int, SkillItem> Skills = new Dictionary<int, SkillItem>();
        public Dictionary<int, SlotItem> Inventory = new Dictionary<int, SlotItem>();
        public Dictionary<EItemType, SlotItem> Equipped = new Dictionary<EItemType, SlotItem>();

        public Dictionary<float, Effect> Buffs = new Dictionary<float, Effect>();

        public int MagicResist;
        public int Armor;

        public void ApplyCD(int skillID)
        {
            Skills[skillID].SetCooldown();// = Time.time + cooldown;
        }
        public bool CastSkill(int skillID)
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
                Debug.LogError("No SkillID found: " + skillID);
            return false;
        }
        public void ApplyEffects(SkillItem skill, Entity source)
        {
            if (EntityHealth > 0)
                foreach (var effect in skill.effects)
                {
                    //Debug.Log((EffectType)effect.Value.EffectID + " " + (int)(effect.Value.Value * (effect.Value.Multiplier * skill.Level)));
                    if (skill.Level >= effect.Value.MinLevel)
                        switch ((EffectType)effect.Key)
                        {
                            case EffectType.RestoreHealth:
                                EntityHealth += (int)(effect.Value.LeveledValue(skill.Level));
                                break;
                            case EffectType.RestoreMana:
                                EntityMana += (int)(effect.Value.LeveledValue(skill.Level));
                                break;
                            case EffectType.PhysicalDamage:
                            case EffectType.TrueDamage:
                            case EffectType.SpellDamage:
                                //if (source.EntityID != EntityID)
                                EntityHealth -= DamageCalculation(effect.Value, skill.Level);
                                if (EntityHealth <= 0)
                                {
                                    OnDie(source);
                                }
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

        private int DamageCalculation(Effect effect, int level)
        {
            if (effect.EffectID == (int)EffectType.PhysicalDamage)
                return (int)(effect.LeveledValue(level) - Armor);
            if (effect.EffectID == (int)EffectType.SpellDamage)
                return (int)(effect.LeveledValue(level) - MagicResist);
            return (int)effect.LeveledValue(level);
        }

        internal void EquipItem(int slotID)
        {
            foreach (var item in Inventory)
            {
                Debug.Log(item.Key);
            }
            Debug.Log(slotID);
            if (Inventory.ContainsKey(slotID))
                if (Equipped.ContainsKey(Inventory[slotID].ItemType))
                {
                    UnequipItem(Inventory[slotID].ItemType, slotID);
                    CalculateStats();
                }
                else
                {
                    Debug.Log("got: " + slotID);
                    Debug.Log("got: " + Inventory[slotID].ItemType);
                    switch (Inventory[slotID].ItemType)
                    {
                        case EItemType.MainHand:
                        case EItemType.OffHand:
                            if (Equipped.ContainsKey(EItemType.MainHand))
                                if (!Equipped.ContainsKey(EItemType.MainHand))
                                    Equipped.Add(EItemType.MainHand, Inventory[slotID]);
                                else
                                    Equipped.Add(EItemType.MainHand, Inventory[slotID]);
                            else
                                Equipped.Add(EItemType.MainHand, Inventory[slotID]);
                            break;
                        case EItemType.LeftRing:
                        case EItemType.RightRing:
                            if (Equipped.ContainsKey(EItemType.LeftRing))
                                if (!Equipped.ContainsKey(EItemType.RightRing))
                                    Equipped.Add(EItemType.RightRing, Inventory[slotID]);
                                else
                                    Equipped.Add(EItemType.LeftRing, Inventory[slotID]);
                            else
                                Equipped.Add(EItemType.LeftRing, Inventory[slotID]);
                            break;
                        case EItemType.LeftEarring:
                        case EItemType.RightEarring:
                            if (Equipped.ContainsKey(EItemType.LeftEarring))
                                if (!Equipped.ContainsKey(EItemType.RightEarring))
                                    Equipped.Add(EItemType.RightEarring, Inventory[slotID]);
                                else
                                    Equipped.Add(EItemType.LeftEarring, Inventory[slotID]);
                            else
                                Equipped.Add(EItemType.LeftEarring, Inventory[slotID]);
                            break;

                        default:
                            Equipped.Add(Inventory[slotID].ItemType, Inventory[slotID]);
                            Inventory.Remove(slotID);
                            break;
                    }
                    AreaMessageSender.Instance.EquippedInventoryItem(Connection, slotID);
                    CalculateStats();
                }
        }
        internal void UnequipItem(EItemType eSlotID, int iSlotID = -1)
        {

            foreach (var item in Equipped)
            {
                if (eSlotID == (EItemType)item.Value.SlotID)
                {
                    if (iSlotID == -1)
                    {
                        for (int i = 0; i < MaxInventorySize; i++)
                        {
                            if (!Inventory.ContainsKey(i))
                            {
                                iSlotID = i;
                                Inventory.Add(i, item.Value);
                                Equipped.Remove(item.Key);
                                break;
                            }
                        }
                    }
                    else
                    {
                        SlotItem Temp = item.Value;
                        Equipped[item.Key] = Inventory[iSlotID];
                        Inventory[iSlotID] = Temp;
                    }
                    break;
                }
            }
            if (iSlotID == -1)
                Debug.LogError("iSlotID was: " + iSlotID);
            AreaMessageSender.Instance.UnequippedInventoryItem(Connection, (int)eSlotID, iSlotID);
            foreach (var item in Inventory)
            {
                Debug.Log(item.Value.ID);
            }
        }
        public void CalculateStats()
        {
            Dictionary<EffectType, int> bonusEffects = new Dictionary<EffectType, int>();
            foreach (var item in Equipped.Values)
            {
                foreach (var effect in item.effects)
                {
                    EffectType key = (EffectType)effect.Value.EffectID;
                    if (bonusEffects.ContainsKey(key))
                    {
                        bonusEffects[key] += (int)effect.Value.LeveledValue(item.Level);
                    }
                    else
                    {
                        bonusEffects.Add(key, (int)effect.Value.LeveledValue(item.Level));
                    }
                }
            }
            MagicResist += bonusEffects.ContainsKey(EffectType.MagicResist) ? bonusEffects[EffectType.MagicResist] : 0;
            Armor += bonusEffects.ContainsKey(EffectType.Armor) ? bonusEffects[EffectType.Armor] : 0;
        }
        internal void Use(int slotID)
        {
            if (Inventory.ContainsKey(slotID))
            {
                ApplyEffects(Inventory[slotID]);
                RemoveInventoryItem(slotID, 1);
            }
        }


        public void AddInventoryItem(SlotItem item)
        {
            if (CurrentInventorySize < MaxInventorySize)
            {
                if (!Inventory.ContainsKey(item.ID))
                {
                    Inventory.Add(item.ID, item);
                    CurrentInventorySize += item.InventorySpace();
                }
                else
                {
                    CurrentInventorySize -= Inventory[item.ID].InventorySpace();
                    Inventory[item.ID].Amount += item.Amount;
                    CurrentInventorySize += Inventory[item.ID].InventorySpace();
                }
                AreaMessageSender.Instance.AddedItem(Connection, item);
            }
        }
        private void RemoveInventoryItem(int ID, int amount)
        {
            if (Inventory.ContainsKey(ID))
            {
                Inventory[ID].Amount -= amount;
                if (Inventory[ID].Amount == 0)
                {
                    Inventory.Remove(ID);
                    CurrentInventorySize--;
                }
                AreaMessageSender.Instance.RemovedItem(Connection, ID, amount);
            }
        }
        public virtual void OnDie(Entity source)
        {

        }
        public virtual void OnRespawn()
        {

        }
        private void ApplyEffects(SlotItem slotItem)
        {
            if (EntityHealth > 0)
                foreach (var effect in slotItem.effects)
                {
                    switch ((EffectType)effect.Key)
                    {
                        case EffectType.RestoreHealth:
                            EntityHealth += (int)effect.Value.LeveledValue(slotItem.Level);
                            break;
                        case EffectType.RestoreMana:
                            EntityMana += (int)effect.Value.LeveledValue(slotItem.Level);
                            break;
                    }
                }

        }
    }
}