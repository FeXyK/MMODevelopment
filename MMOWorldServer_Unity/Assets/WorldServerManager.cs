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
    static Thread areaServerThread;
    WorldServerCore worldServer;
    string worldServerConfigFile = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\MMOConfig\WorldServerConfig.txt";
    string areaServerConfigFile = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\MMOConfig\AreaServerConfig.txt";
    void Start()
    {
        worldServer = new WorldServerCore();
        worldServer.areaServer = new AreaServerCore();

        worldServer.Initialize(worldServerConfigFile);
        bool success = worldServer.ConnectToLoginServer("127.0.0.1", 52221);
        if (success)
        {
            worldServer.areaServer.Initialize(areaServerConfigFile);

            areaServerThread = new Thread(new ThreadStart(WorkerThread));
            areaServerThread.IsBackground = true;
            areaServerThread.Start();
        }
        else
        {
            Debug.LogError("Unable to connect to login server");
        }
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
    private void OnApplicationQuit()
    {
        worldServer.netPeer.Shutdown("bye");
        worldServer.areaServer.netPeer.Shutdown("byte");
    }
}
