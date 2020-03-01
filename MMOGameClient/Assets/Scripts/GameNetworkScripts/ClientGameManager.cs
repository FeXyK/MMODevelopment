using Assets.Scripts.Handlers;
using Assets.Scripts.LoginScreen;
using Lidgren.Network;
using Lidgren.Network.ServerFiles;
using Lidgren.Network.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.GameNetworkScripts
{
    public class ClientGameManager : NetPeerOverride
    {
        GameObject[] gameObjects;
        public new GameMessageHandler messageHandler;
        public override void Initialize(string PEER_NAME, int PEER_PORT = 0, bool IS_SERVER = false)
        {
            base.Initialize(PEER_NAME, PEER_PORT, IS_SERVER);

            messageHandler = new GameMessageHandler(netPeer as NetClient);

            ConnectToGameServer();
        }
        public override void ReceiveMessages()
        {
            NetIncomingMessage msgIn;
            MessageType msgType;
            while ((msgIn = netPeer.ReadMessage()) != null)
            {
                if (msgIn.MessageType == NetIncomingMessageType.Data)
                {
                    msgType = (MessageType)msgIn.ReadByte();
                    switch (msgType)
                    {
                        case MessageType.Notification:
                            messageHandler.PrintFeedBack(msgIn);
                            break;
                        case MessageType.NewCharacter:
                            Debug.Log(msgType);
                            messageHandler.HandleNewCharacter(msgIn);
                            break;
                        case MessageType.CharacterMovement:
                            messageHandler.HandePositionUpdate(msgIn);
                            break;
                        case MessageType.OtherCharacterRemove:
                            messageHandler.HandleCharacterRemove(msgIn);
                            break;
                        case MessageType.AdminChatMessage:
                            messageHandler.HandleAdminCommand(msgIn);
                            break;
                        case MessageType.PrivateChatMessage:
                            messageHandler.HandleChatMessage(msgIn, "PM");
                            break;
                        case MessageType.PublicChatMessage:
                            messageHandler.HandleChatMessage(msgIn);
                            break;

                    }
                }
                if (msgIn.MessageType == NetIncomingMessageType.StatusChanged)
                {
                    NetConnectionStatus msgStat = (NetConnectionStatus)msgIn.ReadByte();
                    switch (msgStat)
                    {
                        case NetConnectionStatus.Connected:
                            messageHandler.SendClientReady();
                            Debug.Log("Connected to GameServer");
                            break;
                        case NetConnectionStatus.Disconnected:
                            Debug.Log("Disconnecting from LoginServer");
                            break;
                    }
                }
            }
        }

        private void ConnectToGameServer()
        {
            NetOutgoingMessage msgOut = netPeer.CreateMessage();
            msgOut.Write((byte)MessageType.ClientAuthentication);
            SelectionController selection = GameObject.FindObjectOfType<SelectionController>();
            Debug.Log(selection.loginDataController.authToken);
            Debug.Log(selection.loginDataController.publicKey);
            PacketHandler.WriteEncryptedByteArray(msgOut, selection.loginDataController.authToken, selection.loginDataController.publicKey);
            PacketHandler.WriteEncryptedByteArray(msgOut, selection.loginDataController.characterName, selection.loginDataController.publicKey);
            Debug.Log(selection.loginDataController.serverIP);
            Debug.Log(selection.loginDataController.serverPort);
            netPeer.Connect(selection.loginDataController.serverIP, selection.loginDataController.serverPort, msgOut);
        }
        public override void Update()
        {
            ReceiveMessages();
            messageHandler.SendPositionUpdate();
        }
        public void HideNameTags()
        {
            NetOutgoingMessage msgOut = netPeer.CreateMessage();
            msgOut.Write((byte)MessageType.HideNames);
            (netPeer as NetClient).ServerConnection.SendMessage(msgOut, NetDeliveryMethod.ReliableOrdered, 1);
        }
    }
}
