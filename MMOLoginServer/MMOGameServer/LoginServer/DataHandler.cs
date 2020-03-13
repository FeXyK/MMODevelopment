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

        public List<ConnectionData> newConnections = new List<ConnectionData>();
        public List<ConnectionData> accounts = new List<ConnectionData>();
        public List<ConnectionData> worldServers = new List<ConnectionData>();
        public string gameServerKey = "HARDCODEDKEY";
        internal string loginServerKey = "HARDCODEDLOGINKEY";

        public DataHandler()
        {
            Instance = this;
        }
        public ConnectionData GetNewConnection(NetIncomingMessage msgIn)
        {
            NetConnection connection = msgIn.SenderConnection;
            foreach (var newConnection in newConnections)
            {
                if (newConnection.connection == connection)
                {
                    Console.WriteLine("Connection found");
                    return newConnection;
                }
            }
            Console.WriteLine("Connection not in new connections");
            return null;
        }
        public ConnectionData GetWorldServer(NetIncomingMessage msgIn)
        {
            NetConnection connection = msgIn.SenderConnection;
            foreach (var server in worldServers)
            {
                if (server.connection == connection)
                {
                    return server;
                }
            }
            return null;
        }
        public ConnectionData GetWorldServer(string worldServerName)
        {
            foreach (var server in worldServers)
            {
                if (server.name == worldServerName)
                {
                    return server;
                }
            }
            return null;
        }
        public ConnectionData GetAccount(NetIncomingMessage msgIn)
        {
            NetConnection connection = msgIn.SenderConnection;
            foreach (var acc in accounts)
            {
                if (connection == acc.connection)
                {
                    return acc;
                }
            }
            return null;
        }
        public void ClearConnections()
        {
            ConnectionData acc = null;
            foreach (var account in accounts)
            {
                if (account.connection.Status == NetConnectionStatus.Disconnected)
                {
                    acc = account;
                }
            }
            if (acc != null)
                accounts.Remove(acc);
        }
    }

}
