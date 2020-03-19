using MMOGameServer;
using System;
using System.Threading;
using UnityEngine;
using System.Data;
using System.IO;
using System.Diagnostics.PerformanceData;
using System.Text;

public class WorldServerManager : MonoBehaviour
{
    static Thread worldServerThread;
    WorldServerCore worldServer;
    string worldServerConfigFile = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\MMOConfig\WorldServerConfig.txt";
    string areaServerConfigFile = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\MMOConfig\AreaServerConfig.txt";
    void Start()
    {
        Application.targetFrameRate = 60;
        worldServer = new WorldServerCore();
        worldServer.areaServer = new AreaServerCore();
        worldServer.areaServer.Initialize(areaServerConfigFile);

        worldServer.Initialize(worldServerConfigFile);

        worldServerThread = new Thread(new ThreadStart(WorkerThread));
        worldServerThread.IsBackground = true;
        worldServerThread.Start();
        bool success = worldServer.ConnectToLoginServer("127.0.0.1", 52221);
        if (!success)
        {
            Debug.LogError("Unable to connect to login server");
        }
    }
    private void WorkerThread()
    {
        worldServer.StartServer(20);
    }
    void Update()
    {
        worldServer.areaServer.ReceiveMessages();
        worldServer.areaServer.Update();
    }
    private void OnApplicationQuit()
    {
        worldServer.netPeer.Shutdown("bye");
        worldServer.areaServer.netPeer.Shutdown("byte");
    }
}
