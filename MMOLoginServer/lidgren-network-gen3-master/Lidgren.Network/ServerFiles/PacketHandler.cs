using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace Lidgren.Network.ServerFiles
{
    public class PacketHandler
    {
        public static byte[] ReadByteArray(NetIncomingMessage msgIn)
        {
            int length = msgIn.ReadInt16();
            return msgIn.ReadBytes(length);
        }
        public static void WriteByteArray(NetOutgoingMessage msgOut, byte[] msgData)
        {
            msgOut.Write(msgData.Length, 16);
            msgOut.Write(msgData);
        }
        public static byte[] ReadEncryptedByteArray(NetIncomingMessage msgIn)
        {
            int length = msgIn.ReadInt16();
            byte[] data = msgIn.ReadBytes(length);
            return DataEncryption.RSADecrypt(data);
        }
        public static void WriteEncryptedByteArray(NetOutgoingMessage msgOut, byte[] msgData, string publicKey = null)
        {
            if (publicKey == null)
                publicKey = DataEncryption.publicKey;

            msgData = DataEncryption.RSAEncrypt(msgData, publicKey);
            msgOut.Write(msgData.Length, 16);
            msgOut.Write(msgData);
        }
        public static void WriteEncryptedByteArray(NetOutgoingMessage msgOut, string msgData, string publicKey = null)
        {
            if (publicKey == null)
                publicKey = DataEncryption.publicKey;

            byte[] data = DataEncryption.RSAEncrypt(msgData, publicKey);
            msgOut.Write(data.Length, 16);
            msgOut.Write(data);
        }
    }
}
