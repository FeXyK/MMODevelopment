using System;
using System.IO;
using System.Collections.Generic;
using Lidgren.Network.ServerFiles.Data;

namespace MMOLoginServer
{
    class Program
    {
        static LoginServerCore loginMaster;

        const string LOGIN_SERVER_NAME = "NetLidgrenLogin";
        static int LOGIN_SERVER_PORT = 52221;
        static int LOGIN_SERVER_FRAMERATE = 5;
        static bool DEBUG_ENABLED = true;
        static List<ConnectionData> gameServers;
        static void Main(string[] args)
        {

            gameServers = new List<ConnectionData>();
            Debug.enable = DEBUG_ENABLED;
            ConnectionData gameServerData = new ConnectionData();
            string[] lines = File.ReadAllLines(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\MMOConfig\LoginConfig.txt");
            string[] data;
            foreach (var line in lines)
            {
                data = line.Split('=');
                switch (data[0].Trim().ToLower())
                {
                    case "serverport":
                        LOGIN_SERVER_PORT = int.Parse(data[1]);
                        break;
                    case "framerate":
                        LOGIN_SERVER_FRAMERATE = int.Parse(data[1]);
                        break;
                    case "debug":
                        DEBUG_ENABLED = bool.Parse(data[1]);
                        break;
                    case "gameserver":
                        string[] conn;
                        conn = data[1].Split(':');
                        gameServerData.ip = conn[0];
                        gameServerData.port = int.Parse(conn[1]);
                        gameServers.Add(gameServerData);
                        break;

                }
            }
            loginMaster = new LoginServerCore();

            loginMaster.Initialize(LOGIN_SERVER_NAME, LOGIN_SERVER_PORT);
            loginMaster.StartServer(LOGIN_SERVER_FRAMERATE);
        }
    }
}
