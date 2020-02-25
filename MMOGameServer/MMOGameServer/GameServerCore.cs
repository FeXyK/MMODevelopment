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
        List<CharacterData> accounts = new List<CharacterData>();

        public override void ReceiveMessages()
        {
            NetIncomingMessage msgIn;
            MessageType msgType;
            while ((msgIn = netPeer.ReadMessage()) != null)
            {
                Console.WriteLine(msgIn);
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

                        NetOutgoingMessage msgOut = messageHandler.CreateRSAKeyMessage(netPeer, ConnectionType.GameServer, serverName);
                        msgIn.SenderConnection.Approve(msgOut);
                    }
                    if (msgType == MessageType.ClientAuthentication)
                    {
                        Console.WriteLine(msgType);
                        byte[] clientLoginToken = PacketHandler.ReadEncryptedByteArray(msgIn);
                        string username = Encoding.UTF8.GetString(PacketHandler.ReadEncryptedByteArray(msgIn));
                        Console.WriteLine(clientLoginToken);
                        Console.WriteLine(username);

                        if (CheckLoginToken(clientLoginToken, username))
                        {
                            CharacterData characterData = new CharacterData();
                            Console.WriteLine("Authenticated!");
                        }
                        else
                        {
                            connections.Remove(FindConnection(msgIn.SenderConnection));
                            msgIn.SenderConnection.Disconnect("Bad AuthenticationKey");
                        }
                    }
                }
                else if (msgIn.MessageType == NetIncomingMessageType.Data)
                {
                    msgType = (MessageType)msgIn.ReadByte();
                    Console.WriteLine((MessageType)msgType);
                    switch (msgType)
                    {
                        case MessageType.NewLoginToken:
                            LoginTokenData newLoginToken = new LoginTokenData();
                            newLoginToken.token = PacketHandler.ReadEncryptedByteArray(msgIn);
                            newLoginToken.expireDate = Encoding.UTF8.GetString(PacketHandler.ReadEncryptedByteArray(msgIn));
                            newLoginToken.username = Encoding.UTF8.GetString(PacketHandler.ReadEncryptedByteArray(msgIn));

                            Console.WriteLine("TOKEN VALIDATION:");
                            Console.WriteLine(newLoginToken.expireDate);
                            Console.WriteLine(newLoginToken.username);
                            Console.WriteLine(BitConverter.ToString(newLoginToken.token));

                            loginTokens.Add(newLoginToken);
                            break;
                        case MessageType.LoginServerAuthentication:
                            byte[] encoded = PacketHandler.ReadEncryptedByteArray(msgIn);
                            if (Encoding.UTF8.GetString(encoded) == "HARDCODEDKEY")
                            {
                                loginServer = (LoginServerData)FindConnection(msgIn.SenderConnection);
                                connections.Remove((ConnectionData)loginServer);
                                Console.WriteLine("LoginServerConnectionApproved");
                            }
                            else
                            {
                                msgIn.SenderConnection.Disconnect("Bad AuthenticationKey");
                            }
                            break;
                    }
                    netPeer.Recycle(msgIn);
                }
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

        private bool CheckLoginToken(byte[] clientLoginToken, string username)
        {
            foreach (var token in loginTokens)
            {
                if (//token.expireDate > DateTime.Now &&
                     token.token == clientLoginToken
                    && token.username == username)
                {
                    Console.WriteLine("ClientAuthenticated");
                    return true;
                }
            }
            return false;
        }
    }
}
