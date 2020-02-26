using Lidgren.Network;
using Lidgren.Network.ServerFiles;
using Lidgren.Network.Wrapper;
using System;

namespace MMOGameServer
{
    class GameServerCore : NetPeerOverride
    {
       new MessageHandler messageHandler = null;

        public override void Initialize(string PEER_NAME, int PEER_PORT = 0)
        {
            base.Initialize(PEER_NAME, PEER_PORT);
            messageHandler = new MessageHandler(netPeer as NetServer);
        }
        public override void ReceiveMessages()
        {
            NetIncomingMessage msgIn;
            while ((msgIn = netPeer.ReadMessage()) != null)
            {
                if (msgIn.MessageType == NetIncomingMessageType.ConnectionApproval)
                {
                    Console.WriteLine("New Connection: {0}, {1}, {2}", msgIn.ReceiveTime, msgIn.SenderEndPoint.Address, msgIn.SenderEndPoint.Port);
                    MessageType msgType = (MessageType)msgIn.ReadByte();
                    Console.WriteLine((MessageType)msgType);
                    switch (msgType)
                    {
                        case MessageType.KeyExchange:
                            messageHandler.KeyExchange(msgIn);
                            break;
                        case MessageType.ClientAuthentication:
                            messageHandler.ClientAuthentication(msgIn);
                            break;
                    }
                }
                else if (msgIn.MessageType == NetIncomingMessageType.Data)
                {
                    MessageType msgType = (MessageType)msgIn.ReadByte();
                    Console.WriteLine((MessageType)msgType);
                    switch (msgType)
                    {
                        case MessageType.NewLoginToken:
                            messageHandler.NewLoginToken(msgIn);
                            break;
                        case MessageType.LoginServerAuthentication:
                            messageHandler.LoginServerAuthentication(msgIn);
                            break;
                        ///GameLogic
                        ///
                        case MessageType.CharacterMovement:
                            messageHandler.CharacterMovementData(msgIn);
                            break;
                        case MessageType.ClientReady:
                            messageHandler.ClientReady(msgIn);
                            break;
                    }
                }
                netPeer.Recycle(msgIn);
            }
        }
        public override void Update()
        {
            messageHandler.SendMessages();
            //RemoveExpiredLoginTokens(); //TODO
        }
    }
}
