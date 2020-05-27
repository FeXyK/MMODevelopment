using System.Text;
using System.Security.Cryptography;
using System;

namespace Lidgren.Network.ServerFiles
{
    public static class DataEncryption
    {
        public static RSACryptoServiceProvider csp = new RSACryptoServiceProvider(2048);
        public static string privateKey = csp.ToXmlString(true);
        public static string publicKey = csp.ToXmlString(false);

        public static string SaltHash(string inputString)
        {
            byte[] data = Encoding.UTF8.GetBytes(inputString);
            data = new SHA256Managed().ComputeHash(data);
            return Encoding.UTF8.GetString(data);
        }
        public static byte[] RSAEncrypt(string data, string RSAPublicKey)
        {
            csp.FromXmlString(RSAPublicKey);

            byte[] dataRaw = Encoding.UTF8.GetBytes(data);
            byte[] dataEncrypted = csp.Encrypt(dataRaw, false);

            return dataEncrypted;
        }
        public static byte[] RSAEncrypt(byte[] data, string RSAPublicKey)
        {
            csp.FromXmlString(RSAPublicKey);


            Console.WriteLine(data.Length);
            Console.WriteLine(RSAPublicKey.Length);
            byte[] dataEncrypted = csp.Encrypt(data, false);


            return dataEncrypted;
        }
        public static byte[] RSAEncrypt(string data)
        {
            csp.FromXmlString(publicKey);

            byte[] dataRaw = Encoding.UTF8.GetBytes(data);
            byte[] dataEncrypted = csp.Encrypt(dataRaw, false);

            return dataEncrypted;
        }
        public static byte[] RSAEncrypt(byte[] data)
        {
            csp.FromXmlString(publicKey);

            byte[] dataEncrypted = csp.Encrypt(data, false);

            return dataEncrypted;
        }
        public static byte[] RSADecrypt(byte[] data)
        {
            byte[] dataDecrypted;
            csp.FromXmlString(privateKey);
            try
            {
                dataDecrypted = csp.Decrypt(data, false);

            }
            catch (Exception)
            {
                return null;
            }
            return dataDecrypted;
        }
        public static byte[] HashString(string dataString)
        {
            byte[] data = Encoding.UTF8.GetBytes(dataString);
            return new SHA256Managed().ComputeHash(data);
        }
    }
}
