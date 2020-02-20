using System;

namespace MMOGameServer
{
    class Program
    {
        const int SERVER_FRAMERATE = 60;
        const int LOGIN_SERVER_FRAMERATE = 10;
        const string SERVER_NAME = "NetLidgrenLogin";
        const int SERVER_PORT = 52220;
        const string LOGIN_SERVER_IP = "127.0.0.1";
        const int LOGIN_SERVER_PORT = 52221;
        private static GameServerCore gameServer;
        static void Main(string[] args)
        {
            //GSM = new GameServerMaster();
            //GSM.InitializeGameServer(SERVER_NAME, SERVER_PORT);
            //GSM.StartGameServer(SERVER_FRAMERATE, LOGIN_SERVER_IP, LOGIN_SERVER_PORT);

            gameServer = new GameServerCore();
            gameServer.Initialize(SERVER_NAME, LOGIN_SERVER_PORT);
            gameServer.StartServer(LOGIN_SERVER_FRAMERATE);

            
        }
    }
}
