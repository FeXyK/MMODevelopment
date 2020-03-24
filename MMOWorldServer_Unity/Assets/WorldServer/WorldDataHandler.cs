using Lidgren.Network;
using Lidgren.Network.ServerFiles;
using Lidgren.Network.ServerFiles.Data;
using System.Collections.Generic;

namespace MMOGameServer.WorldServer
{
    class WorldDataHandler
    {
        internal ConnectionData loginServer;
        internal List<ConnectionData> newConnections = new List<ConnectionData>();
        internal List<ClientData> authenticatedConnections = new List<ClientData>();
        internal List<AuthenticationTokenData> authTokens = new List<AuthenticationTokenData>();

        public List<AreaServerCore> areaServers = new List<AreaServerCore>();

        internal string loginServerAuthToken = "HARDCODEDKEY";
        internal string worldServerName = "EUROPE";

        public WorldDataHandler()
        {

        }
        public ConnectionData GetNewConnection(NetConnection senderConnection)
        {
            foreach (var conn in newConnections)
            {
                if (conn.connection == senderConnection)
                    return conn;
            }
            return null;
        }

        internal ClientData GetAuthenticatedUser(NetConnection senderConnection)
        {
            foreach (var conn in authenticatedConnections)
            {
                if (conn.connection == senderConnection)
                    return conn;
            }
            return null;
        }
    }
}
