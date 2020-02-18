using System.Collections.Generic;
using Lidgren.Network;

namespace MMOLoginServer.ServerData
{
    public class Account
    {
        public string publicKey;
        public NetConnection connection;
        public List<Character> characters = new List<Character>();

        public int id;
        public string username;
        public byte[] salt;
        public byte[] password;
        public Account(NetConnection conn)
        {
            connection = conn;
        }
        public Account()
        {

        }
    }
}
