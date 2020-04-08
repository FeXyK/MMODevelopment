using Assets.Scripts.Character;
using Assets.Scripts.GameNetworkScripts;
using System;
using System.IO;
using UnityEngine;

public class InGameManager : MonoBehaviour
{
    string PEER_NAME = "NetLidgrenLogin";
    bool IS_SERVER = false;
    bool SIMULATE_LATENCY = true;
    ClientGameManager gameManager;
    //MenuController menuController;

    void Awake()
    {
        string[] lines = File.ReadAllLines(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\MMOConfig\ClientConfig.cfg");
        string[] data;
        foreach (var line in lines)
        {
            data = line.Split('=');
            switch (data[0].Trim().ToLower())
            {
                case "peersimulatelatency":
                    SIMULATE_LATENCY = bool.Parse(data[1]);
                    break;
            }
        }
        //Debug.Log(SIMULATE_LATENCY);
        gameManager = new ClientGameManager();
        gameManager.Initialize(PEER_NAME, 0, IS_SERVER, SIMULATE_LATENCY);

        gameManager.SpawnPlayer();


        //menuController = FindObjectOfType<MenuController>();
        //menuController.ChatWindow.gameObject.SetActive(true);
        //menuController.messageHandler = gameManager.messageHandler;
    }

    void Update()
    {
        gameManager.Update();
    }
}
