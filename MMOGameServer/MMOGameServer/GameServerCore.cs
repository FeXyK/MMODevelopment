using Lidgren.Network;
using Lidgren.Network.ServerFiles;
using Lidgren.Network.Wrapper;
using System;

namespace MMOGameServer
{
    class GameServerCore : NetPeerOverride
    {
        new MessageHandler messageHandler = null;
        int tickCount = 0;
        public override void Initialize(string PEER_NAME, int PEER_PORT = 0, bool IS_SERVER = true, bool simulateLatency = true)
        {
            base.Initialize(PEER_NAME, PEER_PORT, IS_SERVER, simulateLatency);
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
                    switch (msgType)
                    {
                        case MessageType.NewLoginToken:
                            Console.WriteLine((MessageType)msgType);
                            messageHandler.NewLoginToken(msgIn);
                            break;
                        case MessageType.LoginServerAuthentication:
                            Console.WriteLine((MessageType)msgType);
                            messageHandler.LoginServerAuthentication(msgIn);
                            break;
                        ///GameLogic
                        ///
                        case MessageType.CharacterMovement:
                            Debug.Log((MessageType)msgType);
                            messageHandler.CharacterMovementData(msgIn);
                            break;
                        case MessageType.ClientReady:
                            Console.WriteLine((MessageType)msgType);
                            messageHandler.ClientReady(msgIn);
                            break;
                        case MessageType.HideNames:
                            break;
                        case MessageType.PublicChatMessage:

                            messageHandler.SendPublicChatMessage(msgIn);
                            break;
                        case MessageType.PrivateChatMessage:

                            messageHandler.SendPrivateChatMessage(msgIn);
                            break;
                        case MessageType.AdminChatMessage:

                            messageHandler.HandleAdminMessage(msgIn);
                            break;

                    }
                }
                netPeer.Recycle(msgIn);
            }
        }
        public override void Update()
        {
            messageHandler.SendMovementMessages();
            if (tickCount > 50000)
            {
                messageHandler.ClearConnections();
                tickCount = 0;
                //RemoveExpiredLoginTokens(); //TODO
            }
            tickCount++;
        }
    }
}
