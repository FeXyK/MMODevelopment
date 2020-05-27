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


        internal void SendPickUpMessage(int transactionID)
        {
            NetOutgoingMessage msgOut = messageCreater.CreatePickUpMessage(transactionID);
            netClient.SendMessage(msgOut, NetDeliveryMethod.Unreliable);
            Debug.LogWarning("tID: " + transactionID);
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

        internal void SkillLevelUp(UIItem item)
        {
            if (item.ID >= 0)
            {
                NetOutgoingMessage msgOut = messageCreater.SkillLevelUp(item);
                netClient.ServerConnection.SendMessage(msgOut, NetDeliveryMethod.ReliableOrdered, 1);
            }
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

        internal void SendUseMessage(UIContainer container)
        {
            NetOutgoingMessage msgOut = null;
            switch (container.Item.ItemType)
            {
                case EItemType.Skill:
                    msgOut = messageCreater.SkillLevelUp(container.Item);
                    break;
                case EItemType.Weapon:
                    msgOut = messageCreater.Equip(container);
                    break;
                case EItemType.Armor:
                    msgOut = messageCreater.Equip(container);
                    break;
                case EItemType.Potion:
                    msgOut = messageCreater.UsePotion(container.Item);
                    break;
                case EItemType.Food:
                    msgOut = messageCreater.UseFood(container.Item);
                    break;
            }
            if (msgOut != null)
                netClient.ServerConnection.SendMessage(msgOut, NetDeliveryMethod.ReliableOrdered, 1);
        }
        internal void Unequip(UIContainer container)
        {
            Debug.Log(container.SlotID);
            NetOutgoingMessage msgOut = messageCreater.Unequip(container);

            netClient.ServerConnection.SendMessage(msgOut, NetDeliveryMethod.ReliableOrdered, 1);
        }
        internal void Equip(UIContainer container)
        {
            NetOutgoingMessage msgOut = messageCreater.Equip(container);

            netClient.ServerConnection.SendMessage(msgOut, NetDeliveryMethod.ReliableOrdered, 1);
        }
    }
}
