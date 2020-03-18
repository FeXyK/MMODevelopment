using Lidgren.Network;
using Lidgren.Network.ServerFiles;
using MMOLoginServer.ServerData;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net.Security;
using System.Security.Claims;
using System.Text;
using Utility_dotNET_Framework;

namespace MMOLoginServer.LoginServerLogic
{
    public class MessageHandler
    {
        private NetServer netServer;
        private Selection dbSelection;
        DataHandler dataHandler;
        MessageCreater messageCreate;
        string connectionString = "Server=192.168.0.24" + ";Database=mmo" + ";Port=" + 3306 + ";User=fexyk" + ";Password=asdqwe123" + ";" + "CharSet=utf8;";
        public MessageHandler(NetServer server)
        {
            netServer = server;
            dataHandler = new DataHandler();
            messageCreate = new MessageCreater();

            dbSelection = new Selection("192.168.0.24", "mmo", 3306, "fexyk", "asdqwe123");
        }


        internal void KeyExchange(NetIncomingMessage msgIn)
        {
            ConnectionData connectionData = new ConnectionData();
            connectionData.connection = msgIn.SenderConnection;
            connectionData.publicKey = msgIn.ReadString();
            connectionData.ip = msgIn.SenderConnection.RemoteEndPoint.Address.ToString();
            connectionData.port = msgIn.SenderConnection.RemoteEndPoint.Port;
            dataHandler.newConnections.Add(connectionData);

            Debug.Log(connectionData.publicKey);

            NetOutgoingMessage msgOut = netServer.CreateMessage();
            msgOut.Write((byte)MessageType.KeyExchange);
            msgOut.Write(DataEncryption.publicKey);
            connectionData.connection.Approve(msgOut);
        }

        internal void AuthenticateWorldServer(NetIncomingMessage msgIn)
        {
            string authToken = PacketHandler.ReadEncryptedString(msgIn);
            ConnectionData gameServer;
            if (authToken == dataHandler.gameServerKey)
            {
                gameServer = dataHandler.GetNewConnection(msgIn);
                if (gameServer != null)
                {
                    NetOutgoingMessage msgOut = netServer.CreateMessage();

                    gameServer.name = PacketHandler.ReadEncryptedString(msgIn);
                    gameServer.ip = msgIn.SenderConnection.RemoteEndPoint.Address.ToString();
                    gameServer.port = msgIn.SenderConnection.RemoteEndPoint.Port;
                    gameServer.authenticated = true;
                    gameServer.type = ConnectionType.GameServer;

                    dataHandler.worldServers.Add(gameServer);
                    dataHandler.newConnections.Remove(gameServer);

                    msgOut.Write((byte)MessageType.LoginServerAuthentication);
                    PacketHandler.WriteEncryptedByteArray(msgOut, dataHandler.loginServerKey);
                    msgIn.SenderConnection.SendMessage(msgOut, NetDeliveryMethod.ReliableOrdered, 1);
                }
            }
        }

        internal void Alive(NetIncomingMessage msgIn)
        {
            ConnectionData connection = dataHandler.GetAccount(msgIn);
            if (connection != null)
                Debug.Log(connection.name + ": Alive");
        }

        internal void SendWorldServerAuthenticationToken(NetIncomingMessage msgIn)
        {
            ConnectionData account = dataHandler.GetAccount(msgIn);
            ConnectionData worldServer;
            NetOutgoingMessage msgOut;

            if (account != null && account.authenticated)
            {
                byte[] clientAuthToken = Util.GenerateRandomSequence(40);
                string worldServerName = PacketHandler.ReadEncryptedString(msgIn);

                //SendToClient
                msgOut = netServer.CreateMessage();
                msgOut.Write((byte)MessageType.NewAuthenticationToken);
                PacketHandler.WriteEncryptedByteArray(msgOut, clientAuthToken, account.publicKey);
                msgIn.SenderConnection.SendMessage(msgOut, NetDeliveryMethod.ReliableOrdered, 1);

                //SendToWorldServer
                msgOut = netServer.CreateMessage();
                worldServer = dataHandler.GetWorldServer(worldServerName);
                msgOut.Write((byte)MessageType.NewAuthenticationToken);
                PacketHandler.WriteEncryptedByteArray(msgOut, clientAuthToken, worldServer.publicKey);
                PacketHandler.WriteEncryptedByteArray(msgOut, account.name, worldServer.publicKey);
                PacketHandler.WriteEncryptedByteArray(msgOut, account.ip, worldServer.publicKey);
                PacketHandler.WriteEncryptedByteArray(msgOut, BitConverter.GetBytes(account.id), worldServer.publicKey);
                worldServer.connection.SendMessage(msgOut, NetDeliveryMethod.ReliableOrdered, 1);
            }
        }
        public void SendNotificationMessage(string msg, NetConnection connection)
        {
            NetOutgoingMessage msgOut = netServer.CreateMessage();
            msgOut.Write((byte)MessageType.Notification);
            msgOut.Write(msg);
            connection.SendMessage(msgOut, NetDeliveryMethod.ReliableOrdered, 1);
        }
        public void RegisterAccount(NetIncomingMessage msgIn, ConnectionData account)
        {
            string username = PacketHandler.ReadEncryptedString(msgIn);
            byte[] password = PacketHandler.ReadEncryptedByteArray(msgIn);
            string email = PacketHandler.ReadEncryptedString(msgIn);

            byte[] saltBytes = Util.GenerateRandomSequence(16);
            byte[] passwordSalted = new byte[password.Length + saltBytes.Length];

            passwordSalted = Util.ConcatByteArrays(password, saltBytes);

            passwordSalted = new System.Security.Cryptography.SHA256Managed().ComputeHash(passwordSalted);

            Debug.Log("Registering: " + username + "\nEmail: " + email + "\nPw: " + BitConverter.ToString(passwordSalted));
            int result = dbSelection.CreateAccount(username, email, passwordSalted, saltBytes);
            if (result >= 0)
            {
                SendNotificationMessage("Successfull registration!", msgIn.SenderConnection);
            }
            else
            {
                SendNotificationMessage("Invalid Username or Email: Already exists", msgIn.SenderConnection);
            }
            //List<string[]> sqlData = dbSelection.GetSqlData("SELECT * FROM Account WHERE username = @0 OR email = @1", new SqlParameter("0", username), new SqlParameter("1", email));
            //if (sqlData.Count == 0)
            //{
            //    int result = dbSelection.InsertSqlData("INSERT INTO Account (Username,Password,Email,Salt) VALUES (@username,@password,@email,@salt)",
            //        new SqlParameter("username", username),
            //        new SqlParameter("password", passwordSalted),
            //        new SqlParameter("email", email),
            //        new SqlParameter("salt", saltBytes));
            //    if (result >= 0)
            //        SendNotificationMessage("Successfull registration!", msgIn.SenderConnection);
            //}
            //else
            //{
            //    SendNotificationMessage("Invalid Username or Email: Already exists", msgIn.SenderConnection);
            //}
        }
        public void AuthenticateClient(NetIncomingMessage msgIn)
        {
            ConnectionData account = dataHandler.GetNewConnection(msgIn);
            string usernameDecoded;

            byte[] username = PacketHandler.ReadEncryptedByteArray(msgIn);
            byte[] password = PacketHandler.ReadEncryptedByteArray(msgIn);

            usernameDecoded = Encoding.UTF8.GetString(username);

            Debug.Log("Logging in: " + usernameDecoded);
            if ((account.id = dbSelection.UserAuthentication(usernameDecoded, password)) != -1)
            {
                dataHandler.accounts.Add(account);
                dataHandler.newConnections.Remove(account);
                account.name = usernameDecoded;
                account.authenticated = true;

                NetOutgoingMessage msgOut = netServer.CreateMessage();
                msgOut.Write((byte)MessageType.ServerLoginSuccess);
                msgOut.Write("Authenticated");
                msgIn.SenderConnection.SendMessage(msgOut, NetDeliveryMethod.ReliableOrdered, 1);

                msgOut = netServer.CreateMessage();
                msgOut.Write((byte)MessageType.GameServersData);

                msgOut.Write(dataHandler.worldServers.Count, 16);
                foreach (var server in dataHandler.worldServers)
                {
                    msgOut.Write(server.name);
                    if (msgIn.SenderConnection.RemoteEndPoint.Address.ToString() == "127.0.0.1")
                        msgOut.Write(server.connection.RemoteEndPoint.Address.ToString());
                    else
                    {
                        msgOut.Write("86.101.120.217");
                    }
                    msgOut.Write(server.connection.RemoteEndPoint.Port, 32);
                    msgOut.Write(server.publicKey);
                }
                msgIn.SenderConnection.SendMessage(msgOut, NetDeliveryMethod.ReliableOrdered, 1);

                Debug.Log(usernameDecoded + ": Authenticated");
            }
            else
            {
                Debug.Log(usernameDecoded + ": Bad Username or Password");
                SendNotificationMessage("Bad Username or Password", msgIn.SenderConnection);
            }
        }
        public void HandleConnectionApproval(NetIncomingMessage msgIn)
        {
            MessageType msgType;
            Console.WriteLine("New Connection: {0}, {1}, {2}", msgIn.ReceiveTime, msgIn.SenderEndPoint.Address, msgIn.SenderEndPoint.Port);

            msgType = (MessageType)msgIn.ReadByte();
            {
                ConnectionData account = new ConnectionData();
                account.connection = msgIn.SenderConnection;
                account.publicKey = msgIn.ReadString();
                dataHandler.accounts.Add(account);
            }
            NetOutgoingMessage msgOut = netServer.CreateMessage();
            msgOut.Write(DataEncryption.publicKey);
            msgIn.SenderConnection.Approve(msgOut);
        }
    }
}
