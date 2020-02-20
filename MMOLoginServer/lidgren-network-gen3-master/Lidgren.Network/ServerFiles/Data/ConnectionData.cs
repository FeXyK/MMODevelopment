using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lidgren.Network.ServerFiles
{
    public class ConnectionData
    {
        public string ip;
        public string name;
        public int port;
        public ConnectionType type;
        public NetConnection connection;
        public string publicKey;
    }
}
