using Assets.Scripts.Character;
using Assets.Scripts.SkillSystem;
using Assets.Scripts.UI;
using Assets.Scripts.UI.UIItems;
using Lidgren.Network;
using System;
using UnityEngine;

namespace Assets.Scripts.Handlers
{
    public class GameMessageSender
    {
        private static GameMessageSender instance = null;
        private NetClient netClient;
        private GameDataHandler dataHandler;
        GameMessageCreater messageCreater;

        public EntityContainer target;

        public GameMessageSender(NetClient netClient, GameDataHandler dataHandler)
        {
            if (instance == null)
            {
                messageCreater = new GameMessageCreater(netClient);
                this.dataHandler = dataHandler;
                this.netClient = netClient;
                instance = this;
            }
        }

        internal void GetSkillInformation()
        {
            NetOutgoingMessage msgOut = messageCreater.SkillInformation();
            netClient.ServerConnection.SendMessage(msgOut, NetDeliveryMethod.ReliableOrdered, 1);
        }

        public static GameMessageSender Instance
        {
            get
            {
                if (instance == null)
                {
                    throw new Exception("GameMessageSender is not initialized yet");
                }
                return instance;
            }
        }
        internal void SendSkillCast(SkillItem item)
        {
            if (target != null)
            {
                NetOutgoingMessage msgOut = messageCreater.CreateSkillCast(item, target);
                netClient.SendMessage(msgOut, NetDeliveryMethod.Unreliable);
            }
        }
        public void SendClientReady()
        {
            NetOutgoingMessage msgReady = messageCreater.ClientReady();
            netClient.SendMessage(msgReady, NetDeliveryMethod.ReliableOrdered);
        }
        public void SendPositionUpdate()
        {
            if (netClient.ServerConnection == null)
                return;
            NetOutgoingMessage msgOut = messageCreater.PositionUpdate(dataHandler.myCharacter.entity.id, dataHandler.myCharacter.transform.position);
            netClient.ServerConnection.SendMessage(msgOut, NetDeliveryMethod.ReliableOrdered, 1);
        }
        public void SendPrivateChatMessage(string[] msg)
        {
            NetOutgoingMessage msgOut = messageCreater.PrivateChatMessage(dataHandler.myCharacter.entity.characterName, msg);
            netClient.ServerConnection.SendMessage(msgOut, NetDeliveryMethod.ReliableOrdered, 1);
        }
        public void SendChatMessage(string msg)
        {
            NetOutgoingMessage msgOut = messageCreater.ChatMessage(dataHandler.myCharacter.entity.characterName, msg);
            netClient.ServerConnection.SendMessage(msgOut, NetDeliveryMethod.ReliableOrdered, 1);
        }
        public void SendAdminChatMessage(string msg)
        {
            NetOutgoingMessage msgOut = messageCreater.AdminChatMessage(msg);
            netClient.ServerConnection.SendMessage(msgOut, NetDeliveryMethod.ReliableOrdered, 1);
        }
        public void ConnectToGameServer(int id, byte[] authToken, string publicKey, string ip, int port)
        {
            NetOutgoingMessage msgOut = messageCreater.ConnectingMessage(id, authToken, publicKey);
            netClient.Connect(ip, port, msgOut);
            Debug.Log("CONNECTING TO AREA SERVER");
        }
        internal void SendDisconnect()
        {
            if (netClient.ServerConnection != null)
            {
                netClient.Disconnect("Exiting");
            }
        }

        internal void SendUseMessage(UIItem item)
        {
            NetOutgoingMessage msgOut = null;
            switch (item.ItemType)
            {
                case UIItemType.Skill:
                    msgOut = messageCreater.LevelUpSkill(item);
                    break;
                case UIItemType.Weapon:
                    msgOut = messageCreater.TakeOn(item);
                    break;
                case UIItemType.Armor:
                    msgOut = messageCreater.TakeOn(item);
                    break;
                case UIItemType.Potion:
                    msgOut = messageCreater.Use(item);
                    break;
                case UIItemType.Food:
                    msgOut = messageCreater.Use(item);
                    break;
            }
            if (msgOut != null)
                netClient.ServerConnection.SendMessage(msgOut, NetDeliveryMethod.ReliableOrdered, 1);
        }
    }
}
