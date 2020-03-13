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
        public List<CharacterData> characters;
        public ClientData(ConnectionData connectionData)
        {
            this.name = connectionData.name;
            this.ip = connectionData.ip;
            this.port = connectionData.port;
            this.type = connectionData.type;
            this.publicKey = connectionData.publicKey;
            this.id = connectionData.id;
            this.authToken = connectionData.authToken;
            this.authenticated = connectionData.authenticated;
            this.admin = connectionData.admin;
            this.connection = connectionData.connection;
            characters = new List<CharacterData>();
        }
    }
}
