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
        MenuController menuController;
        GameObject[] hiddenNameTags;
        bool hideNames = false;
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

        internal void HandleNewCharacter(NetIncomingMessage msgIn)
        {
            int characterID = msgIn.ReadInt16();
            Debug.Log("ADDING NEW CHARACTER ID: " + characterID);
            if (characterID == dataHandler.myCharacter.id)
            {
                return;
            }
            int characterLevel = msgIn.ReadInt16();
            int characterHealth = msgIn.ReadInt16();
            int characterType = msgIn.ReadInt16();
            string characterName = msgIn.ReadString();
            GameObject newCharacterObj = null;
            switch (characterType)
            {
                case 1:
                    newCharacterObj = GameObject.Instantiate(Resources.Load<GameObject>("Type1Character"));
                    break;
                case 2:
                    newCharacterObj = GameObject.Instantiate(Resources.Load<GameObject>("Type2Character"));
                    break;
                default:
                    newCharacterObj = GameObject.Instantiate(Resources.Load<GameObject>("Type1Character"));
                    break;
            }
            Character newCharacter = newCharacterObj.GetComponent<Character>();

            newCharacter.Set(characterID, characterLevel, characterHealth, characterType, characterName);
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
                float rot = msgIn.ReadFloat();
                dataHandler.otherCharacters[cId].posX = posX;
                dataHandler.otherCharacters[cId].posY = posY;
                dataHandler.otherCharacters[cId].posZ = posZ;
                dataHandler.otherCharacters[cId].rot = rot;

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
