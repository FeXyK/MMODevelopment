using Assets.Scripts.Handlers;
using Lidgren.Network;
using Lidgren.Network.ServerFiles;
using Lidgren.Network.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.LoginNetworkScripts
{
    public class LoginClientManager : NetPeerOverride
    {
        //public LoginScreenInputData input;
        public SceneLoader sceneLoader;
        new LoginMessageHandler messageHandler;



        public LoginClientManager()
        {
            Initialize("NetLidgren");
        }
        public override void Initialize(string PEER_NAME, int PEER_PORT = 0, bool IS_SERVER = false)
        {
                base.Initialize(PEER_NAME, PEER_PORT, IS_SERVER);

            //if (input == null)
            //    input = GameObject.FindObjectOfType<LoginScreenInputData>();
            //input.netClient = (NetClient)netPeer;
            if (sceneLoader == null)
                sceneLoader = GameObject.FindObjectOfType<SceneLoader>();
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
                        case MessageType.CharacterData:
                            messageHandler.HandleCharacterData(msgIn);
                            break;
                        case MessageType.ServerLoginSuccess:
                            messageHandler.HandleSuccessfullLogin();
                            messageHandler.HandleNotification(msgIn);
                            break;
                        case MessageType.Notification:
                            messageHandler.HandleNotification(msgIn);
                            break;
                        case MessageType.AuthToken:
                            messageHandler.HandleAuthenticationToken(msgIn);
                            break;
                        case MessageType.GameServersData:
                            messageHandler.HandleGameServerData(msgIn);
                            break;
                        case MessageType.NewLoginToken:
                            messageHandler.HandleNewLoginToken(msgIn);
                            sceneLoader.LoadGameScene(1);
                            break;
                    }
                }
            }
        }

        
    }

}
