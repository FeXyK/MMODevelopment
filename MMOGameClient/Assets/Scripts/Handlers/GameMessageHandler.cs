using Assets.Scripts.Character;
using Assets.Scripts.SkillSystem;
using Lidgren.Network;
using Lidgren.Network.ServerFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Handlers
{
    public class GameMessageHandler : Lidgren.Network.Message.MessageHandler
    {
        NetClient netClient;
        public GameDataHandler dataHandler;
        LoginMessageCreater messageCreater;
        LoginMessageReader messageReader;
        GameMessageSender messageSender;
        MenuController menuController;
        GameObject[] hiddenNameTags;
        bool hideNames = false;

        GameObject FireShock;

        public string ChatMessage
        {
            set
            {
                if (value.Length > 0)
                {
                    if (value.StartsWith("/admin "))
                    {
                        SendAdminChatMessage(value.Remove(0, 7));
                        Debug.Log(value.Remove(0, 7));
                    }
                    if (value.StartsWith("/w "))
                    {
                        string[] msg = value.Remove(0, 3).Split(' ');
                        Debug.Log(value.Remove(0, 3));
                        SendPrivateChatMessage(msg);
                    }
                    if (!value.StartsWith("/"))
                    {
                        SendChatMessage(value);
                    }
                }
            }
        }
        public GameMessageHandler(NetClient client)
        {
            netClient = client;
            messageSender = new GameMessageSender(netClient);
            dataHandler = new GameDataHandler();
            messageCreater = new LoginMessageCreater(netClient);
            messageReader = new LoginMessageReader();
            menuController = GameObject.FindObjectOfType<MenuController>();
        }

        private void SendPrivateChatMessage(string[] msg)
        {
            NetOutgoingMessage msgOut = netClient.CreateMessage();
            msgOut.Write((byte)MessageType.PrivateChatMessage);
            msgOut.Write(dataHandler.myCharacter.characterName);
            msgOut.Write(msg[0]);
            msgOut.Write(msg[1]);
            netClient.ServerConnection.SendMessage(msgOut, NetDeliveryMethod.ReliableOrdered, 1);
        }

        public void SendChatMessage(string msg)
        {
            NetOutgoingMessage msgOut = netClient.CreateMessage();
            msgOut.Write((byte)MessageType.PublicChatMessage);
            msgOut.Write(dataHandler.myCharacter.characterName);
            msgOut.Write(msg);
            netClient.ServerConnection.SendMessage(msgOut, NetDeliveryMethod.ReliableOrdered, 1);
        }
        public void SendAdminChatMessage(string msg)
        {
            NetOutgoingMessage msgOut = netClient.CreateMessage();
            msgOut.Write((byte)MessageType.AdminChatMessage);
            msgOut.Write(msg);
            netClient.ServerConnection.SendMessage(msgOut, NetDeliveryMethod.ReliableOrdered, 1);
        }

        internal void HandleAdminCommand(NetIncomingMessage msgIn)
        {
            MessageType msgType = (MessageType)msgIn.ReadByte();
            switch (msgType)
            {
                case MessageType.HideNames:
                    HideNameTags();
                    break;
                case MessageType.ShowNames:
                    ShowNameTags();
                    break;
                case MessageType.AdminChatMessage:
                    HandleChatMessage(msgIn, "A");
                    break;
            }
        }

        internal void EntityUpdate(NetIncomingMessage msgIn)
        {

            int targetID = msgIn.ReadInt16();
            Entity target;
            if (targetID == dataHandler.myCharacter.id)
                target = dataHandler.myCharacter;
            else
                target = dataHandler.otherCharacters[targetID];

            target.Health = msgIn.ReadInt16();
        }

        internal void SkillCasted(NetIncomingMessage msgIn)
        {
            int sourceID = msgIn.ReadInt16();
            int targetID = msgIn.ReadInt16();
            int skillID = msgIn.ReadInt16();

            Debug.Log("SOURCE ID: " + sourceID);
            Debug.Log("TARGET ID: " + targetID);
            Debug.Log("Characters count : " + dataHandler.otherCharacters.Count);

            foreach (var ch in dataHandler.otherCharacters)
            {
                Debug.Log("CHARACTERS: " + ch.Key);
            }
            Entity target;
            if (targetID != dataHandler.myCharacter.id)
                target = dataHandler.otherCharacters[targetID];
            else target = dataHandler.myCharacter;


            Entity source;
            if (sourceID != dataHandler.myCharacter.id)
                source = dataHandler.otherCharacters[sourceID];
            else source = dataHandler.myCharacter;
            switch (skillID)
            {
                case 1:
                    GameObject.Instantiate(SkillList.Instance.skill[skillID]).GetComponent<Skill_FireballScript>().Set(source.transform, target.transform);
                    break;
                case 4:
                    GameObject.Instantiate(SkillList.Instance.skill[skillID], target.transform);
                    break;
                default:
                    break;
            }

            //target.Health = msgIn.ReadInt16();
        }

        public void ShowNameTags()
        {
            hideNames = false;
            foreach (var item in hiddenNameTags)
            {
                item.SetActive(true);
            }
        }
        public void HideNameTags()
        {
            hideNames = true;
            if (hiddenNameTags != null)
                foreach (var item in hiddenNameTags)
                {
                    item.SetActive(true);
                }
            hiddenNameTags = GameObject.FindGameObjectsWithTag("NameTag");
            foreach (var item in hiddenNameTags)
            {
                item.SetActive(false);
            }
        }
        public void HandleChatMessage(NetIncomingMessage msgIn, string t = "G")
        {
            string from = msgIn.ReadString();
            string msg = msgIn.ReadString();
            menuController.ChatWindow.text += "\n[" + t + "]" + from + ": " + msg;
        }

        internal void PrintFeedBack(NetIncomingMessage msgIn)
        {
            throw new NotImplementedException();
        }
        internal void HandleMobAreaData(NetIncomingMessage msgIn)
        {
            int count = msgIn.ReadInt16();
            Debug.Log("MOBCOUNT: " + count);
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
                Debug.Log("Adding MOB: " + entityName);

                newMobObj.transform.position = new Vector3(x, y, z);
                Entity newMob = newMobObj.GetComponent<Entity>();
                newMob.Set(entityID, entityLevel, health, 0, entityName, maxHealth);
                dataHandler.otherCharacters.Add(entityID, newMob);
            }
        }
        internal void MobInformationUpdate(NetIncomingMessage msgIn)
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
                    dataHandler.otherCharacters[entityID].posX = posX;
                    dataHandler.otherCharacters[entityID].posY = posY;
                    dataHandler.otherCharacters[entityID].posZ = posZ;
                }
            }
        }
        internal void HandleNewCharacter(NetIncomingMessage msgIn)
        {
            int characterID = msgIn.ReadInt16();
            if (characterID == dataHandler.myCharacter.id)
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
            Entity newCharacter = newCharacterObj.GetComponent<Entity>();

            newCharacter.Set(characterID, characterLevel, characterHealth, characterType, characterName, characterMaxHealth);
            if (dataHandler.otherCharacters.ContainsKey(newCharacter.id))
            {
                GameObject.Destroy(dataHandler.otherCharacters[newCharacter.id].character.gameObject);
                dataHandler.otherCharacters.Remove(newCharacter.id);
            }
            dataHandler.otherCharacters.Add(newCharacter.id, newCharacter);
            if (hideNames)
                HideNameTags();
        }
        internal void HandePositionUpdate(NetIncomingMessage msgIn)
        {
            int cId = msgIn.ReadInt16();
            if (cId == dataHandler.myCharacter.id)
            {
                return;
            }
            if (dataHandler.otherCharacters.ContainsKey(cId))
            {
                float posX = msgIn.ReadFloat();
                float posY = msgIn.ReadFloat();
                float posZ = msgIn.ReadFloat();
                dataHandler.otherCharacters[cId].posX = posX;
                dataHandler.otherCharacters[cId].posY = posY;
                dataHandler.otherCharacters[cId].posZ = posZ;
            }
        }
        public void SendClientReady()
        {
            NetOutgoingMessage msgReady = netClient.CreateMessage();
            msgReady.Write((byte)MessageType.ClientReady);
            netClient.SendMessage(msgReady, NetDeliveryMethod.ReliableOrdered);
        }

        public void HandleCharacterRemove(NetIncomingMessage msgIn)
        {
            int cRemoveId = msgIn.ReadInt16();
            GameObject.Destroy(dataHandler.otherCharacters[cRemoveId].gameObject);
            dataHandler.otherCharacters.Remove(cRemoveId);
        }
        public void SendPositionUpdate()
        {
            if (netClient.ServerConnection == null)
                return;

            NetOutgoingMessage msgOut = netClient.CreateMessage();
            msgOut.Write((byte)MessageType.CharacterMovement);
            msgOut.Write(dataHandler.myCharacter.id, 16);
            msgOut.Write(dataHandler.myCharacter.transform.position.x);
            msgOut.Write(dataHandler.myCharacter.transform.position.y);
            msgOut.Write(dataHandler.myCharacter.transform.position.z);
            msgOut.Write(dataHandler.myCharacter.transform.rotation.eulerAngles.y);
            netClient.ServerConnection.SendMessage(msgOut, NetDeliveryMethod.ReliableOrdered, 1);
        }
    }
}
