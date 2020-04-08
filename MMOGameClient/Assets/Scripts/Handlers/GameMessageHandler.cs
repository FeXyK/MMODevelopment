using Assets.Scripts.Character;
using Assets.Scripts.SkillSystem;
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

        internal void EntityHealthUpdate(NetIncomingMessage msgIn)
        {

            int targetID = msgIn.ReadInt16();
            EntityContainer target = dataHandler.GetEntity(targetID);
            target.Health = msgIn.ReadInt16();
            Debug.LogWarning(target.Health);
        }
        internal void SkillCasted(NetIncomingMessage msgIn)
        {
            int sourceID = msgIn.ReadInt16();
            int targetID = msgIn.ReadInt16();
            int skillID = msgIn.ReadInt16();

            EntityContainer target = dataHandler.GetEntity(targetID);
            EntityContainer source = dataHandler.GetEntity(sourceID);

            switch (skillID)
            {
                case 1:
                    //GameObject.Instantiate(SkillList.Instance.skill[skillID]).GetComponent<Skill_FireballScript>().Set(source.transform, target.transform);
                    break;
                case 4:
                    //GameObject.Instantiate(SkillList.Instance.skill[skillID], target.transform);
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

        internal void SkillListInformation(NetIncomingMessage msgIn)
        {
            int count = msgIn.ReadInt16();
            int skillID;
            int level;
            //uiManager.wSkill.SetActive(true);
            for (int i = 0; i < count; i++)
            {
                skillID = msgIn.ReadInt16();
                level = msgIn.ReadInt16();
                Debug.LogWarning("SKILLID: " + skillID);
                Debug.LogWarning("level: " + level);
                //foreach (var skill in GameObject.FindObjectOfType<SkillTreeController>().skills)
                //{
                //    Debug.LogWarning("skill: " + skill.SkillID);
                //    if (skill.SkillID == skillID)
                //    {
                //        skill.SetLevel((SkillRank)level);
                //    }
                //}
                //skillController.gameObject.SetActive(true);
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
