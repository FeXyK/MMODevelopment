﻿using Lidgren.Network;
using Lidgren.Network.Message;
using Lidgren.Network.Override;
using Assets.Scripts.Handlers;
using UnityEngine;

namespace Assets.Scripts.LoginNetworkScripts
{
    public class LoginClientManager : NetPeerOverride
    {
        public SceneLoader sceneLoader;
        new LoginMessageHandler messageHandler;

        public override void Initialize(string source)
        {
            base.Initialize(source);
            messageHandler = new LoginMessageHandler(netPeer as NetClient);
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
                        case MessageType.ServerLoginSuccess:
                            messageHandler.HandleSuccessfullLogin();
                            messageHandler.HandleNotification(msgIn);
                            break;
                        case MessageType.Notification:
                            messageHandler.HandleNotification(msgIn);
                            break;
                        case MessageType.GameServersData:
                            messageHandler.HandleGameServerData(msgIn);
                            break;
                        case MessageType.NewAuthenticationToken:
                            messageHandler.HandleNewLoginToken(msgIn);
                            break;
                    }
                }
            }
        }
    }
}
