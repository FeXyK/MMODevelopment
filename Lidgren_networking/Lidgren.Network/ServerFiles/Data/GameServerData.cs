

namespace Lidgren.Network.ServerFiles.Data
{
    public class GameServerData : ConnectionData
    {
        public int areaServerPort;
        public byte[] areaServerAuthToken;
        public GameServerData()
        {
            type = ConnectionType.GameServer;
        }
    }
}
