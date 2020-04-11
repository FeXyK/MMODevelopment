using Assets;
using Assets.AreaServer.Entity;
using Lidgren.Network;
using Lidgren.Network.Message;
using Lidgren.Network.Override;
using Lidgren.Network.ServerFiles.Data;
using System;
using System.Collections.Concurrent;
using UnityEngine;

namespace MMOGameServer
{
    class AreaServerCore : NetPeerOverride
    {
       public new AreaMessageHandler messageHandler = null;
        public ConcurrentQueue<CharacterWrapper> newConnections = new ConcurrentQueue<CharacterWrapper>();
        int tickCount = 0;

        public override void Initialize(string source)
        {
            base.Initialize(source);
            messageHandler = new AreaMessageHandler(netPeer as NetServer, newConnections);
            Console.WriteLine("Area Server Initialized");
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
                            //messageHandler.KeyExchange(msgIn);
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
                        case MessageType.LoginServerAuthentication:
                            Console.WriteLine((MessageType)msgType);
                            //messageHandler.LoginServerAuthentication(msgIn);
                            break;
                        ///GameLogic
                        ///
                        case MessageType.CharacterMovement:
                            messageHandler.CharacterMovementData(msgIn);
                            break;
                        case MessageType.ClientReady:
                            Console.WriteLine((MessageType)msgType);
                            messageHandler.ClientReady(msgIn);
                            break;
                        case MessageType.SkillLeveled:
                            messageHandler.SkillLeveled(msgIn);
                            break;
                        case MessageType.StartSkillCast:
                            messageHandler.StartSkillCast(msgIn);
                            break;
                        case MessageType.UsePotion:
                            messageHandler.UsePotion(msgIn);
                            break;
                        case MessageType.SkillListInformation:
                            messageHandler.ClientSkillInformation(msgIn);
                            break;
                    }
                }
                netPeer.Recycle(msgIn);
            }
        }
        public override void Update()
        {
            messageHandler.GetNewConnections();
            messageHandler.SendMovementMessages();
            messageHandler.SendMobPositions();
            if (tickCount > 1000)
            {
                Console.WriteLine("Remove connections");
                Console.WriteLine("Count: "+netPeer.Connections.Count);
                messageHandler.ClearConnections();
                
                tickCount = 0;
            }
            tickCount++;
        }

        internal Character GetCharacter(int characterID)
        {
            return messageHandler.GetCharacter(characterID);   
        }
    }
}
