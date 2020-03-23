using Assets.AreaServer.Entity;
using Assets.Scripts.Handlers;
using Lidgren.Network;
using Lidgren.Network.ServerFiles;
using MMOLoginServer.ServerData;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

namespace MMOGameServer
{
    public class AreaMessageHandler
    {
        NetServer netServer;

        AreaDataHandler dataHandler;
        AreaMessageReader messageReader;
        AreaMessageCreater messageCreate;
        ConcurrentQueue<CharacterData> newConnectionsQue;
        AreaMessageSender messageSender;
        Dictionary<NetConnection, Character> onlineCharacters = new Dictionary<NetConnection, Character>();
        List<NetConnection> onlineConnections = new List<NetConnection>();
        public bool hideNames = false;
        public AreaMessageHandler(NetServer server, ConcurrentQueue<CharacterData> newConnections)
        {
            netServer = server;
            newConnectionsQue = newConnections;
            dataHandler = new AreaDataHandler();
            messageCreate = new AreaMessageCreater(netServer);
            messageReader = new AreaMessageReader();
            messageSender = new AreaMessageSender(netServer);
            {
                MobAreaSpawner mobArea = new MobAreaSpawner(new Vector3(7, 80, 178));
                dataHandler.mobAreas.Add(mobArea);
                Debug.Log("MobSpawner COUNT: " + dataHandler.mobAreas.Count);
            }
            Debug.Log(dataHandler.mobAreas.ToString());
        }
        internal void GetNewConnections()
        {
            if (newConnectionsQue.Count > 0)
            {
                CharacterData character = new CharacterData();
                newConnectionsQue.TryDequeue(out character);
                if (character != null)
                {
                    dataHandler.connections.Add(character);

                    //Character newCharacter = LoadCharacterFrom(character);

                    //onlineCharacters.Add(character.connection, newCharacter);
                    //onlineConnections.Add(character.connection);

                    Debug.Log(character.name + " added to characters");
                }
            }
        }
        private Character LoadCharacterFrom(CharacterData data)
        {
            Character character = new Character();
            character.EntityID = data.id;
            character.AccountID = data.accountID;
            character.EntityName = data.name;
            character.EntityGold = data.gold;
            character.EntityHealth = data.currentHealth;
            character.EntityMaxHealth = data.maxHealth;
            character.EntityMana = data.currentMana;
            character.EntityLevel = data.level;
            character.EntityBaseArmor = 60;
            character.EntityBaseMagicResist = 60;
            character.CharacterType = (CharacterApperance)data.characterType;

            return character;
        }

        internal void StartSkillCast(NetIncomingMessage msgIn)
        {
            int sourceID;//= msgIn.ReadInt16();
            int targetID = msgIn.ReadInt16();
            int skillID = msgIn.ReadInt16();
            sourceID = dataHandler.GetCharacter(msgIn).id;

            Entity target = null;
            if (targetID < 10000)
            {
                target = dataHandler.GetMob(targetID);//[sourceID] as Character;
            }
            else
            {
                target = dataHandler.entities[targetID] as Character;
            }
            Character source = dataHandler.entities[sourceID] as Character;

            target.ApplyDamage(24);

            NetOutgoingMessage msgOut = netServer.CreateMessage();
            msgOut.Write((byte)MessageType.SkillCasted);
            msgOut.Write(sourceID, 16);
            msgOut.Write(targetID, 16);
            msgOut.Write(skillID, 16);
            msgOut.Write(target.EntityHealth,16);
            netServer.SendToAll(msgOut, NetDeliveryMethod.Unreliable);
            //if (source.SkillReady(skillID))
            //{
            //    Character target = dataHandler.entities[targetID] as Character;
            //    NetOutgoingMessage msgOut = netServer.CreateMessage();
            //    msgOut.Write((byte)MessageType.SkillCasted);
            //    msgOut.Write(skillID, 16);
            //    SendInRange(msgOut);

            //    target.ApplyDamage(50);
            //}

        }

        private void SendInRange(NetOutgoingMessage msgOut)
        {
            throw new NotImplementedException();
        }

        public void ClientReady(NetIncomingMessage msgIn)
        {
            CharacterData characterData = dataHandler.GetCharacter(msgIn);

            //To already ingame players
            NetOutgoingMessage msgOut = messageCreate.CreateNewCharacterMessage(characterData);
            netServer.SendMessage(msgOut, dataHandler.netConnections, NetDeliveryMethod.ReliableOrdered, 1);

            //To new connected player
            foreach (var character in dataHandler.characters)
            {
                if (character.Value.connection != msgIn.SenderConnection)
                {
                    msgOut = messageCreate.CreateNewCharacterMessage(character.Value);
                    msgIn.SenderConnection.SendMessage(msgOut, NetDeliveryMethod.ReliableOrdered, 1);
                }
            }
            //msgOut = CreateHideNamesMessage();
            //msgIn.SenderConnection.SendMessage(msgOut, NetDeliveryMethod.ReliableOrdered, 1);
            ClientMobSpawn(msgIn.SenderConnection);
        }

        internal void SendMobPositions()
        {
            foreach (var mobArea in dataHandler.mobAreas)
            {
                NetOutgoingMessage msgOut = netServer.CreateMessage();
                msgOut.Write((byte)MessageType.MobInformation);
                msgOut.Write(mobArea.SpawnedMobs.Count, 16);
                foreach (var mob in mobArea.SpawnedMobs.Values)
                {
                    msgOut.Write(mob.EntityID, 16);
                    msgOut.Write(mob.transform.position.x);
                    msgOut.Write(mob.transform.position.y);
                    msgOut.Write(mob.transform.position.z);
                }
                netServer.SendToAll(msgOut, NetDeliveryMethod.Unreliable);
            }
        }
        public void ClientMobSpawn(NetConnection connection)
        {
            NetOutgoingMessage msgOut = netServer.CreateMessage();
            msgOut.Write((byte)MessageType.NewMobAreaData);
            Debug.Log(connection.ToString() + " SENDING NEW MOBINFO: " + dataHandler.mobAreas.Count);
            foreach (var mobArea in dataHandler.mobAreas)
            {
                msgOut.Write(mobArea.SpawnedMobs.Count, 16);
                foreach (var mob in mobArea.SpawnedMobs.Values)
                {
                    msgOut.Write(mob.EntityName);
                    msgOut.Write(mob.EntityID, 16);
                    msgOut.Write(mob.EntityLevel, 16);
                    msgOut.Write(mob.transform.position.x);
                    msgOut.Write(mob.transform.position.y);
                    msgOut.Write(mob.transform.position.z);
                    msgOut.Write(mob.EntityHealth,16);
                    msgOut.Write(mob.EntityMaxHealth,16);
                }
                connection.SendMessage(msgOut, NetDeliveryMethod.ReliableOrdered, 1);
            }
        }

        public NetOutgoingMessage CreateHideNamesMessage()
        {
            NetOutgoingMessage msgOut = netServer.CreateMessage();
            if (hideNames)
                msgOut.Write((byte)MessageType.HideNames);
            else
                msgOut.Write((byte)MessageType.ShowNames);
            return msgOut;
        }
        internal void SendPrivateChatMessage(NetIncomingMessage msgIn)
        {
            NetOutgoingMessage msgOut = netServer.CreateMessage();
            string from = msgIn.ReadString();
            string to = msgIn.ReadString();
            string msg = msgIn.ReadString();

            msgOut.Write((byte)MessageType.PrivateChatMessage);
            msgOut.Write(from);
            msgOut.Write(msg);
            foreach (var character in dataHandler.characters.Values)
            {
                if (character.name.ToLower() == to.ToLower())
                {
                    character.connection.SendMessage(msgOut, NetDeliveryMethod.ReliableUnordered, 0);
                }
            }
        }
        internal void SendPublicChatMessage(NetIncomingMessage msgIn)
        {
            NetOutgoingMessage msgOut = netServer.CreateMessage();
            string from = msgIn.ReadString();
            string msg = msgIn.ReadString();
            msgOut.Write((byte)MessageType.PublicChatMessage);
            msgOut.Write(from);
            msgOut.Write(msg);
            netServer.SendToAll(msgOut, NetDeliveryMethod.Unreliable);
        }

        internal void HandleAdminMessage(NetIncomingMessage msgIn)
        {
            NetOutgoingMessage msgOut = netServer.CreateMessage();
            string msg = msgIn.ReadString();
            switch (msg.ToLower())
            {
                case "hidenames":
                    hideNames = true;
                    msgOut.Write((byte)MessageType.AdminChatMessage);
                    msgOut.Write((byte)MessageType.HideNames);
                    break;
                case "shownames":
                    hideNames = false;
                    msgOut.Write((byte)MessageType.AdminChatMessage);
                    msgOut.Write((byte)MessageType.ShowNames);
                    break;
                default:
                    msgOut.Write((byte)MessageType.AdminChatMessage);
                    msgOut.Write((byte)MessageType.AdminChatMessage);
                    msgOut.Write("Admin");
                    msgOut.Write(msg);
                    break;
            }
            netServer.SendToAll(msgOut, NetDeliveryMethod.ReliableOrdered);
        }

        public void ClientAuthentication(NetIncomingMessage msgIn)
        {
            byte[] clientLoginToken = PacketHandler.ReadEncryptedByteArray(msgIn);
            int characterId = msgIn.ReadInt16();
            CharacterData characterData = dataHandler.connections.Find(x => x.id == characterId);

            Console.WriteLine("Token length: " + clientLoginToken.Length);
            Console.WriteLine(BitConverter.ToString(clientLoginToken));
            Console.WriteLine(BitConverter.ToString(characterData.authToken));
            if (dataHandler.CheckLoginToken(clientLoginToken, characterId))
            {
                msgIn.SenderConnection.Approve();

                characterData.connection = msgIn.SenderConnection;
                //characterData.id = validToken.characterData.id;
                //characterData.name = validToken.characterData.name;
                characterData.currentHealth = 100;
                characterData.positionX = 0;//new System.Numerics.Vector3(0, 0, 0);
                characterData.positionY = 0;//new System.Numerics.Vector3(0, 0, 0);
                characterData.positionZ = 0;//new System.Numerics.Vector3(0, 0, 0);
                characterData.rotation = 0;
                if (dataHandler.characters.ContainsKey(characterData.id))
                    dataHandler.characters.Remove(characterData.id);
                dataHandler.characters.Add(characterData.id, characterData);
                dataHandler.netConnections.Add(msgIn.SenderConnection);

                //Character newCharacter = new Character(characterData);
                Character newCharacter = LoadCharacterFrom(characterData);
                dataHandler.entities.Add(characterData.id, newCharacter);
                foreach (var item in dataHandler.characters.Values)
                {
                    Console.WriteLine("CHARACTER: " + item.ToString());
                }
                Console.WriteLine("Authenticated!");
            }
            else
            {
                Console.WriteLine("Bad AuthenticationKey");
            }
        }
        public void CharacterMovementData(NetIncomingMessage msgIn)
        {
            int id = msgIn.ReadInt16();
            float x = msgIn.ReadFloat();
            float y = msgIn.ReadFloat();
            float z = msgIn.ReadFloat();
            dataHandler.characters[id].positionX = x;
            dataHandler.characters[id].positionY = y;
            dataHandler.characters[id].positionZ = z;
        }
        public void SendMovementMessages()
        {
            NetOutgoingMessage msgOut;
            foreach (var character in dataHandler.characters)
            {
                msgOut = messageCreate.MovementMessage(character.Value);
                if (dataHandler.netConnections.Count > 0)
                    netServer.SendMessage(msgOut, dataHandler.netConnections, NetDeliveryMethod.UnreliableSequenced, 1);
            }
        }
        public void SendLogoutMessages(int id)
        {
            NetOutgoingMessage msgOut;
            msgOut = messageCreate.LogoutMessage(id);
            if (dataHandler.netConnections.Count > 0)
                netServer.SendMessage(msgOut, dataHandler.netConnections, NetDeliveryMethod.UnreliableSequenced, 1);
        }
        public void ClearConnections()
        {
            //List<int> removeKeys = new List<int>();
            //Console.WriteLine("Online: " + dataHandler.characters.Count);
            //foreach (var character in dataHandler.characters)
            //{
            //    Console.WriteLine(character.Value.name + ": " + character.Value.connection.Status);
            //    if (character.Value.connection.Status == NetConnectionStatus.Disconnected)
            //    {
            //        removeKeys.Add(character.Key);
            //    }
            //}
            //for (int i = dataHandler.connections.Count - 1; i >= 0; i--)
            //{
            //    if (dataHandler.connections[i].connection.Status == NetConnectionStatus.Disconnected)
            //    {
            //        dataHandler.connections.RemoveAt(i);
            //    }
            //}
            //for (int i = dataHandler.netConnections.Count - 1; i >= 0; i--)
            //{
            //    if (dataHandler.netConnections[i].Status == NetConnectionStatus.Disconnected)
            //    {
            //        dataHandler.netConnections.RemoveAt(i);
            //    }
            //}
            //foreach (var key in removeKeys)
            //{
            //    SendLogoutMessages(key);
            //    dataHandler.characters.Remove(key);
            //}
        }
    }
}