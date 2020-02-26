using Lidgren.Network;
using Lidgren.Network.ServerFiles;
using MMOLoginServer.ServerData;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace MMOLoginServer.LoginServerLogic
{
    public class ClientMessageHandler
    {
        private NetServer netServer;
        private DatabaseSelection dbSelection;
        private BasicFunctions basicFunction;
        const string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\Github\MMODevelopment\MMOLoginServer\MMOGameServer\MMODB.mdf;Integrated Security=True";

        public ClientMessageHandler(NetServer server)
        {
            netServer = server;
            basicFunction = new BasicFunctions();
            dbSelection = new DatabaseSelection(connectionString);
        }

        public void SendCharacterData(ClientData account)
        {
            NetOutgoingMessage msgOut = netServer.CreateMessage();

            List<CharacterData> characters = dbSelection.GetCharactersData(account.id);

            foreach (var character in characters)
            {
                Console.WriteLine(character.ToString());
                msgOut = netServer.CreateMessage();
                msgOut.Write((byte)MessageType.CharacterData);
                PacketHandler.WriteEncryptedByteArray(msgOut, character.ToString(), account.publicKey);
                account.connection.SendMessage(msgOut, NetDeliveryMethod.ReliableOrdered, 2);
            }
        }
        public void HandleCreateMessage(NetIncomingMessage msgIn, ClientData account)
        {
            byte[] characterNameEncrypted = PacketHandler.ReadEncryptedByteArray(msgIn);
            string characterName = Encoding.UTF8.GetString(characterNameEncrypted);

            dbSelection.CreateCharacter(characterName, account);
            SendCharacterData(account);
        }
        public void HandleDeleteMessage(NetIncomingMessage msgIn, ClientData account)
        {
            int accountId = account.id;
            byte[] characterNameEncrypted = PacketHandler.ReadEncryptedByteArray(msgIn);
            string characterName = Encoding.UTF8.GetString(characterNameEncrypted);

            dbSelection.DeleteCharacter(accountId, characterName);
            SendCharacterData(account);

        }
        public void HandleRegisterMessage(NetIncomingMessage msgIn, ClientData account)
        {
            if (account != null)
            {
                //string salt = basicFunction.GenerateRandomSequence(16);

                byte[] username = PacketHandler.ReadEncryptedByteArray(msgIn);
                byte[] password = PacketHandler.ReadEncryptedByteArray(msgIn);
                byte[] email = PacketHandler.ReadEncryptedByteArray(msgIn);

                byte[] saltBytes = basicFunction.GenerateRandomSequence(16);
                byte[] passwordSalted = new byte[password.Length + saltBytes.Length];

                passwordSalted = basicFunction.ConcatByteArrays(password, saltBytes);

                passwordSalted = new System.Security.Cryptography.SHA256Managed().ComputeHash(passwordSalted);

                Debug.Log("Registering: " + Encoding.UTF8.GetString(username) + "\nEmail: " + Encoding.UTF8.GetString(email) + "\nPw: " + BitConverter.ToString(passwordSalted));

                List<string[]> sqlData = dbSelection.GetSqlData("SELECT * FROM Account WHERE username = @0 OR email = @1", new SqlParameter("0", username), new SqlParameter("1", email));
                if (sqlData.Count == 0)
                {
                    int result = dbSelection.InsertSqlData("INSERT INTO Account (Username,Password,Email,Salt) VALUES (@username,@password,@email,@salt)",
                        new SqlParameter("username", username),
                        new SqlParameter("password", passwordSalted),
                        new SqlParameter("email", email),
                        new SqlParameter("salt", saltBytes));
                    if (result >= 0)
                        RegisterSuccessfullMessage("Successfull registration!", msgIn.SenderConnection);
                }
                else
                {
                    RegisterErrorMessage("Error, existing user", msgIn.SenderConnection);
                }
            }
            else
            {
                RegisterErrorMessage("Error, not existing connection", msgIn.SenderConnection);
            }
        }
        public void HandleLoginMessage(NetIncomingMessage msgIn, ClientData account, List<GameServerData> gameServers)
        {
            string usernameDecoded;

            byte[] username = PacketHandler.ReadEncryptedByteArray(msgIn);
            byte[] password = PacketHandler.ReadEncryptedByteArray(msgIn);

            usernameDecoded = Encoding.UTF8.GetString(username);

            Debug.Log("Logging in: " + usernameDecoded);
            if ((account.id = dbSelection.UserAuthentication(usernameDecoded, password)) != -1)
            {
                account.name = usernameDecoded;
                account.characters = dbSelection.GetCharactersData(account.id);

                NetOutgoingMessage msgOut = netServer.CreateMessage();
                msgOut.Write((byte)MessageType.ServerLoginAnswerOk);
                msgOut.Write("Authenticated");
                msgIn.SenderConnection.SendMessage(msgOut, NetDeliveryMethod.ReliableOrdered, 1);

                SendCharacterData(account);

                msgOut = netServer.CreateMessage();
                msgOut.Write((byte)MessageType.GameServersData);

                msgOut.Write(gameServers.Count, 16);
                foreach (var server in gameServers)
                {
                    msgOut.Write(server.name);
                    msgOut.Write(server.connection.RemoteEndPoint.Address.ToString());
                    msgOut.Write(server.connection.RemoteEndPoint.Port, 32);
                    msgOut.Write(server.publicKey);
                }
                msgIn.SenderConnection.SendMessage(msgOut, NetDeliveryMethod.ReliableOrdered, 1);

                Debug.Log(usernameDecoded + ": Authenticated");
            }
            else
            {
                Debug.Log(usernameDecoded + ": Bad Username or Password");
                NetOutgoingMessage msgError = netServer.CreateMessage();
                msgError.Write((byte)MessageType.ServerLoginError);
                msgError.Write("Bad Username or Password");
                msgIn.SenderConnection.SendMessage(msgError, NetDeliveryMethod.ReliableOrdered, 1);
            }
        }

        public void HandleConnectionApproval(NetIncomingMessage msgIn, List<ClientData> accounts)
        {
            MessageType msgType;
            Debug.Log("New Connection: {0}, {1}, {2}", msgIn.ReceiveTime, msgIn.SenderEndPoint.Address, msgIn.SenderEndPoint.Port);

            msgType = (MessageType)msgIn.ReadByte();
            {
                ClientData account = new ClientData();
                account.connection = msgIn.SenderConnection;
                account.publicKey = msgIn.ReadString();
                accounts.Add(account);
            }
            NetOutgoingMessage msgOut = netServer.CreateMessage();
            msgOut.Write(DataEncryption.publicKey);
            msgIn.SenderConnection.Approve(msgOut);
        }
        private void RegisterSuccessfullMessage(string msgSucc, NetConnection senderConnection)
        {
            NetOutgoingMessage msgOut = netServer.CreateMessage();
            msgOut.Write((byte)MessageType.RegisterAnswerOk);
            msgOut.Write(msgSucc);

            senderConnection.SendMessage(msgOut, NetDeliveryMethod.ReliableOrdered, 1);
        }
        private void RegisterErrorMessage(string msgErr, NetConnection senderConnection)
        {
            NetOutgoingMessage msgRegError = netServer.CreateMessage();
            msgRegError.Write((byte)MessageType.RegisterAnswerError);
            msgRegError.Write(msgErr);

            senderConnection.SendMessage(msgRegError, NetDeliveryMethod.ReliableOrdered, 1);
        }   
    }
}
