using Lidgren.Network;
using Lidgren.Network.ServerFiles;
using System;
using System.Collections.Generic;
using System.Text;

namespace MMOLoginServer
{
    public class DataHandler
    {
        public static DataHandler Instance;

        public List<ClientData> accounts = new List<ClientData>();
        public List<GameServerData> gameServers = new List<GameServerData>();
        public string gameServerKey = "HARDCODEDKEY";

        public DataHandler()
        {
            Instance = this;
        }
        public ClientData GetAccount(NetConnection connection)
        {
            foreach (var acc in accounts)
            {
                if (connection == acc.connection)
                {
                    return acc;
                }
            }
            return null;
        }
    }

}
