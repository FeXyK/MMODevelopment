
namespace Lidgren.Network.ServerFiles.Data
{
    public class ConnectionData
    {
        public string ip;
        public string name;
        public int port;
        public ConnectionType type;
        public NetConnection connection;
        public string publicKey;
        public bool admin = false;
        public bool authenticated = false;
        public byte[] authToken;
        public byte[] salt;
        public byte[] password;
        public int id;
        public ConnectionData()
        {
            type = ConnectionType.Client;
        }
    }
}
