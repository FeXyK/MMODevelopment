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
        const string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\Github\MMOLoginServer\MMOGameServer\MMODatabase.mdf;Integrated Security=True";



        public void InitializeLoginServer(string SERVER_NAME, int LOGIN_SERVER_PORT)
        {
            netPeerConfiguration = new NetPeerConfiguration(SERVER_NAME);
            netPeerConfiguration.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
            netPeerConfiguration.Port = LOGIN_SERVER_PORT;

            netServer = new NetServer(netPeerConfiguration);
        }

        public void StartLoginServer(int LOGIN_SERVER_FRAMERATE)
        {
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
                    }
                    else { msgDecrypted = msgIn.ReadString(); }

                    switch (msgType)
                    {
                        case MessageType.ServerLoginRequest:
                            HandleLoginMessage(msgIn);
                            break;
                        case MessageType.RegisterRequest:
                            HandleRegisterMessage(msgIn);
                            break;
                        case MessageType.CreateCharacter:
                            HandleCreateMessage(msgIn);
                            break;
                        case MessageType.DeleteCharacter:
                            HandleDeleteMessage(msgIn);
                            break;
                        case MessageType.GameServer:
                            switch (msgType)
                            {
                                case MessageType.ServerLoginRequest:
                                    break;
                            }
                            break;
                    }
                }
                netServer.Recycle(msgIn);

            }
        }
        private void HandleCreateMessage(NetIncomingMessage msgIn)
        {
            Account account = GetAccount(msgIn.SenderConnection);
            byte[] characterNameEncrypted = PacketHandler.ReadEncryptedByteArray(msgIn);
            string characterName = Encoding.UTF8.GetString(characterNameEncrypted);

            CreateCharacter(characterName, account);
            SendCharacterData(account);
        }
        private void HandleDeleteMessage(NetIncomingMessage msgIn)
        {
            Account account = GetAccount(msgIn.SenderConnection);
            int accountId = account.id;
            byte[] characterNameEncrypted = PacketHandler.ReadEncryptedByteArray(msgIn);
            string characterName = Encoding.UTF8.GetString(characterNameEncrypted);

            DeleteCharacter(accountId, characterName);
            SendCharacterData(account);

        }
        private void HandleRegisterMessage(NetIncomingMessage msgIn)
        {
            Account account = GetAccount(msgIn.SenderConnection);
            if (account != null)
            {
                string salt = GenerateSalt(16);

                byte[] username = PacketHandler.ReadEncryptedByteArray(msgIn);
                byte[] password = PacketHandler.ReadEncryptedByteArray(msgIn);
                byte[] email = PacketHandler.ReadEncryptedByteArray(msgIn);

                byte[] saltBytes = (Encoding.UTF8.GetBytes(salt));
                byte[] passwordSalted = new byte[password.Length + saltBytes.Length];

                passwordSalted = ConcatByteArrays(password, saltBytes);

                passwordSalted = new System.Security.Cryptography.SHA256Managed().ComputeHash(passwordSalted);

                Debug.Log("Registering: " + Encoding.UTF8.GetString(username) + "\nEmail: " + Encoding.UTF8.GetString(email) + "\nPw: " + BitConverter.ToString(passwordSalted));

                List<string[]> sqlData = GetSqlData("SELECT * FROM Account WHERE username = @0 OR email = @1", new SqlParameter("0", username), new SqlParameter("1", email));
                if (sqlData.Count == 0)
                {
                    int result = InsertSqlData("INSERT INTO Account (Id,Username,Password,Email,Salt) VALUES (@id,@username,@password,@email,@salt)",
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
        private void HandleLoginMessage(NetIncomingMessage msgIn)
        {
            Account account = GetAccount(msgIn.SenderConnection);

            string usernameDecoded;

            byte[] username = PacketHandler.ReadEncryptedByteArray(msgIn);
            byte[] password = PacketHandler.ReadEncryptedByteArray(msgIn);

            usernameDecoded = Encoding.UTF8.GetString(username);

            Debug.Log("Logging in: " + usernameDecoded);
            if ((account.id = UserAuthentication(usernameDecoded, password)) != -1)
            {
                account.username = usernameDecoded;
                account.characters = GetCharactersData(account.id);

                NetOutgoingMessage msgOut = netServer.CreateMessage();
                msgOut.Write((byte)MessageType.ServerLoginAnswerOk);
                msgOut.Write("Authenticated");
                msgIn.SenderConnection.SendMessage(msgOut, NetDeliveryMethod.ReliableOrdered, 1);

                SendCharacterData(account);


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
        private int DeleteCharacter(int accountId, string characterName)
        {
            int result = DeleteSqlData("DELETE FROM Character WHERE Name = @characterName AND accountID = @accountId", new SqlParameter("characterName", characterName), new SqlParameter("accountId", accountId));
            return result;
        }
        private int CreateCharacter(string characterName, Account account)
        {
            int result = InsertSqlData("INSERT INTO Character(AccountID,Name) VALUES(@accountId,@name); SELECT @@IDENTITY AS Id", new SqlParameter("accountId", account.id), new SqlParameter("Name", characterName));
            return result;
        }
        private void SendCharacterData(Account account)
        {
            NetOutgoingMessage msgOut = netServer.CreateMessage();

            List<Character> characters = GetCharactersData(account.id);

            foreach (var character in characters)
            {
                msgOut = netServer.CreateMessage();
                msgOut.Write((byte)MessageType.Encrypted);
                msgOut.Write((byte)MessageType.CharacterData);
                PacketHandler.WriteEncryptedByteArray(msgOut, character.ToString(), account.publicKey);
                account.connection.SendMessage(msgOut, NetDeliveryMethod.ReliableOrdered, 2);
            }
        }
        private int UserAuthentication(string username, byte[] password)
        {
            List<Account> accounts = GetSqlAccountData("SELECT Username, Password, Salt, Id FROM Account WHERE Username = @0", new SqlParameter("0", username));

            if (accounts.Count == 0)
                return -1;

            byte[] saltByte = accounts[0].salt;
            byte[] passwordSalted = new byte[password.Length + saltByte.Length];

            Buffer.BlockCopy(password, 0, passwordSalted, 0, password.Length);
            Buffer.BlockCopy(saltByte, 0, passwordSalted, password.Length, saltByte.Length);

            passwordSalted = new System.Security.Cryptography.SHA256Managed().ComputeHash(passwordSalted);

            Debug.Log("Got: {0}\nStoredPassword: {1}\nMessagedPassword: {2}", username, BitConverter.ToString(passwordSalted), BitConverter.ToString(accounts[0].password));

            Console.WriteLine(BitConverter.ToString(passwordSalted));
            Console.WriteLine(BitConverter.ToString(accounts[0].password));

            if (username.ToLower() == accounts[0].username.ToLower() && BitConverter.ToString(passwordSalted) == BitConverter.ToString(accounts[0].password))
            {
                return accounts[0].id;
            }
            return -1;
        }
        private List<Character> GetCharactersData(int accountId)
        {
            List<Character> characters = new List<Character>();
            List<string[]> rows = GetSqlData("SELECT * FROM Character WHERE AccountID = @0", new SqlParameter("0", accountId)); ;
            foreach (var row in rows)
            {
                characters.Add(new Character(row));
            }
            return characters;
        }
        private string GenerateLoginToken()
        {
            //TODO
            return "";
        }
        private int DeleteSqlData(string selection, params SqlParameter[] sqlParam)
        {
            int result;
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = connectionString;
                SqlCommand command = new SqlCommand(selection, conn);
                foreach (var param in sqlParam)
                {
                    command.Parameters.Add(param);
                }
                conn.Open();
                result = command.ExecuteNonQuery();

            }
            if (result < 0)
                Debug.Log("Error deleting from database");
            return result;
        }
        private int InsertSqlData(string selection, params SqlParameter[] sqlParam)
        {
            int result;
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = connectionString;
                SqlCommand command = new SqlCommand(selection, conn);
                foreach (var param in sqlParam)
                {
                    command.Parameters.Add(param);
                }

                conn.Open();
                result = command.ExecuteNonQuery();

            }
            if (result < 0)
                Debug.Log("Error inserting into database");
            return result;
        }
        private List<string[]> GetSqlData(string selection, params SqlParameter[] sqlParam)
        {
            List<string[]> data = new List<string[]>();
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = connectionString;
                SqlCommand command = new SqlCommand(selection, conn);
                foreach (var param in sqlParam)
                {
                    command.Parameters.Add(param);
                }

                conn.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    int rowNum = 0;
                    while (reader.Read())
                    {
                        data.Add(new string[reader.FieldCount]);
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            data[rowNum][i] = reader[i].ToString();
                        }
                        rowNum++;
                    }
                }
            }
            return data;
        }
        private List<Account> GetSqlAccountData(string selection, params SqlParameter[] sqlParam)
        {
            List<Account> data = new List<Account>();
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = connectionString;
                SqlCommand command = new SqlCommand(selection, conn);
                foreach (var param in sqlParam)
                {
                    command.Parameters.Add(param);
                }

                conn.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Account acc = new Account();
                        acc.username = (string)reader[0];
                        acc.password = (byte[])reader[1];
                        acc.salt = (byte[])reader[2];
                        acc.id = (int)reader[3];
                        data.Add(acc);
                    }
                }
            }
            return data;
        }
        private string GenerateSalt(int lengthOfSalt)
        {
            string salt = "";
            for (int i = 0; i < lengthOfSalt; i++)
            {
                salt += new Random().Next(0, 300);
            }
            return salt;
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
        private byte[] ConcatByteArrays(params byte[][] arrays)
        {
            byte[] data;
            int length = 0;
            foreach (var array in arrays)
            {
                length += array.Length;
            }
            data = new byte[length];
            int offset = 0;
            foreach (var array in arrays)
            {
                Buffer.BlockCopy(array, 0, data, offset, array.Length);
                offset += array.Length;
            }
            return data;
        }
    }
}
