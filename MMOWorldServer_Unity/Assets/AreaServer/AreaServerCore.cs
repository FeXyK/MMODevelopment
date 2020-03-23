﻿using Lidgren.Network;
using Lidgren.Network.ServerFiles;
using Lidgren.Network.Wrapper;
using MMOLoginServer.ServerData;
using System;
using System.Collections.Concurrent;
using UnityEngine;

namespace MMOGameServer
{
    class AreaServerCore : NetPeerOverride
    {
        new AreaMessageHandler messageHandler = null;
        public ConcurrentQueue<CharacterData> newConnections = new ConcurrentQueue<CharacterData>();
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
                        case MessageType.StartSkillCast:
                            messageHandler.StartSkillCast(msgIn);
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
    }
}
