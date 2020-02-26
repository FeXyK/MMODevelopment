using Lidgren.Network.Wrapper;
using System;
using System.Collections.Generic;
using Lidgren.Network.ServerFiles;
using Lidgren.Network;

namespace MMOLoginServer.LoginServerLogic
{
    public class LoginServerCore : NetPeerOverride
    {
        ClientData currentAccount = null;
        new MessageHandler messageHandler;

        public override void Initialize(string SERVER_NAME, int LOGIN_SERVER_PORT)
        {
            base.Initialize(SERVER_NAME, LOGIN_SERVER_PORT);
            messageHandler = new MessageHandler((NetServer)netPeer);
        }
        public override void ReceiveMessages()
        {
            NetIncomingMessage msgIn;
            MessageType msgType;
            while ((msgIn = netPeer.ReadMessage()) != null)
            {
                Console.WriteLine(msgIn);
                if (msgIn.MessageType == NetIncomingMessageType.ConnectionApproval)
                {
                    messageHandler.HandleConnectionApproval(msgIn);
                }
                else if (msgIn.MessageType == NetIncomingMessageType.Data)
                {
                    Debug.Log(msgIn.ToString());
                    msgType = (MessageType)msgIn.ReadByte();
                    Console.WriteLine((MessageType)msgType);

                    currentAccount = DataHandler.Instance.GetAccount(msgIn.SenderConnection);
                    switch (msgType)
                    {
                        case MessageType.ClientAuthentication:
                            messageHandler.HandleLoginMessage(msgIn, currentAccount);
                            break;
                        case MessageType.RegisterRequest:
                            messageHandler.HandleRegisterMessage(msgIn, currentAccount);
                            break;
                        case MessageType.CreateCharacter:
                            messageHandler.HandleCreateMessage(msgIn, currentAccount);
                            break;
                        case MessageType.DeleteCharacter:
                            messageHandler.HandleDeleteMessage(msgIn, currentAccount);
                            break;
                        case MessageType.CharacterLogin:
                            messageHandler.CharacterLogin(msgIn, currentAccount);
                            break;
                    }
                }
                netPeer.Recycle(msgIn);
            }
        }
        public override void Update()
        {
        }
        public void ConnectToGameServerList(List<ConnectionData> netConnections)
        {
            foreach (var connData in netConnections)
            {
                messageHandler.ConnectToGameServer(connData);
            }
        }
    }
}
