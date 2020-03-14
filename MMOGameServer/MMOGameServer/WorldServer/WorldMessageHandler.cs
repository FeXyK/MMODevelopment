using Lidgren.Network;
using Lidgren.Network.ServerFiles;
using Lidgren.Network.ServerFiles.Data;
using MMOLoginServer.ServerData;
using System;
using System.Text;
using System.Collections.Generic;
using Utility;
using Utility.Models;

namespace MMOGameServer.WorldServer
{
    class WorldMessageHandler : Lidgren.Network.Message.MessageHandler
    {

        NetServer netServer;
        WorldMessageReader messageReader;
        WorldMessageCreater messageCreater;
        WorldDataHandler dataHandler;
        Selection selection;
        const string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\Github\MMODevelopment\MMOLoginServer\MMOGameServer\MMODB.mdf;Integrated Security=True";

        public WorldMessageHandler(NetServer server, WorldDataHandler handler)
        {
            netServer = server;
            dataHandler = handler;
            messageReader = new WorldMessageReader();
            messageCreater = new WorldMessageCreater();
            messageCreater = new WorldMessageCreater();
            selection = new Selection(connectionString);
        }
        internal void KeyExchange(NetIncomingMessage msgIn)
        {
            NetOutgoingMessage msgOut = netServer.CreateMessage();
            ConnectionData connection = new ConnectionData();

            connection.publicKey = msgIn.ReadString();
            connection.connection = msgIn.SenderConnection;
            dataHandler.newConnections.Add(connection);

            msgOut.Write((byte)MessageType.KeyExchange);
            msgOut.Write(DataEncryption.publicKey);
            connection.connection.Approve(msgOut);

            Debug.Log(connection.publicKey);

        }
        internal void DeleteCharacter(NetIncomingMessage msgIn)
        {
            ConnectionData account = dataHandler.GetAuthenticatedUser(msgIn.SenderConnection);
            int accountId = account.id;
            byte[] characterNameEncrypted = PacketHandler.ReadEncryptedByteArray(msgIn);
            string characterName = Encoding.UTF8.GetString(characterNameEncrypted);

            Selection.instance.DeleteCharacter(account.id, characterName);
            SendCharacterList(msgIn);
        }

        internal void CreateCharacter(NetIncomingMessage msgIn)
        {
            ConnectionData account;
            byte[] characterNameEncrypted = PacketHandler.ReadEncryptedByteArray(msgIn);
            string characterName = Encoding.UTF8.GetString(characterNameEncrypted);
            int characterType = msgIn.ReadInt32();
            int result;
            //if (0 == Selection.instance.CountSqlData("SELECT COUNT(*) FROM Character WHERE LOWER(Name) = LOWER(@characterName)", new SqlParameter("characterName", characterName)))
            account = dataHandler.GetAuthenticatedUser(msgIn.SenderConnection);
            result = Selection.instance.CreateCharacter(account.id, characterName, characterType);

            if (result > 0)
                SendNotificationMessage("Character created!", msgIn.SenderConnection);
            else
                SendNotificationMessage("Invalid Name: Character already exists", msgIn.SenderConnection);
            SendCharacterList(msgIn);
        }
        internal void PlayCharacter(NetIncomingMessage msgIn, AreaServerCore areaServer)
        {
            ConnectionData account = dataHandler.GetAuthenticatedUser(msgIn.SenderConnection);

            string username = PacketHandler.ReadEncryptedString(msgIn);
            string characterName = PacketHandler.ReadEncryptedString(msgIn);
            int characterId = msgIn.ReadInt16();// PacketHandler.ReadEncryptedInt(msgIn);

            Debug.Log("PlayCharacter");
            Debug.Log(username);
            Debug.Log(characterName);
            Debug.Log(account.name);
            Debug.Log(account.id);
            Debug.Log(account.authenticated);
            if (username == account.name && account.authenticated)
            {
                account.authToken = Util.GenerateRandomSequence(40);
                CharacterData character = new CharacterData();
                Character temp = selection.GetCharacterData(account.id, characterId, characterName);

                {
                    character.name = temp.Name;
                    character.id = temp.Id;
                    character.accountID = temp.AccountId;
                    character.positionX = (float)temp.PosX.Value;
                    character.positionY = (float)temp.PosY.Value;
                    character.positionZ = (float)temp.PosZ.Value;
                    character.rotation = (float)temp.Rotation.Value;
                    character.currentHealth = temp.Health.Value;
                    character.currentMana = temp.Mana.Value;
                    character.level = temp.Level.Value;
                    character.currentExp = temp.Exp.Value;
                    character.characterType = temp.CharType.Value;
                    //character.= temp.CharSkills.Value;
                    character.gold = temp.Gold.Value;
                }

                character.authToken = account.authToken;
                character.admin = account.admin;
                character.authenticated = false;
                character.accountID = account.id;
                character.publicKey = account.publicKey;
                Console.WriteLine("AIUODEHUSADASJKNADSJNDSA:::::::    " + character.id);

                areaServer.newConnections.Enqueue(character);

                NetOutgoingMessage msgOut = netServer.CreateMessage();
                msgOut.Write((byte)MessageType.NewAuthenticationToken);
                PacketHandler.WriteEncryptedByteArray(msgOut, account.authToken, account.publicKey);
                PacketHandler.WriteEncryptedByteArray(msgOut, BitConverter.GetBytes(areaServer.netPeer.Port), account.publicKey);
                msgIn.SenderConnection.SendMessage(msgOut, NetDeliveryMethod.ReliableOrdered, 1);
            }
        }
        internal void Alive(NetIncomingMessage msgIn)
        {
            ConnectionData connection = dataHandler.GetAuthenticatedUser(msgIn.SenderConnection);
            if (connection != null)
                Debug.Log(connection.name + ": Alive");
        }

        private void SendNotificationMessage(string msg, NetConnection senderConnection)
        {
            NetOutgoingMessage msgOut = netServer.CreateMessage();
            msgOut.Write((byte)MessageType.Notification);
            msgOut.Write(msg);
            senderConnection.SendMessage(msgOut, NetDeliveryMethod.ReliableOrdered, 1);
        }

        internal void SendAuthenticationToken(NetIncomingMessage msgIn, string authToken, string worldServerName)
        {
            ConnectionData connection = dataHandler.loginServer;
            if (connection != null)
            {
                NetOutgoingMessage msgOut = netServer.CreateMessage();
                msgOut.Write((byte)MessageType.WorldServerAuthentication);
                PacketHandler.WriteEncryptedByteArray(msgOut, authToken, connection.publicKey);
                PacketHandler.WriteEncryptedByteArray(msgOut, worldServerName, connection.publicKey);
                msgIn.SenderConnection.SendMessage(msgOut, NetDeliveryMethod.ReliableOrdered, 1);
                Debug.Log("Sending AuthToken to login server");
            }
        }
        internal void NewAuthenticationToken(NetIncomingMessage msgIn)
        {
            AuthenticationTokenData authToken = new AuthenticationTokenData();

            authToken.token = PacketHandler.ReadEncryptedByteArray(msgIn);
            authToken.accountName = Encoding.UTF8.GetString(PacketHandler.ReadEncryptedByteArray(msgIn));
            authToken.validIP = Encoding.UTF8.GetString(PacketHandler.ReadEncryptedByteArray(msgIn));
            authToken.id = PacketHandler.ReadEncryptedInt(msgIn);
            //authToken.expireDate = PacketHandler.ReadEncryptedByteArray(msgIn);

            dataHandler.authTokens.Add(authToken);
        }
        internal void ClientAuthentication(NetIncomingMessage msgIn)
        {
            ConnectionData connection = dataHandler.GetNewConnection(msgIn.SenderConnection);

            byte[] token = PacketHandler.ReadEncryptedByteArray(msgIn);
            string username = Encoding.UTF8.GetString(PacketHandler.ReadEncryptedByteArray(msgIn));
            string ip = msgIn.SenderConnection.RemoteEndPoint.Address.ToString();
            //int id = BitConverter.ToInt32(PacketHandler.ReadEncryptedByteArray(msgIn), 0);

            connection.name = username;
            connection.ip = ip;
            connection.authToken = token;
            //connection.id = id;

            foreach (var authToken in dataHandler.authTokens)
            {

                Debug.Log(authToken.validIP);
                Debug.Log(ip);
                Debug.Log(authToken.accountName);
                Debug.Log(username);
                Debug.Log(BitConverter.ToString(token));
                Debug.Log(BitConverter.ToString(authToken.token));
                if (Util.CompareByteArrays(authToken.token, token))
                {
                    if (authToken.validIP == ip && authToken.accountName == username)
                    {
                        connection.authenticated = true;
                        connection.id = authToken.id;
                        NetOutgoingMessage msgOut = netServer.CreateMessage();
                        msgOut.Write((byte)MessageType.ClientAuthenticated);

                        dataHandler.authenticatedConnections.Add(new ClientData(connection));

                        Console.WriteLine((NetSendResult)msgIn.SenderConnection.SendMessage(msgOut, NetDeliveryMethod.ReliableOrdered, 1));
                        Debug.Log("Successfull authentication");
                    }
                    else
                    {
                        dataHandler.authTokens.Remove(authToken);
                        Debug.Log("Unsuccessfull authentication");
                    }
                    dataHandler.authTokens.Remove(authToken);
                    break;
                }
            }
            dataHandler.newConnections.Remove(connection);
        }
        public void SendCharacterList(NetIncomingMessage msgIn)
        {
            ClientData account = dataHandler.GetAuthenticatedUser(msgIn.SenderConnection);
            Console.WriteLine("Authenticated users count: " + dataHandler.authenticatedConnections.Count);
            Console.WriteLine("Account auth: " + account.authenticated);
            if (account.authenticated)
            {
                UpdateAccountCharacterList(account);
                NetOutgoingMessage msgOut = netServer.CreateMessage();

                msgOut = netServer.CreateMessage();
                msgOut.Write((byte)MessageType.CharacterData);
                msgOut.Write(account.characters.Count, 16);

                foreach (var character in account.characters)
                {
                    Console.WriteLine(character.ToString());
                    msgOut.Write(character.name);
                    msgOut.Write(character.id, 32);
                    msgOut.Write(character.accountID, 32);
                    msgOut.Write(character.level, 32);
                    msgOut.Write(character.gold, 32);
                    msgOut.Write(character.characterType, 32);
                }
                account.connection.SendMessage(msgOut, NetDeliveryMethod.ReliableOrdered, 2);
                Console.WriteLine("CharacterList Sent");
            }
        }
        internal void UpdateAccountCharacterList(ClientData account)
        {
            Console.WriteLine("UPDATE CH LIST");
            Console.WriteLine(account.id);
            Console.WriteLine(account.name);
            List<Character> charactersTemp = Selection.instance.GetCharactersData(account.id);
            CharacterData character;
            foreach (var temp in charactersTemp)
            {
                character = new CharacterData();
                character.name = temp.Name;
                character.id = temp.Id;
                character.accountID = temp.AccountId;
                character.positionX = (float)temp.PosX.Value;
                character.positionY = (float)temp.PosY.Value;
                character.positionZ = (float)temp.PosZ.Value;
                character.rotation = (float)temp.Rotation.Value;
                character.currentHealth = temp.Health.Value;
                character.currentMana = temp.Mana.Value;
                character.level = temp.Level.Value;
                character.currentExp = temp.Exp.Value;
                character.characterType = temp.CharType.Value;
                //character.= temp.CharSkills.Value;
                character.gold = temp.Gold.Value;
                account.characters.Add(character);
            }
        }
        internal void SuccessfullAuthentication(NetIncomingMessage msgIn)
        {
            ConnectionData connection = dataHandler.GetNewConnection(msgIn.SenderConnection);
            dataHandler.loginServer = connection;
            dataHandler.newConnections.Remove(connection);

            connection.name = "LoginServer";
            connection.type = ConnectionType.LoginServer;

        }
    }
}
