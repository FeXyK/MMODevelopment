using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lidgren.Network.ServerFiles
{
    public class GameServerData: ConnectionData
    {
        public GameServerData()
        {
            type = ConnectionType.GameServer;
        }
    }
}
