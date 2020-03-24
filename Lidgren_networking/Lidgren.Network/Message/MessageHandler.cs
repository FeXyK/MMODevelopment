using Lidgren.Network.ServerFiles;
using Lidgren.Network.ServerFiles.Data;
using System;

namespace Lidgren.Network.Message
{
    public class MessageHandler
    {
        public NetOutgoingMessage CreateRSAKeyMessage(NetPeer netPeer, ConnectionType connectionType, string gameServerName = null)
        {
            NetOutgoingMessage msgOut = netPeer.CreateMessage();
            msgOut.Write((byte)MessageType.KeyExchange);
            msgOut.Write((byte)connectionType);
            msgOut.Write(DataEncryption.publicKey);
            if (gameServerName != null)
                msgOut.Write(gameServerName);
            Console.WriteLine(gameServerName);
            return msgOut;
        }
        public void ReadRSAKeyMessage(NetIncomingMessage msgIn)
        {

        }
    }
}
