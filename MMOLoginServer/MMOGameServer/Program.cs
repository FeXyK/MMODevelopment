using System;
using Lidgren.Network.ServerFiles;
using MMOLoginServer.LoginServerLogic;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Collections.Generic;

namespace MMOLoginServer
{
    class Program
    {
        static LoginServerCore loginMaster;

        const string LOGIN_SERVER_NAME = "NetLidgrenLogin";
        const int LOGIN_SERVER_PORT = 52222;
        const int LOGIN_SERVER_FRAMERATE = 5;
        const bool DEBUG_ENABLED = true;
        static List<ConnectionData> gameServers;
        static void Main(string[] args)
        {
            Debug.enable = DEBUG_ENABLED;
            gameServers = new List<ConnectionData>();
            ConnectionData gameServerData = new ConnectionData();
            gameServerData.ip = "127.0.0.1";
            gameServerData.port = 52221;
            gameServers.Add(gameServerData);
            loginMaster = new LoginServerCore(); 

            loginMaster.Initialize(LOGIN_SERVER_NAME, LOGIN_SERVER_PORT);
            loginMaster.ConnectToGameServerList(gameServers);
            loginMaster.StartServer(LOGIN_SERVER_FRAMERATE);

        }
    }
}
