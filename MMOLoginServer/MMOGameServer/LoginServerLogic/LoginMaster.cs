using Lidgren.Network;
using Lidgren.Network.ServerFiles;
using MMOLoginServer.ServerData;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading;


namespace MMOLoginServer.LoginServerLogic
{
    class LoginMaster
    {
        private static NetServer netServer;
        private static NetPeerConfiguration netPeerConfiguration;

        private List<Account> accounts = new List<Account>();
        private List<GameServer> gameServers = new List<GameServer>();
        Account currentAccount = null;
        ClientMessageHandler clientMessage = null;
        GameServerMessageHandler gameServerMessage = null;

        public void InitializeLoginServer(string SERVER_NAME, int LOGIN_SERVER_PORT)
        {
            netPeerConfiguration = new NetPeerConfiguration(SERVER_NAME);
            netPeerConfiguration.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
            netPeerConfiguration.Port = LOGIN_SERVER_PORT;

            netServer = new NetServer(netPeerConfiguration);
        }

        public void StartLoginServer(int LOGIN_SERVER_FRAMERATE)
        {
            clientMessage = new ClientMessageHandler(netServer);
            gameServerMessage = new GameServerMessageHandler(netServer);
            netServer.Start();

            Console.WriteLine("Listening Port: " + netServer.Port);

            MainLoop(LOGIN_SERVER_FRAMERATE);

            netServer.Shutdown("bye");

            Console.WriteLine("Server Offline");
            Console.ReadLine();
        }
        private void MainLoop(int desiredFPS)
        {
            while (true)
            {
                ReceiveMessages();

                Thread.Sleep(1000 / desiredFPS);
            }
        }

        private void ReceiveMessages()
        {
            NetIncomingMessage msgIn;
            MessageType msgType;
            while ((msgIn = netServer.ReadMessage()) != null)
            {

                if (msgIn.MessageType == NetIncomingMessageType.ConnectionApproval)
                {
                    Debug.Log("New Connection: {0}, {1}, {2}", msgIn.ReceiveTime, msgIn.SenderEndPoint.Address, msgIn.SenderEndPoint.Port);

                    msgType = (MessageType)msgIn.ReadByte();

                    if (msgType == MessageType.GameServer)
                    {
                        GameServer gameServer = new GameServer(msgIn.SenderConnection);

                        gameServer.publicKey = msgIn.ReadString();
                        gameServers.Add(gameServer);
                    }
                    else
                    {
                        Account account = new Account(msgIn.SenderConnection);

                        account.publicKey = msgIn.ReadString();
                        accounts.Add(account);
                    }

                    NetOutgoingMessage msgOut = netServer.CreateMessage();
                    msgOut.Write(DataEncryption.publicKey);
                    msgIn.SenderConnection.Approve(msgOut);
                }
                else if (msgIn.MessageType == NetIncomingMessageType.Data)
                {
                    Debug.Log(msgIn.ToString());
                    msgType = (MessageType)msgIn.ReadByte();
                    Console.WriteLine((MessageType)msgType);
                    string msgDecrypted = "";
                    int numOfBytes;
                    byte[] msgEncrypted;

                    if (msgType == MessageType.Encrypted)
                    {
                        msgType = (MessageType)msgIn.ReadByte();
                        if (msgType != MessageType.GameServer)
                            currentAccount = GetAccount(msgIn.SenderConnection);
                    }
                    else { msgDecrypted = msgIn.ReadString(); }

                    switch (msgType)
                    {
                        case MessageType.GameServer:
                            switch (msgType)
                            {
                                case MessageType.ServerLoginRequest:
                                    break;
                            }
                            break;
                        case MessageType.ServerLoginRequest:
                            clientMessage.HandleLoginMessage(msgIn, currentAccount);
                            break;
                        case MessageType.RegisterRequest:
                            clientMessage.HandleRegisterMessage(msgIn, currentAccount);
                            break;
                        case MessageType.CreateCharacter:
                            clientMessage.HandleCreateMessage(msgIn, currentAccount);
                            break;
                        case MessageType.DeleteCharacter:
                            clientMessage.HandleDeleteMessage(msgIn, currentAccount);
                            break;

                    }
                }
                netServer.Recycle(msgIn);

            }
        }
        private Account GetAccount(NetConnection connection)
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
