using Assets;
using Assets.AreaServer.Entity;
using Assets.AreaServer.SkillSystem;
using Assets.Scripts.Handlers;
using Lidgren.Network;
using Lidgren.Network.Message;
using Lidgren.Network.ServerFiles;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

namespace MMOGameServer
{
    public class AreaMessageHandler
    {
        ConcurrentQueue<CharacterWrapper> newConnectionsQue;

        NetServer netServer;

        AreaDataHandler dataHandler;
        AreaMessageReader messageReader;
        AreaMessageCreater messageCreater;
        AreaMessageSender messageSender;

        GameObject characterEntity;

        public bool hideNames = false;
        public AreaMessageHandler(NetServer server, ConcurrentQueue<CharacterWrapper> newConnections)
        {
            netServer = server;
            newConnectionsQue = newConnections;
            dataHandler = new AreaDataHandler();
            messageCreater = new AreaMessageCreater(netServer, dataHandler);
            messageReader = new AreaMessageReader();
            messageSender = new AreaMessageSender(netServer);
            {
                MobAreaSpawner mobArea = new MobAreaSpawner(new Vector3(7, 80, 178));
                dataHandler.mobAreas.Add(mobArea);
                Debug.Log("MobSpawner COUNT: " + dataHandler.mobAreas.Count);
            }
            Debug.Log(dataHandler.mobAreas.ToString());
            characterEntity = Resources.Load<GameObject>("Character");
        }
        internal void GetNewConnections()
        {
            if (newConnectionsQue.Count > 0)
            {
                CharacterWrapper characterData = new CharacterWrapper();
                newConnectionsQue.TryDequeue(out characterData);
                if (characterData != null)
                {
                    dataHandler.waitingForAuth.Add(characterData);

                    Debug.Log(characterData.character.EntityName + " added to characters");
                }
            }
        }
        internal void StartSkillCast(NetIncomingMessage msgIn)
        {
            int targetID = msgIn.ReadInt16();
            int skillID = msgIn.ReadInt16();
            Character source = dataHandler.GetEntity(msgIn.SenderConnection) as Character;

            Entity target = null;
            if (targetID < 10000)
            {
                target = dataHandler.GetMob(targetID);//[sourceID] as Character;
            }
            else
            {
                target = dataHandler.entitiesByID[targetID] as Character;
            }
            Debug.Log(source.EntityName + "SKILLID: " + skillID);

            if (source.SkillReady(skillID))
            {
                switch (source.Skills[skillID].skillType)
                {
                    case 0:
                        source.ApplyCD(skillID);
                        target.ApplyDamage(source.GetDamage(skillID));
                        break;
                    case 1:
                        source.ApplyCD(skillID);
                        GameObject.Instantiate(SkillList.Instance.Projectile).GetComponent<SkillProjectile>().Set(source.transform, target, source.GetDamage(skillID));
                        break;
                    case 2: 
                        break;
                }
                NetOutgoingMessage msgOut = messageCreater.SkillCasted(source.EntityID, targetID, skillID);
                netServer.SendToAll(msgOut, NetDeliveryMethod.Unreliable);
            }
            else
            {
                NetOutgoingMessage msgOut = messageCreater.CreateNotification("Skill not ready! CD: " + source.Skills[skillID].GetCooldown());
                msgIn.SenderConnection.SendMessage(msgOut, NetDeliveryMethod.Unreliable, 0);
            }
        }

        internal void SkillLeveled(NetIncomingMessage msgIn)
        {
            Character source = dataHandler.GetEntity(msgIn.SenderConnection) as Character;

            int skillID = msgIn.ReadInt16();
            if (source.Skills.ContainsKey(skillID))
                if (source.Skills[skillID].IsMaxLevel())
                {
                    NetOutgoingMessage msgOut = messageCreater.CreateNotification("Skill already max! Level: " + source.Skills[skillID].GetLevel());
                    msgIn.SenderConnection.SendMessage(msgOut, NetDeliveryMethod.Unreliable, 0);
                }
                else
                    source.Skills[skillID].LevelUp();
            else
            {
                int baseDamage = SkillList.Instance.Skills[skillID].SpellDamage(); //.Effects[40].Value;
                int cooldown = SkillList.Instance.Skills[skillID].CooldownGet();   //.Effects[130].Value;
                int skillType = SkillList.Instance.Skills[skillID].CooldownGet();   //.Effects[130].Value;
                source.Skills.Add(skillID, new SkillItem(skillID, baseDamage, cooldown, 1, skillType));
            }
        }
        private void SendInRange(NetOutgoingMessage msgOut)
        {
            throw new NotImplementedException();
        }
        public void ClientReady(NetIncomingMessage msgIn)
        {
            Character characterData = dataHandler.GetEntity(msgIn.SenderConnection) as Character;

            //To already ingame players
            NetOutgoingMessage msgOut = messageCreater.CreateNewEntityMessage(characterData);
            netServer.SendToAll(msgOut, msgIn.SenderConnection, NetDeliveryMethod.ReliableOrdered, 1);

            //To new connected player
            foreach (var character in dataHandler.entitiesByConnection)
            {
                if (character.Key != msgIn.SenderConnection)
                {
                    msgOut = messageCreater.CreateNewEntityMessage((Character)character.Value);
                    msgIn.SenderConnection.SendMessage(msgOut, NetDeliveryMethod.ReliableOrdered, 1);
                }
            }
            ClientMobSpawn(msgIn.SenderConnection);
        }

        public void ClientSkillInformation(NetIncomingMessage msgIn)
        {
            Character characterData = dataHandler.GetEntity(msgIn.SenderConnection) as Character;
            NetOutgoingMessage msgOut = messageCreater.SkillInformation(characterData.Skills);
            msgIn.SenderConnection.SendMessage(msgOut, NetDeliveryMethod.ReliableOrdered, 1);
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
            NetOutgoingMessage msgOut = messageCreater.CreateNewMobMessage(connection);
            connection.SendMessage(msgOut, NetDeliveryMethod.ReliableOrdered, 1);
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
            foreach (var character in dataHandler.entitiesByConnection)
            {
                if (character.Value.EntityName.ToLower() == to.ToLower())
                {
                    character.Key.SendMessage(msgOut, NetDeliveryMethod.ReliableUnordered, 0);
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

            CharacterWrapper data = dataHandler.waitingForAuth.Find(x => x.character.EntityID == characterId);

            if (dataHandler.CheckLoginToken(clientLoginToken, characterId))
            {
                msgIn.SenderConnection.Approve();
                Console.WriteLine("Authenticated!");

                Character newCharacter = LoadCharacterFrom(data);
                dataHandler.AddEntity(newCharacter, msgIn.SenderConnection);

                dataHandler.waitingForAuth.Remove(data);
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
            dataHandler.entitiesByID[id].transform.position = new Vector3(x, y, z);
        }
        public void SendMovementMessages()
        {
            NetOutgoingMessage msgOut;
            foreach (var character in dataHandler.entitiesByID.Values)
            {
                msgOut = messageCreater.MovementMessage(character);
                //if (dataHandler.entitiesByID.Count > 0)
                netServer.SendToAll(msgOut, NetDeliveryMethod.UnreliableSequenced, 1);
            }
        }
        public void SendLogoutMessages(int id)
        {
            NetOutgoingMessage msgOut;
            msgOut = messageCreater.LogoutMessage(id);
            //if (dataHandler.entitiesByID.Count > 0)
            netServer.SendToAll(msgOut, NetDeliveryMethod.UnreliableSequenced, 1);
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
        private Character LoadCharacterFrom(CharacterWrapper data)
        {
            Character character = GameObject.Instantiate(characterEntity).GetComponent<Character>();
            character.name = data.character.EntityName;
            character.EntityName = data.character.EntityName;
            character.EntityID = data.character.EntityID;
            character.AccountID = data.character.AccountID;
            character.EntityGold = data.character.EntityGold;
            character.EntityHealth = data.character.EntityHealth;
            character.EntityMaxHealth = data.character.EntityMaxHealth;
            character.EntityMana = data.character.EntityMana;
            character.EntityLevel = data.character.EntityLevel;
            character.CharacterType = data.character.CharacterType;
            character.transform.position = data.position;
            character.EntityBaseArmor = 60;
            character.EntityBaseMagicResist = 60;

            character.Skills = data.character.Skills;

            return character;
        }
    }
}