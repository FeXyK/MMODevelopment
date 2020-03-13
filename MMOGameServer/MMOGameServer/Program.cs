using System;
using System.IO;
using System.Threading;

namespace MMOGameServer
{
    class Program
    {
        static Thread areaServerThread;
        static int AREA_SERVER_FRAMERATE = 60;
        static int WORLD_SERVER_FRAMERATE = 60;
        static string worldServerConfigFile = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\MMOConfig\WorldServerConfig.txt";
        static string areaServerConfigFile = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\MMOConfig\AreaServerConfig.txt";
        private static WorldServerCore worldServer;
        static void Main(string[] args)
        {
            worldServer = new WorldServerCore();
            worldServer.Initialize(worldServerConfigFile);

            worldServer.areaServer = new AreaServerCore();
            worldServer.areaServer.Initialize(areaServerConfigFile);

            areaServerThread = new Thread(new ThreadStart(WorkerThread));
            areaServerThread.IsBackground = true;
            areaServerThread.Start();

            Console.WriteLine("Area server started");
            worldServer.ConnectToLoginServer("127.0.0.1", 52221);
            worldServer.StartServer(WORLD_SERVER_FRAMERATE);
        }
        private static void WorkerThread()
        {
            worldServer.areaServer.StartServer(AREA_SERVER_FRAMERATE);
        }
    }
}