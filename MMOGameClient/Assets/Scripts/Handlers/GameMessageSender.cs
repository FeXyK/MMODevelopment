using Assets.Scripts.SkillSystem;
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
        internal void SendSkillCast(SkillItem skill, Entity target)
        {
            if (target != null)
            {
                NetOutgoingMessage msgOut = messageCreater.CreateSkillCast(skill, target);
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
            NetOutgoingMessage msgOut = messageCreater.PositionUpdate(dataHandler.myCharacter.id, dataHandler.myCharacter.transform.position);
            netClient.ServerConnection.SendMessage(msgOut, NetDeliveryMethod.ReliableOrdered, 1);
        }
        public void SendPrivateChatMessage(string[] msg)
        {
            NetOutgoingMessage msgOut = messageCreater.PrivateChatMessage(dataHandler.myCharacter.characterName, msg);
            netClient.ServerConnection.SendMessage(msgOut, NetDeliveryMethod.ReliableOrdered, 1);
        }
        public void SendChatMessage(string msg)
        {
            NetOutgoingMessage msgOut = messageCreater.ChatMessage(dataHandler.myCharacter.characterName, msg);
            netClient.ServerConnection.SendMessage(msgOut, NetDeliveryMethod.ReliableOrdered, 1);
        }
        public void SendAdminChatMessage(string msg)
        {
            NetOutgoingMessage msgOut =messageCreater. AdminChatMessage(msg);
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
            if(netClient .ServerConnection != null)
            {
                netClient.Disconnect("Exiting");
            }
        }
    }
}
