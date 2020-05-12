using Assets.Scripts.Character;
using Assets.Scripts.InventorySystem;
using Assets.Scripts.SkillSystem;
using Assets.Scripts.SkillSystem.SkillSys;
using Assets.Scripts.UI;
using Assets.Scripts.UI.UIItems;
using Assets.Scripts.UI_Window;
using Lidgren.Network;
using Lidgren.Network.Message;
using Lidgren.Network.ServerFiles;
using System;
using UnityEngine;

namespace Assets.Scripts.Handlers
{
    public class GameMessageHandler : Lidgren.Network.Message.MessageHandler
    {
        NetClient netClient;
        public GameDataHandler dataHandler;
        GameMessageSender messageSender;
        UIManager uiManager;


        public GameMessageHandler(NetClient client)
        {
            netClient = client;
            dataHandler = new GameDataHandler();
            uiManager = GameObject.FindObjectOfType<UIManager>();



            messageSender = new GameMessageSender(netClient, dataHandler);
        }
        public string ChatMessage
        {
            set
            {
                if (value.Length > 0)
                {
                    if (value.StartsWith("/admin "))
                    {
                        messageSender.SendAdminChatMessage(value.Remove(0, 7));
                        Debug.Log(value.Remove(0, 7));
                    }
                    if (value.StartsWith("/w "))
                    {
                        string[] msg = value.Remove(0, 3).Split(' ');
                        Debug.Log(value.Remove(0, 3));
                        messageSender.SendPrivateChatMessage(msg);
                    }
                    if (!value.StartsWith("/"))
                    {
                        messageSender.SendChatMessage(value);
                    }
                }
            }
        }


        internal void AdminCommand(NetIncomingMessage msgIn)
        {
            MessageType msgType = (MessageType)msgIn.ReadByte();
            switch (msgType)
            {
                case MessageType.AdminChatMessage:
                    HandleChatMessage(msgIn, "A");
                    break;
            }
        }

        internal void EntityResourceUpdate(NetIncomingMessage msgIn)
        {

            int targetID = msgIn.ReadInt16();
            EntityContainer target = dataHandler.GetEntity(targetID);
            target.Health = msgIn.ReadInt16();
            target.Mana = msgIn.ReadInt16();
        }
        internal void SkillCasted(NetIncomingMessage msgIn)
        {
            int sourceID = msgIn.ReadInt16();
            int targetID = msgIn.ReadInt16();
            int skillID = msgIn.ReadInt16();

            EntityContainer target = dataHandler.GetEntity(targetID);
            EntityContainer source = dataHandler.GetEntity(sourceID);
            if (sourceID == dataHandler.myCharacter.entity.id)
            {
                uiManager.wHotbar.SetCooldown(skillID);
            }
            switch (skillID)
            {
                case 1:
                    GameObject.Instantiate(SkillLibrary.Projectile).GetComponent<Skill_FireballScript>().Set(source.transform, target.transform);
                    break;
                case 0:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                    GameObject.Instantiate(SkillLibrary.Shock).transform.position = target.transform.position;
                    break;
                case 7:
                    GameObject.Instantiate(SkillLibrary.Instant).transform.position = target.transform.position;
                    break;
                case 8:
                case 9:
                    GameObject.Instantiate(SkillLibrary.AoE).transform.position = target.transform.position;
                    break;
                case 10:
                    GameObject.Instantiate(SkillLibrary.Manawave, target.transform);//.transform.position = target.transform.position;
                    break;
                case 11:
                    GameObject.Instantiate(SkillLibrary.EssenceOfLife, target.transform);//transform.position = target.transform.position;
                    break;
                default:
                    break;
            }
        }

        public void HandleChatMessage(NetIncomingMessage msgIn, string t = "G")
        {
            string from = msgIn.ReadString();
            string msg = msgIn.ReadString();
            uiManager.wChat.WriteLine("[" + t + "]" + from + ": " + msg);
        }

        internal void MobSpawn(NetIncomingMessage msgIn)
        {
            int count = msgIn.ReadInt16();
            //Debug.Log("MOBCOUNT: " + count);
            for (int i = 0; i < count; i++)
            {
                GameObject newMobObj = GameObject.Instantiate(Resources.Load<GameObject>("Mob"));

                string entityName = msgIn.ReadString();
                int entityID = msgIn.ReadInt16();
                int entityLevel = msgIn.ReadInt16();
                float x = msgIn.ReadFloat();
                float y = msgIn.ReadFloat();
                float z = msgIn.ReadFloat();
                int health = msgIn.ReadInt16();
                int maxHealth = msgIn.ReadInt16();
                //Debug.Log("Adding MOB: " + entityName);

                newMobObj.transform.position = new Vector3(x, y, z);
                EntityContainer newMob = newMobObj.GetComponent<EntityContainer>();
                newMob.Set(entityID, entityLevel, health, 0, entityName, maxHealth);
                dataHandler.otherCharacters.Add(entityID, newMob);
            }
        }
        internal void MobPositionUpdate(NetIncomingMessage msgIn)
        {

            int count = msgIn.ReadInt16();
            for (int i = 0; i < count; i++)
            {
                int entityID = msgIn.ReadInt16();
                if (dataHandler.otherCharacters.ContainsKey(entityID))
                {
                    float posX = msgIn.ReadFloat();
                    float posY = msgIn.ReadFloat();
                    float posZ = msgIn.ReadFloat();
                    dataHandler.otherCharacters[entityID].entity.position = new Vector3(posX, posY, posZ);
                }
            }
        }

        internal void EquipItem(NetIncomingMessage msgIn)
        {
            int slotID = msgIn.ReadInt16();

            uiManager.wCharacter.Equip(slotID);
        }
        internal void UnequipItem(NetIncomingMessage msgIn)
        {
            int slotID = msgIn.ReadInt16();
            int iSlotID = msgIn.ReadInt16();
            uiManager.wCharacter.Unequip(slotID, iSlotID);
        }

        internal void StorageInfo(NetIncomingMessage msgIn)
        {
            throw new NotImplementedException();
        }

        internal void RemoveItem(NetIncomingMessage msgIn)
        {
            int ID = msgIn.ReadInt32();
            int amount = msgIn.ReadInt16();
            Debug.LogWarning(ID);
            Debug.LogWarning(amount);
            uiManager.wInvertory.RemoveItem(ID, amount);
        }
        internal void LootDrop(NetIncomingMessage msgIn)
        {
            int entityID = msgIn.ReadInt32();
            int corpseID = msgIn.ReadInt32();

            GameObject newDrop = null;
            int transactionID;
            int ID;
            int Level;
            int Amount;

            int count = msgIn.ReadInt16();

            for (int i = 0; i < count; i++)
            {
                transactionID = msgIn.ReadInt32();

                Debug.LogWarning("inc tID: " + transactionID);
                ID = msgIn.ReadInt32();
                Level = msgIn.ReadInt16();
                Amount = msgIn.ReadInt16();
                if (ItemLibrary.Items().ContainsKey(ID))
                {
                    switch (ItemLibrary.Items()[ID].Rarity)
                    {
                        case EItemRarity.Scrap:
                            newDrop = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/ItemDrop/Common"), dataHandler.GetEntity(corpseID).transform.position, Quaternion.identity);
                            break;
                        case EItemRarity.Common:
                            newDrop = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/ItemDrop/Common"), dataHandler.GetEntity(corpseID).transform.position, Quaternion.identity);
                            break;
                        case EItemRarity.Uncommon:
                            newDrop = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/ItemDrop/Uncommon"), dataHandler.GetEntity(corpseID).transform.position, Quaternion.identity);
                            break;
                        case EItemRarity.Rare:
                            newDrop = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/ItemDrop/Rare"), dataHandler.GetEntity(corpseID).transform.position, Quaternion.identity);
                            break;
                        case EItemRarity.Epic:
                            newDrop = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/ItemDrop/Epic"), dataHandler.GetEntity(corpseID).transform.position, Quaternion.identity);
                            break;
                        case EItemRarity.Legendary:
                            newDrop = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/ItemDrop/Legendary"), dataHandler.GetEntity(corpseID).transform.position, Quaternion.identity);
                            break;
                    }

                    newDrop.transform.position += new Vector3(UnityEngine.Random.Range(-2, 2), 0, UnityEngine.Random.Range(-2, 2));
                    newDrop.GetComponent<DropItemController>().TransactionID = transactionID;
                    newDrop.GetComponent<DropItemController>().entityID = entityID;
                    newDrop.GetComponent<DropItemController>().Amount = Amount;
                    newDrop.GetComponent<DropItemController>().Level = Level;
                    newDrop.GetComponent<DropItemController>().ID = ID;
                    newDrop.GetComponent<DropItemController>().Name.text = ID + ": " + ItemLibrary.Items()[ID].Name + " x " + Amount + "\nLevel: " + Level + "\nItem of " + entityID;
                    newDrop.GetComponent<DropItemController>().uiManager = uiManager;
                }
            }
        }
        internal void EntitySpawn(NetIncomingMessage msgIn)
        {
            int characterID = msgIn.ReadInt16();
            if (characterID == dataHandler.myCharacter.entity.id)
            {
                return;
            }
            int characterLevel = msgIn.ReadInt16();
            int characterHealth = msgIn.ReadInt16();
            int characterMaxHealth = msgIn.ReadInt16();
            CharacterApperance characterType = (CharacterApperance)msgIn.ReadInt16();
            string characterName = msgIn.ReadString();
            Debug.Log("ADDING NEW CHARACTER ID: " + characterID + " HEALTH " + characterHealth);

            GameObject newCharacterObj = null;
            switch (characterType)
            {
                case CharacterApperance.Male:
                    newCharacterObj = GameObject.Instantiate(Resources.Load<GameObject>("TypeMale"));
                    break;
                case CharacterApperance.Female:
                    newCharacterObj = GameObject.Instantiate(Resources.Load<GameObject>("TypeFemale"));
                    break;
                case CharacterApperance.NotDecided:
                    newCharacterObj = GameObject.Instantiate(Resources.Load<GameObject>("TypeNonDecided"));
                    break;
                default:
                    newCharacterObj = GameObject.Instantiate(Resources.Load<GameObject>("TypeMale"));
                    break;
            }
            EntityContainer newCharacter = newCharacterObj.GetComponent<EntityContainer>();

            newCharacter.Set(characterID, characterLevel, characterHealth, characterType, characterName, characterMaxHealth);
            if (dataHandler.otherCharacters.ContainsKey(newCharacter.entity.id))
            {
                GameObject.Destroy(dataHandler.otherCharacters[newCharacter.entity.id].gameObject);
                dataHandler.otherCharacters.Remove(newCharacter.entity.id);
            }
            dataHandler.otherCharacters.Add(newCharacter.entity.id, newCharacter);
        }
        internal void SkillLeveled(NetIncomingMessage msgIn)
        {
            int skillID = msgIn.ReadByte();
            int level = msgIn.ReadByte();

            if (dataHandler.myCharacter.entity.skills.ContainsKey(skillID))
                dataHandler.myCharacter.entity.skills[skillID] = level;
            else
                dataHandler.myCharacter.entity.skills[skillID] = level;
            uiManager.wSkill.Refresh();
        }
        internal void AddedItem(NetIncomingMessage msgIn)
        {
            int ID = msgIn.ReadInt32();
            int level = msgIn.ReadInt16();
            int amount = msgIn.ReadInt16();
            Debug.LogWarning(ID);
            Debug.LogWarning(level);
            Debug.LogWarning(amount);
            UIContainer item = null;
            if (ItemLibrary.Items()[ID].MaxAmount == 0)
                ItemLibrary.Items()[ID].MaxAmount = 1;

            while (amount > 0)
            {
                if (ItemLibrary.Items()[ID].MaxAmount < amount)
                {
                    item = new UIContainer(ItemLibrary.Items()[ID].MaxAmount, level, ItemLibrary.Items()[ID]);
                    amount -= ItemLibrary.Items()[ID].MaxAmount;
                }
                else
                {
                    item = new UIContainer(amount, level, ItemLibrary.Items()[ID]);
                    amount = 0;
                }
                uiManager.wInvertory.AddItem(item);
            }
        }
        internal void EntityPositionUpdate(NetIncomingMessage msgIn)
        {
            int entityID = msgIn.ReadInt16();
            if (entityID == dataHandler.myCharacter.entity.id)
            {
                return;
            }
            if (dataHandler.otherCharacters.ContainsKey(entityID))
            {
                float posX = msgIn.ReadFloat();
                float posY = msgIn.ReadFloat();
                float posZ = msgIn.ReadFloat();
                dataHandler.otherCharacters[entityID].entity.position = new Vector3(posX, posY, posZ);
            }
        }
        public void EntityDespawn(NetIncomingMessage msgIn)
        {
            int cRemoveId = msgIn.ReadInt16();
            GameObject.Destroy(dataHandler.otherCharacters[cRemoveId].gameObject);
            dataHandler.otherCharacters.Remove(cRemoveId);
        }
        internal void Notification(NetIncomingMessage msgIn)
        {
            uiManager.wChat.WriteLine(msgIn.ReadString());
        }
    }
}
