using System;
using System.Text;

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
        public static string ReadEncryptedString(NetIncomingMessage msgIn)
        {
            int length = msgIn.ReadInt16();
            byte[] data = msgIn.ReadBytes(length);
            return Encoding.UTF8.GetString(DataEncryption.RSADecrypt(data));
        }
        public static int ReadEncryptedInt(NetIncomingMessage msgIn)
        {
            int length = msgIn.ReadInt16();
            byte[] data = msgIn.ReadBytes(length);
            return BitConverter.ToInt32(DataEncryption.RSADecrypt(data), 0);
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
