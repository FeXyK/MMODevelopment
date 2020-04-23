using System;
using System.Text;
using System.Collections.Generic;
using Lidgren.Network;
using Lidgren.Network.ServerFiles;
using Lidgren.Network.ServerFiles.Data;
using Utility;
using UnityEngine;
using Lidgren.Network.Message;
using Assets;
using Assets.AreaServer.Entity;
using Assets.AreaServer.SkillSystem;
using Assets.AreaServer.InventorySystem;

namespace MMOGameServer.WorldServer
{
    class WorldMessageHandler : MessageHandler
    {

        NetServer netServer;
        WorldMessageReader messageReader;
        WorldMessageCreater messageCreater;
        WorldDataHandler dataHandler;
        Selection selection;
        public AreaServerCore areaServer;


        public WorldMessageHandler(NetServer server, WorldDataHandler handler, AreaServerCore areaServer)
        {
            netServer = server;
            dataHandler = handler;
            messageReader = new WorldMessageReader();
            messageCreater = new WorldMessageCreater();
            messageCreater = new WorldMessageCreater();
            selection = new Selection("192.168.0.24", "mmo", 3306, "fexyk", "asdqwe123");
            this.areaServer = areaServer;
            foreach (var skill in selection.GetSkillsData())
            {
                SkillLibrary.Instance.Skills.Add(skill.ID, skill);

            }
            foreach (var item in selection.GetItems())
            {
                ItemLibrary.Instance.Items.Add(item.ID, item);
                ItemLibrary.Instance.ItemsList.Add(item);
            }

            //foreach (var skill in ItemLibrary.Instance.Items)
            //{
            //    Debug.Log(skill);
            //    foreach (var e in skill.Value.Effects)
            //    {
            //        Debug.Log("Value: " + e.Value.Value);
            //    }
            //}
            //ItemList.Instance.Items = selection.GetItemsData();
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
            int characterType = msgIn.ReadInt16();
            int result;
            //if (0 == Selection.instance.CountSqlData("SELECT COUNT(*) FROM Character WHERE LOWER(Name) = LOWER(@characterName)", new SqlParameter("characterName", characterName)))
            account = dataHandler.GetAuthenticatedUser(msgIn.SenderConnection);
            result = Selection.instance.CreateCharacter(account.id, characterName, characterType);
            if (result >= 0)
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
                CharacterWrapper data = new CharacterWrapper();
                Utility.Models.Character temp = selection.GetCharacterData(account.id, characterId, characterName);
                {
                    data.character.Connection = msgIn.SenderConnection;
                    data.character.EntityName = temp.Name;
                    data.character.EntityID = temp.CharacterID;
                    data.character.AccountID = temp.AccountID;
                    data.character.EntityMaxHealth = temp.MaxHealth.Value;
                    data.character.EntityHealth = temp.Health.Value;
                    data.character.EntityMaxMana = temp.MaxMana.Value;
                    data.character.EntityMana = temp.Mana.Value;
                    data.character.EntityLevel = temp.Level.Value;
                    data.character.EntityExp = temp.Exp.Value;
                    data.character.CharacterType = (ECharacterApperance)temp.CharType.Value;
                    data.character.EntityGold = temp.Gold.Value;
                    foreach (var skill in temp.Skills)
                    {
                        foreach (var skillItem in SkillLibrary.Instance.Skills)
                        {
                            if (skill.Key == skillItem.Key)
                            {
                                data.character.Skills.Add(skill.Key, new SkillItem(skillItem.Value, skill.Value));
                            }
                        }
                    }
                    foreach (var item in temp.Inventory)
                    {
                        data.character.Inventory.Add(item.Key, new SlotItem(item.Value));
                    }
                    Debug.Log("CHARACTER PLAY INVENTORY SIZE: " + data.character.Inventory.Count);
                }
                data.position = new Vector3((float)temp.PosX.Value, (float)temp.PosY.Value, (float)temp.PosZ.Value);
                data.authToken = account.authToken;
                data.admin = account.admin;
                data.authenticated = false;
                data.accountID = account.id;
                data.publicKey = account.publicKey;



                areaServer.newConnections.Enqueue(data);

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
                    msgOut.Write(character.currentExp, 32);
                    msgOut.Write(character.level, 32);
                    msgOut.Write(character.gold, 32);
                    msgOut.Write(character.characterType, 32);

                    msgOut.Write(character.currentHealth, 32);
                    msgOut.Write(character.maxHealth, 32);
                    msgOut.Write(character.currentMana, 32);
                    msgOut.Write(character.maxMana, 32);

                    msgOut.Write(character.positionX);
                    msgOut.Write(character.positionY);
                    msgOut.Write(character.positionZ);

                    msgOut.Write(character.skills.Count, 16);
                    foreach (var skill in character.skills)
                    {
                        msgOut.Write(skill.Key, 16);
                        msgOut.Write(skill.Value, 16);
                    }

                    msgOut.Write(character.Inventory.Count, 16);
                    foreach (var item in character.Inventory)
                    {
                        msgOut.Write(item.Key, 32);
                        msgOut.Write(item.Value[0], 32);
                        msgOut.Write(item.Value[1], 16);
                        msgOut.Write(item.Value[2], 16);
                        msgOut.Write(item.Value[3], 16);
                        msgOut.Write(item.Value[4], 16);
                        msgOut.Write(item.Value[5], 16);
                    }
                }
                account.connection.SendMessage(msgOut, NetDeliveryMethod.ReliableOrdered, 2);
                Console.WriteLine("CharacterList Sent");
            }
        }
        internal void UpdateAccountCharacterList(ClientData account)
        {
            account.characters.Clear();
            List<Utility.Models.Character> charactersTemp = Selection.instance.GetCharactersData(account.id);
            CharacterData character;

            foreach (var temp in charactersTemp)
            {
                Character existingEntity = areaServer.messageHandler.GetCharacter(temp.CharacterID);
                if (existingEntity != null)
                {
                    character = new CharacterData();
                    character.name = temp.Name;
                    character.id = temp.CharacterID;
                    character.accountID = temp.AccountID;
                    character.positionX = existingEntity.position.x;
                    character.positionY = existingEntity.position.y;
                    character.positionZ = existingEntity.position.z;
                    character.currentHealth = temp.Health.Value;
                    character.currentMana = temp.Mana.Value;
                    character.maxHealth = temp.MaxHealth.Value;
                    character.maxMana = temp.MaxMana.Value;
                    character.level = temp.Level.Value;
                    character.currentExp = temp.Exp.Value;
                    character.characterType = temp.CharType.Value;

                    character.skills = new Dictionary<int, int>();
                    foreach (var skill in existingEntity.Skills)
                    {
                        character.skills.Add(skill.Key, skill.Value.Level);
                    }
                    foreach (var item in existingEntity.Inventory)
                    {
                        int[] values = new int[6];
                        values[0] = item.Value.ID;
                        values[1] = item.Value.ItemID;
                        values[2] = item.Value.Level;
                        values[3] = item.Value.Durability;
                        values[4] = item.Value.Amount;
                        values[5] = (int)item.Value.SlotType;
                        character.Inventory.Add(item.Value.SlotID, values);
                    }
                    character.gold = temp.Gold.Value;
                    account.characters.Add(character);
                }
                else
                {
                    character = new CharacterData();
                    character.name = temp.Name;
                    character.id = temp.CharacterID;
                    character.accountID = temp.AccountID;
                    character.positionX = (float)temp.PosX.Value;
                    character.positionY = (float)temp.PosY.Value;
                    character.positionZ = (float)temp.PosZ.Value;
                    character.currentHealth = (int)temp.Health.Value;
                    character.currentMana = (int)temp.Mana.Value;
                    character.maxHealth = (int)temp.MaxHealth.Value;
                    character.maxMana = (int)temp.MaxMana.Value;
                    character.level = temp.Level.Value;
                    character.currentExp = temp.Exp.Value;
                    character.characterType = temp.CharType.Value;

                    character.skills = new Dictionary<int, int>();
                    foreach (var skill in temp.Skills)
                    {
                        character.skills.Add(skill.Key, skill.Value);
                    }
                    foreach (var item in temp.Inventory)
                    {
                        int[] values = new int[6];
                        values[0] = item.Value.ID;
                        values[1] = item.Value.ItemID;
                        values[2] = item.Value.Level;
                        values[3] = item.Value.Durability;
                        values[4] = item.Value.Amount;
                        values[5] = (int)item.Value.SlotType;
                        character.Inventory.Add(item.Value.SlotID, values);
                    }
                    character.gold = temp.Gold.Value;
                    account.characters.Add(character);
                }
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
