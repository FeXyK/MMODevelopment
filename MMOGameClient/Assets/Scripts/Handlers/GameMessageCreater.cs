using Assets.Scripts.Character;
using Assets.Scripts.UI;
using Assets.Scripts.UI.UIItems;
using Lidgren.Network;
using Lidgren.Network.Message;
using Lidgren.Network.ServerFiles;
using System;
using UnityEngine;

namespace Assets.Scripts.Handlers
{
    class GameMessageCreater

    {
        private NetClient netClient;

        public GameMessageCreater(NetClient netClient)
        {
            this.netClient = netClient;
        }
        public NetOutgoingMessage CreateSkillCast(SkillItem skill, EntityContainer target)
        {
            NetOutgoingMessage msgOut = netClient.CreateMessage();
            msgOut.Write((byte)MessageType.StartSkillCast);
            msgOut.Write(target.entity.id, 16);
            msgOut.Write(skill.ID, 16);
            return msgOut;
        }
        public NetOutgoingMessage ClientReady()
        {
            NetOutgoingMessage msgReady = netClient.CreateMessage();
            msgReady.Write((byte)MessageType.ClientReady);
            return msgReady;
        }

        internal NetOutgoingMessage SkillInformation()
        {
            NetOutgoingMessage msgOut = netClient.CreateMessage();
            msgOut.Write((byte)MessageType.SkillListInformation);
            return msgOut;
        }

        public NetOutgoingMessage PositionUpdate(int id, Vector3 position)
        {
            NetOutgoingMessage msgOut = netClient.CreateMessage();
            msgOut.Write((byte)MessageType.CharacterMovement);
            msgOut.Write(id, 16);
            msgOut.Write(position.x);
            msgOut.Write(position.y);
            msgOut.Write(position.z);
            //msgOut.Write(dataHandler.myCharacter.transform.rotation.eulerAngles.y);
            return msgOut;
        }

        public NetOutgoingMessage PrivateChatMessage(string characterName, string[] msg)
        {
            NetOutgoingMessage msgOut = netClient.CreateMessage();
            msgOut.Write((byte)MessageType.PrivateChatMessage);
            msgOut.Write(characterName);
            msgOut.Write(msg[0]);
            msgOut.Write(msg[1]);
            return msgOut;
        }
        public NetOutgoingMessage ChatMessage(string characterName, string msg)
        {
            NetOutgoingMessage msgOut = netClient.CreateMessage();
            msgOut.Write((byte)MessageType.PublicChatMessage);
            msgOut.Write(characterName);
            msgOut.Write(msg);
            return msgOut;
        }
        public NetOutgoingMessage AdminChatMessage(string msg)
        {
            NetOutgoingMessage msgOut = netClient.CreateMessage();
            msgOut.Write((byte)MessageType.AdminChatMessage);
            msgOut.Write(msg);
            return msgOut;
        }
        public NetOutgoingMessage ConnectingMessage(int id, byte[] authToken, string publicKey)
        {
            NetOutgoingMessage msgOut = netClient.CreateMessage();
            msgOut.Write((byte)MessageType.ClientAuthentication);
            PacketHandler.WriteEncryptedByteArray(msgOut, authToken, publicKey);
            msgOut.Write(id, 16);
            return msgOut;
        }

        internal NetOutgoingMessage LevelUpSkill(UIItem item)
        {
            NetOutgoingMessage msgOut = netClient.CreateMessage();
            msgOut.Write((byte)MessageType.SkillLeveled);
            msgOut.Write(item.ID, 16);
            //msgOut.Write(level, 16);
            return msgOut;
        }

        internal NetOutgoingMessage Use(UIItem item)
        {
            NetOutgoingMessage msgOut = netClient.CreateMessage();
            msgOut.Write((byte)MessageType.SkillLeveled);
            msgOut.Write(item.ID, 16);
            //msgOut.Write(level, 16);
            return msgOut;
        }

        internal NetOutgoingMessage TakeOn(UIItem item)
        {
            NetOutgoingMessage msgOut = netClient.CreateMessage();
            msgOut.Write((byte)MessageType.SkillLeveled);
            msgOut.Write(item.ID, 16);
            //msgOut.Write(level, 16);
            return msgOut;
        }
    }
}
