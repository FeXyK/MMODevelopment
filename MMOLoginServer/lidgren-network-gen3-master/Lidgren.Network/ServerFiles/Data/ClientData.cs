using MMOLoginServer.ServerData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lidgren.Network.ServerFiles
{
    public class ClientData : ConnectionData
    {
        public int id;
        public byte[] salt;
        public byte[] password;
        public byte[] authToken;
        public List<CharacterData> characters = new List<CharacterData>();
        public ClientData()
        {
            type = ConnectionType.Client;
        }
    }
}
