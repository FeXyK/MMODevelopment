using Lidgren.Network.Wrapper;
using System;
using System.Collections.Generic;
using System.Text;
using Lidgren.Network.ServerFiles;
using Lidgren.Network;
using MMOLoginServer.ServerData;

namespace MMOLoginServer.LoginServerLogic
{
    public class LoginServerCore : NetPeerOverride
    {
        private List<ClientData> accounts = new List<ClientData>();
        private List<GameServerData> gameServers = new List<GameServerData>();
        ClientData currentAccount = null;
        MessageHandler messageHandler;
        BasicFunctions basicFunctions;

        string gameServerKey = "HARDCODEDKEY";

        public override void Initialize(string SERVER_NAME, int LOGIN_SERVER_PORT)
        {
            base.Initialize(SERVER_NAME, LOGIN_SERVER_PORT);
            basicFunctions = new BasicFunctions();
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
                    messageHandler.HandleConnectionApproval(msgIn, accounts);
                }
                else if (msgIn.MessageType == NetIncomingMessageType.Data)
                {
                    Debug.Log(msgIn.ToString());
                    msgType = (MessageType)msgIn.ReadByte();
                    Console.WriteLine((MessageType)msgType);

                    currentAccount = GetAccount(msgIn.SenderConnection);
                    switch (msgType)
                    {
                        case MessageType.ClientAuthentication:
                            messageHandler.HandleLoginMessage(msgIn, currentAccount, gameServers);
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
                            foreach (var gameServer in gameServers)
                            {
                                Console.WriteLine(gameServer.name);
                                if (gameServer.name == msgIn.ReadString())
                                {
                                    ClientData connection = GetAccount(msgIn.SenderConnection);
                                    string cname = msgIn.ReadString();
                                    Console.WriteLine("CHARACTERS: -------------------");
                                    Console.WriteLine(cname);
                                    foreach (var item in connection.characters)
                                    {
                                        Console.WriteLine(item.name);
                                    }
                                    Console.WriteLine("CLOSE");
                                    connection.characters.Clear();
                                    connection.characters = DatabaseSelection.instance.GetCharactersData(connection.id);
                                    CharacterData characterData = connection.characters.Find(x => x.name == cname);
                                    connection.authToken = basicFunctions.GenerateRandomSequence(40);

                                    NetOutgoingMessage msgOut = netPeer.CreateMessage();
                                    msgOut.Write((byte)MessageType.NewLoginToken);
                                    PacketHandler.WriteEncryptedByteArray(msgOut, connection.authToken, gameServer.publicKey);
                                    msgOut.Write(DateTime.Now.AddSeconds(120).ToShortTimeString());
                                    msgOut.Write(characterData.id, 16);
                                    msgOut.Write(characterData.level, 16);
                                    msgOut.Write(characterData.currentHealth, 16);
                                    msgOut.Write(characterData.characterType, 16);
                                    msgOut.Write(characterData.name);
                                    Console.WriteLine("STATUS:");
                                    Console.WriteLine((NetConnectionStatus)gameServer.connection.Status);
                                    gameServer.connection.SendMessage(msgOut, NetDeliveryMethod.ReliableOrdered, 1);


                                    msgOut = netPeer.CreateMessage();
                                    msgOut.Write((byte)MessageType.NewLoginToken);
                                    PacketHandler.WriteEncryptedByteArray(msgOut, connection.authToken, connection.publicKey);
                                    PacketHandler.WriteEncryptedByteArray(msgOut, DateTime.Now.AddSeconds(120).ToShortTimeString(), connection.publicKey);
                                    PacketHandler.WriteEncryptedByteArray(msgOut, msgIn.ReadString(), connection.publicKey);
                                    msgIn.SenderConnection.SendMessage(msgOut, NetDeliveryMethod.ReliableOrdered, 1);

                                    Console.WriteLine(DateTime.Now.AddSeconds(120).ToShortTimeString());
                                }
                                break;
                            }
                            break;
                    }
                }
                netPeer.Recycle(msgIn);
            }
        }
        public void ConnectToGameServerList(List<ConnectionData> netConnections)
        {
            foreach (var connData in netConnections)
            {
                ConnectToGameServer(connData);
            }
        }
        private void ConnectToGameServer(ConnectionData connData)
        {
            NetIncomingMessage msgIn = null;
            GameServerData gameServerData = new GameServerData();

            NetOutgoingMessage msgConnect = base.messageHandler.CreateRSAKeyMessage(netPeer, ConnectionType.LoginServer);
            netPeer.Connect(connData.ip, connData.port, msgConnect);

            netPeer.MessageReceivedEvent.WaitOne();
            while ((msgIn = netPeer.ReadMessage()) != null)
            {
                switch (msgIn.MessageType)
                {
                    case NetIncomingMessageType.StatusChanged:
                        switch ((NetConnectionStatus)msgIn.ReadByte())
                        {
                            case NetConnectionStatus.Connected:
                                gameServerData.connection = msgIn.SenderConnection;
                                if ((MessageType)msgIn.SenderConnection.RemoteHailMessage.ReadByte() == MessageType.KeyExchange)
                                {
                                    gameServerData.type = (ConnectionType)msgIn.SenderConnection.RemoteHailMessage.ReadByte();
                                    gameServerData.publicKey = msgIn.SenderConnection.RemoteHailMessage.ReadString();
                                    gameServerData.name = msgIn.SenderConnection.RemoteHailMessage.ReadString();

                                    gameServers.Add(gameServerData);
                                    Console.WriteLine("GameServer: " + gameServerData.name + "\nKey: " + gameServerData.publicKey);


                                    NetOutgoingMessage msgOut = netPeer.CreateMessage();
                                    msgOut.Write((byte)MessageType.LoginServerAuthentication);
                                    PacketHandler.WriteEncryptedByteArray(msgOut, gameServerKey, gameServerData.publicKey);
                                    gameServerData.connection.SendMessage(msgOut, NetDeliveryMethod.ReliableOrdered, 1);
                                }
                                break;
                            case NetConnectionStatus.Disconnected:
                                {
                                    string reason = msgIn.ReadString();
                                    if (string.IsNullOrEmpty(reason))
                                        Console.WriteLine("Disconnected\n");
                                    else
                                        Console.WriteLine("Disconnected, Reason: " + reason + "\n");
                                }
                                break;
                        }
                        break;
                }
            }
        }
        public override void Update()
        {
        }
        private ClientData GetAccount(NetConnection connection)
        {
            foreach (var acc in accounts)
            {
                if (connection == acc.connection)
                {
                    return acc;
                }
            }
            return null;
        }
    }
}
