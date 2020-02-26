using Lidgren.Network;
using Lidgren.Network.ServerFiles;
using MMOLoginServer.ServerData;
using System;
using System.Collections.Generic;
using System.Text;

namespace MMOGameServer
{
    public class MessageCreater: Lidgren.Network.Message.MessageHandler
    {
        NetServer netServer;
        public MessageCreater(NetServer _netServer)
        {
            netServer = _netServer;
        }

        public NetOutgoingMessage MovementMessage(CharacterData character)
        {
            NetOutgoingMessage msgOut = netServer.CreateMessage();
            msgOut = netServer.CreateMessage();
            msgOut.Write((byte)MessageType.CharacterMovement);
            msgOut.Write(character.id, 16);
            msgOut.Write(character.position.X);
            msgOut.Write(character.position.Y);
            msgOut.Write(character.position.Z);
            msgOut.Write(character.rotation);
            return msgOut;
        }
        public NetOutgoingMessage CreateNewCharacterMessage(CharacterData character)
        {
            NetOutgoingMessage msgOut = netServer.CreateMessage();
            msgOut.Write((byte)MessageType.NewCharacter);
            msgOut.Write(character.id, 16);
            msgOut.Write(character.level, 16);
            msgOut.Write(character.currentHealth, 16);
            msgOut.Write(character.characterType, 16);
            msgOut.Write(character.name);
            return msgOut;
        }
    }
}
