using Lidgren.Network;
using Lidgren.Network.ServerFiles;
using Lidgren.Network.ServerFiles.Data;
using MMOLoginServer.ServerData;
using System;
using System.Collections.Generic;
using System.Text;

namespace MMOGameServer
{
    public class AreaDataHandler
    {
        public string serverName = "Europe";
        public Dictionary<int, CharacterData> characters = new Dictionary<int, CharacterData>();
        public List<NetConnection> netConnections = new List<NetConnection>();
        public List<ConnectionData> connections = new List<ConnectionData>();

        public List<AuthenticationTokenData> loginTokens = new List<AuthenticationTokenData>();
        public AreaDataHandler()
        {

        }
        public CharacterData GetCharacter(NetIncomingMessage msgIn)
        {
            foreach (var character in characters)
            {
                if (msgIn.SenderConnection == character.Value.connection)
                {
                    return character.Value;
                }
            }
            return null;
        }

        public AuthenticationTokenData CheckLoginToken(byte[] clientLoginToken, string username)
        {
            foreach (var token in loginTokens)
            {
                Console.WriteLine("AUTH TOKENS: ");
                Console.WriteLine(BitConverter.ToString(token.token));
                Console.WriteLine(BitConverter.ToString(clientLoginToken));
                Console.WriteLine(username + " END");
                //Console.WriteLine(token.characterData.name + " END");
                Console.WriteLine("---------------------");


                if (ByteArrayCompare(token.token, clientLoginToken)//token.expireDate > DateTime.Now &&
                                                                   //token.token == clientLoginToken
                    //&& token.characterData.name == username
                    )
                {
                    Console.WriteLine("ClientAuthenticated");
                    return token;
                }
            }
            return null;
        }
        public ConnectionData FindConnection(NetConnection senderConnection)
        {
            foreach (var connection in connections)
            {
                if (connection.connection == senderConnection)
                {
                    return connection;
                }
            }
            return null;
        }
        public void RemoveExpiredLoginTokens()
        {
            AuthenticationTokenData remove = null;
            foreach (var token in loginTokens)
            {
                if (DateTime.Parse(token.expireDate) < DateTime.Now)
                {
                    remove = token;
                    break;
                }
            }
            if (remove != null)
                loginTokens.Remove(remove);
        }

        [System.Runtime.InteropServices.DllImport("msvcrt.dll", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        static extern int memcmp(byte[] b1, byte[] b2, long count);
        public static bool ByteArrayCompare(byte[] b1, byte[] b2)
        {
            // Validate buffers are the same length.
            // This also ensures that the count does not exceed the length of either buffer.  
            return b1.Length == b2.Length && memcmp(b1, b2, b1.Length) == 0;
        }
    }
}
