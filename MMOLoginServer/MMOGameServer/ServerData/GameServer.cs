using Lidgren.Network;

namespace MMOLoginServer.ServerData
{
    class GameServer
    {
        public string publicKey;
        public NetConnection connection;

        public string ip;
        public int port;
        public string name;

        public GameServer(NetConnection conn)
        {
            connection = conn;
        }
    }
}
