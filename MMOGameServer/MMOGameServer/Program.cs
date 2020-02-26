using System;

namespace MMOGameServer
{
    class Program
    {
        const int SERVER_FRAMERATE = 60;
        const string SERVER_NAME = "NetLidgrenLogin";
        const int GAME_SERVER_PORT = 52242;
        private static GameServerCore gameServer;
        static void Main(string[] args)
        {
            gameServer = new GameServerCore();
            gameServer.Initialize(SERVER_NAME, GAME_SERVER_PORT);
            gameServer.StartServer(SERVER_FRAMERATE);
        }
    }
}
