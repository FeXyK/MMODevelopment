using Lidgren.Network;
using Lidgren.Network.ServerFiles;
using Lidgren.Network.ServerFiles.Data;
using Lidgren.Network.Wrapper;
using MMOLoginServer.ServerData;
using System;
using System.Collections.Generic;
using System.Text;

namespace MMOGameServer
{
    class GameServerCore : NetPeerOverride
    {
        string serverName = "Europe";

        ClientMessageHandler clientMessage = null;
        LoginServerMessageHandler loginServerMessage = null;

        CharacterData currentAccount = null;
        LoginServerData loginServer = null;

        List<LoginTokenData> loginTokens = new List<LoginTokenData>();
        Dictionary<int, CharacterData> characters = new Dictionary<int, CharacterData>();
        List<NetConnection> netConnections = new List<NetConnection>();
        public override void ReceiveMessages()
        {
            NetIncomingMessage msgIn;
            MessageType msgType;
            while ((msgIn = netPeer.ReadMessage()) != null)
            {
                //Console.WriteLine(msgIn);
                if (msgIn.MessageType == NetIncomingMessageType.ConnectionApproval)
                {
                    Console.WriteLine("New Connection: {0}, {1}, {2}", msgIn.ReceiveTime, msgIn.SenderEndPoint.Address, msgIn.SenderEndPoint.Port);

                    msgType = (MessageType)msgIn.ReadByte();
                    Console.WriteLine(msgType);

                    if (msgType == MessageType.KeyExchange)
                    {
                        ConnectionData connection = new ConnectionData();
                        connection.port = msgIn.SenderEndPoint.Port;
                        connection.ip = msgIn.SenderEndPoint.Address.ToString();
                        connection.connection = msgIn.SenderConnection;
                        connection.type = (ConnectionType)msgIn.ReadByte();
                        connection.publicKey = msgIn.ReadString();
                        connections.Add(connection);
                        NetOutgoingMessage msgOut = messageHandler.CreateRSAKeyMessage(netPeer, ConnectionType.GameServer, serverName);
                        msgIn.SenderConnection.Approve(msgOut);
                    }
                    if (msgType == MessageType.ClientAuthentication)
                    {
                        byte[] clientLoginToken = PacketHandler.ReadEncryptedByteArray(msgIn);
                        string username = Encoding.UTF8.GetString(PacketHandler.ReadEncryptedByteArray(msgIn));
                        Console.WriteLine("Token length: " + clientLoginToken.Length);
                        Console.WriteLine(BitConverter.ToString(clientLoginToken));
                        Console.WriteLine(username);
                        LoginTokenData validToken = null;
                        if ((validToken = CheckLoginToken(clientLoginToken, username)) != null)
                        {
                            msgIn.SenderConnection.Approve();

                            CharacterData characterData = new CharacterData();
                            characterData.connection = msgIn.SenderConnection;
                            characterData.id = validToken.characterData.id;
                            characterData.name = validToken.characterData.name;
                            characterData.currentHealth = 100;
                            characterData.position = new System.Numerics.Vector3(0, 0, 0);
                            characterData.rotation = 0;



                            characters.Add(characterData.id, characterData);
                            netConnections.Add(msgIn.SenderConnection);
                            Console.WriteLine("Authenticated!");
                        }
                        else
                        {
                            Console.WriteLine("Bad AuthenticationKey");
                        }
                    }
                }
                else if (msgIn.MessageType == NetIncomingMessageType.Data)
                {
                    msgType = (MessageType)msgIn.ReadByte();
                    switch (msgType)
                    {
                        case MessageType.NewLoginToken:
                            Console.WriteLine((MessageType)msgType);

                            LoginTokenData newLoginToken = new LoginTokenData();
                            newLoginToken.characterData = new CharacterData();

                            newLoginToken.token = PacketHandler.ReadEncryptedByteArray(msgIn);
                            newLoginToken.expireDate = msgIn.ReadString();
                            newLoginToken.characterData.id = msgIn.ReadInt16();
                            newLoginToken.characterData.level = msgIn.ReadInt16();
                            newLoginToken.characterData.currentHealth = msgIn.ReadInt16();
                            newLoginToken.characterData.characterType = msgIn.ReadInt16();
                            newLoginToken.characterData.name = msgIn.ReadString();

                            Console.WriteLine("TOKEN VALIDATION:");
                            Console.WriteLine(newLoginToken.expireDate);
                            Console.WriteLine(newLoginToken.characterName);
                            Console.WriteLine(BitConverter.ToString(newLoginToken.token));

                            loginTokens.Add(newLoginToken);
                            break;
                        case MessageType.LoginServerAuthentication:
                            Console.WriteLine((MessageType)msgType);

                            byte[] encoded = PacketHandler.ReadEncryptedByteArray(msgIn);
                            if (Encoding.UTF8.GetString(encoded) == "HARDCODEDKEY")
                            {
                                loginServer = FindConnection(msgIn.SenderConnection) as LoginServerData;
                                Console.WriteLine("LoginServerConnectionApproved");
                            }
                            else
                            {
                                msgIn.SenderConnection.Disconnect("Bad AuthenticationKey");
                            }
                            break;

                        ///GameLogic
                        ///
                        case MessageType.CharacterMovement:
                            int id = msgIn.ReadInt16();
                            float x = msgIn.ReadFloat();
                            float z = msgIn.ReadFloat();
                            float r = msgIn.ReadFloat();
                            characters[id].position = new System.Numerics.Vector3(x, 0, z);
                            characters[id].rotation = r;
                            break;
                        case MessageType.ClientReady:
                            Console.WriteLine((MessageType)msgType);

                            CharacterData characterData = GetCharacter(msgIn);

                            //To already ingame players
                            NetOutgoingMessage msgOut = netPeer.CreateMessage();
                            msgOut.Write((byte)MessageType.NewCharacter);
                            msgOut.Write(characterData.id, 16);
                            msgOut.Write(characterData.level, 16);
                            msgOut.Write(characterData.currentHealth, 16);
                            msgOut.Write(characterData.characterType, 16);
                            msgOut.Write(characterData.name);

                            (netPeer as NetServer).SendMessage(msgOut, netConnections, NetDeliveryMethod.ReliableOrdered, 1);
                            
                            //To new connected player
                            foreach (var character in characters)
                            {
                                if (character.Value.connection != msgIn.SenderConnection)
                                {
                                    msgOut = netPeer.CreateMessage();
                                    msgOut.Write((byte)MessageType.NewCharacter);
                                    msgOut.Write(character.Value.id, 16);
                                    msgOut.Write(character.Value.level, 16);
                                    msgOut.Write(character.Value.currentHealth, 16);
                                    msgOut.Write(character.Value.characterType, 16);
                                    msgOut.Write(character.Value.name);
                                    msgIn.SenderConnection.SendMessage(msgOut, NetDeliveryMethod.ReliableOrdered, 1);
                                }
                            }
                            break;
                    }
                    netPeer.Recycle(msgIn);
                }
            }
        }
        public CharacterData GetCharacter(NetIncomingMessage msgIn)
        {
            foreach (var character in characters)
            {
                if (msgIn.SenderConnection == character.Value.connection)
                {
                    return character.Value;
                }
            }
            return null;
        }
        public override void Update()
        {
            SendMessages();
            //RemoveExpiredLoginTokens(); //TODO
        }
        public void RemoveExpiredLoginTokens()
        {
            LoginTokenData remove = null;
            foreach (var token in loginTokens)
            {
                if (DateTime.Parse(token.expireDate) < DateTime.Now)
                {
                    remove = token;
                    break;
                }
            }
            if (remove != null)
                loginTokens.Remove(remove);
        }
        public void SendMessages()
        {
            NetOutgoingMessage msgOut;
            foreach (var character in characters)
            {
                msgOut = netPeer.CreateMessage();
                msgOut.Write((byte)MessageType.CharacterMovement);
                msgOut.Write(character.Value.id, 16);
                msgOut.Write(character.Value.position.X);
                msgOut.Write(character.Value.position.Z);
                msgOut.Write(character.Value.rotation);
                (netPeer as NetServer).SendMessage(msgOut, netConnections, NetDeliveryMethod.UnreliableSequenced, 1);
            }
        }
        private ConnectionData FindConnection(NetConnection senderConnection)
        {
            foreach (var connection in connections)
            {
                if (connection.connection == senderConnection)
                {
                    return connection;
                }
            }
            return null;
        }

        private LoginTokenData CheckLoginToken(byte[] clientLoginToken, string username)
        {
            foreach (var token in loginTokens)
            {
                Console.WriteLine("AUTH TOKENS: ");
                Console.WriteLine(BitConverter.ToString(token.token));
                Console.WriteLine(BitConverter.ToString(clientLoginToken));
                Console.WriteLine(username + " END");
                Console.WriteLine(token.characterData.name + " END");
                Console.WriteLine("---------------------");


                if (ByteArrayCompare(token.token, clientLoginToken)//token.expireDate > DateTime.Now &&
                                                                   //token.token == clientLoginToken
                    && token.characterData.name == username)
                {
                    Console.WriteLine("ClientAuthenticated");
                    return token;
                }
            }
            return null;
        }

        [System.Runtime.InteropServices.DllImport("msvcrt.dll", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        static extern int memcmp(byte[] b1, byte[] b2, long count);

        static bool ByteArrayCompare(byte[] b1, byte[] b2)
        {
            // Validate buffers are the same length.
            // This also ensures that the count does not exceed the length of either buffer.  
            return b1.Length == b2.Length && memcmp(b1, b2, b1.Length) == 0;
        }
    }
}
