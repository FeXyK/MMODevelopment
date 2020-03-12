using Assets.Scripts.Handlers;
using Assets.Scripts.WorldServerNetworkScripts;
using Lidgren.Network;
using Lidgren.Network.ServerFiles;
using Lidgren.Network.Wrapper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.VersionControl;
using UnityEngine;

namespace Assets.Scripts.LoginNetworkScripts
{
    class WorldServerNetwork : NetPeerOverride
    {
        SceneLoader sceneLoader;
        public new WorldServerMessageHandler messageHandler;
        public LoginDataHandler dataHandler;
        public override void Initialize(string source)
        {
            base.Initialize(source);
            if (sceneLoader == null)
                sceneLoader = GameObject.FindObjectOfType<SceneLoader>();
            dataHandler = LoginDataHandler.GetInstance();
            messageHandler = new WorldServerMessageHandler(netPeer as NetClient);
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
                    Debug.Log((MessageType)msgType);
                    switch (msgType)
                    {
                        case MessageType.CharacterData:
                            messageHandler.HandleCharacterData(msgIn);
                            break;
                        case MessageType.Notification:
                            messageHandler.HandleNotification(msgIn);
                            break;
                        case MessageType.NewAuthenticationToken:
                            messageHandler.HandleNewLoginToken(msgIn);
                            sceneLoader.LoadGameScene(1);
                            break;
                        case MessageType.ClientAuthenticated:
                            messageHandler.ClientAuthenticated(msgIn);
                            break;
                    }
                }
            }
        }
        public void SetupConnection(string SERVER_IP, int SERVER_PORT )
        {
            if ((netPeer as NetClient).ServerConnection != null)
                return;
            NetOutgoingMessage msgLogin = (netPeer as NetClient).CreateMessage();

            msgLogin.Write((byte)MessageType.KeyExchange);
            msgLogin.Write(DataEncryption.publicKey);
            (netPeer as NetClient).Connect(SERVER_IP, SERVER_PORT, msgLogin);
            NetIncomingMessage msgIn = null;
            (netPeer as NetClient).MessageReceivedEvent.WaitOne();
            while ((msgIn = (netPeer as NetClient).ReadMessage()) != null)
            {
                switch (msgIn.MessageType)
                {
                    case NetIncomingMessageType.StatusChanged:
                        switch ((NetConnectionStatus)msgIn.ReadByte())
                        {
                            case NetConnectionStatus.Connected:
                                (netPeer as NetClient).ServerConnection.RemoteHailMessage.ReadByte();
                                dataHandler.selectedWorldServer.publicKey = (netPeer as NetClient).ServerConnection.RemoteHailMessage.ReadString();
                                messageHandler.SendAuthenticationToken(msgIn);
                                break;
                            case NetConnectionStatus.Disconnected:
                                {
                                    string reason = msgIn.ReadString();
                                    if (string.IsNullOrEmpty(reason))
                                        Debug.Log("Disconnected\n");
                                    else
                                        Debug.Log("Disconnected, Reason: " + reason + "\n");
                                }
                                break;
                        }
                        break;
                }
            }
        }
    }
}
