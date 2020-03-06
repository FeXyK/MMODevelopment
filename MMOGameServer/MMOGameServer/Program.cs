using System;
using System.IO;

namespace MMOGameServer
{
    class Program
    {
        static string SERVER_NAME = "NetLidgrenLogin";
        static int GAME_SERVER_PORT = 52242;
        static int SERVER_FRAMERATE = 60;
        static bool SIMULATE_LATENCY = true;
        private static GameServerCore gameServer;
        static void Main(string[] args)
        {
            string[] lines = File.ReadAllLines(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\MMOConfig\ServerConfig.txt");
            string[] data;
            foreach (var line in lines)
            {
                data = line.Split('=');
                switch (data[0].Trim().ToLower())
                {
                    case "serverport":
                        GAME_SERVER_PORT = int.Parse(data[1]);
                        break;
                    case "framerate":
                        SERVER_FRAMERATE = int.Parse(data[1]);
                        break;
                    case "simulatelag":
                        SIMULATE_LATENCY = bool.Parse(data[1]);
                        break;
                    case "debuglog":
                        Debug.enabled = bool.Parse(data[1]);
                        break;
                }
            }
            Console.WriteLine("Framerate: " + SERVER_FRAMERATE);
            Console.WriteLine("Port: " + GAME_SERVER_PORT);
            Console.WriteLine("SimulateLatency: " + SIMULATE_LATENCY);
            gameServer = new GameServerCore();
            gameServer.Initialize(SERVER_NAME, GAME_SERVER_PORT, true, SIMULATE_LATENCY);
            gameServer.StartServer(SERVER_FRAMERATE);
        }
    }
}