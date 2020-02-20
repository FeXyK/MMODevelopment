using Lidgren.Network.ServerFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
