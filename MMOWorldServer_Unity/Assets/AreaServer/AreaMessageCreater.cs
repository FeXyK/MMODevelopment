using Assets.AreaServer.Entity;
using Assets.AreaServer.SkillSystem;
using Lidgren.Network;
using Lidgren.Network.Message;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MMOGameServer
{
    public class AreaMessageCreater : Lidgren.Network.Message.MessageHandler
    {
        NetServer netServer;
        AreaDataHandler dataHandler;
        public AreaMessageCreater(NetServer netServer, AreaDataHandler dataHandler)
        {
            this.dataHandler = dataHandler;
            this.netServer = netServer;
        }

        public NetOutgoingMessage MovementMessage(Entity character)
        {
            NetOutgoingMessage msgOut = netServer.CreateMessage();
            msgOut = netServer.CreateMessage();
            msgOut.Write((byte)MessageType.CharacterMovement);
            msgOut.Write(character.EntityID, 16);
            msgOut.Write(character.transform.position.x);
            msgOut.Write(character.transform.position.y);
            msgOut.Write(character.transform.position.z);
            return msgOut;
        }
        public NetOutgoingMessage CreateNewEntityMessage(Character character)
        {
            NetOutgoingMessage msgOut = netServer.CreateMessage();
            msgOut.Write((byte)MessageType.NewCharacter);
            msgOut.Write(character.EntityID, 16);
            msgOut.Write(character.EntityLevel, 16);
            msgOut.Write(character.EntityHealth, 16);
            msgOut.Write(character.EntityMaxHealth, 16);
            msgOut.Write((int)character.CharacterType, 16);
            msgOut.Write(character.EntityName);
            return msgOut;
        }

        public NetOutgoingMessage LogoutMessage(int id)
        {
            NetOutgoingMessage msgOut = netServer.CreateMessage();
            msgOut.Write((byte)MessageType.OtherCharacterRemove);
            msgOut.Write(id, 16);
            return msgOut;
        }
        public NetOutgoingMessage CreateNewMobMessage(NetConnection connection)
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
                    msgOut.Write(mob.EntityHealth, 16);
                    msgOut.Write(mob.EntityMaxHealth, 16);
                }
            }

            return msgOut;
        }
        public NetOutgoingMessage SkillCasted(int sourceID, int targetID, int skillID)
        {
            NetOutgoingMessage msgOut = netServer.CreateMessage();
            msgOut.Write((byte)MessageType.SkillCasted);
            msgOut.Write(sourceID, 16);
            msgOut.Write(targetID, 16);
            msgOut.Write(skillID, 16);
            return msgOut;
        }

        internal NetOutgoingMessage CreateNotification(string msg)
        {
            NetOutgoingMessage msgOut = netServer.CreateMessage();
            msgOut.Write((byte)MessageType.Notification);
            msgOut.Write(msg);
            return msgOut;
        }

        internal NetOutgoingMessage SkillInformation(Dictionary<int, SkillItem> skills)
        {
            NetOutgoingMessage msgOut = netServer.CreateMessage();
            msgOut.Write((byte)MessageType.SkillListInformation);
            msgOut.Write(skills.Count, 16);
            foreach (var skill in skills)
            {
                msgOut.Write(skill.Key, 16);
                msgOut.Write(skill.Value.GetLevel(), 16);
            }
            return msgOut;
        }

        internal NetOutgoingMessage SkillLeveled(int skillID, int Level)
        {
            NetOutgoingMessage msgOut = netServer.CreateMessage();
            msgOut.Write((byte)MessageType.SkillLeveled);
            msgOut.Write((byte)skillID);
            msgOut.Write((byte)Level);
            return msgOut;
        }
    }
}
