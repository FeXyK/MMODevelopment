using MMOGameServer;
using System;
using System.Threading;
using UnityEngine;

public class WorldServerManager : MonoBehaviour
{
    static Thread areaServerThread;
    WorldServerCore worldServer;
    string worldServerConfigFile = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\MMOConfig\WorldServerConfig.txt";
    string areaServerConfigFile = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\MMOConfig\AreaServerConfig.txt";

    void Start()
    {

        worldServer = new WorldServerCore();
        worldServer.areaServer = new AreaServerCore();

        worldServer.Initialize(worldServerConfigFile);
        worldServer.ConnectToLoginServer("127.0.0.1", 52221);
        worldServer.areaServer.Initialize(areaServerConfigFile);


        areaServerThread = new Thread(new ThreadStart(WorkerThread));
        areaServerThread.IsBackground = true;
        areaServerThread.Start();
    }
    private void WorkerThread()
    {
        worldServer.areaServer.StartServer(60);
    }
    void Update()
    {
        worldServer.ReceiveMessages();
        worldServer.Update();
    }
}
