using Assets.Scripts.GameNetworkScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameManager : MonoBehaviour
{
    string PEER_NAME = "NetLidgrenLogin";
    ClientGameManager gameManager;
    MenuController menuController;

    void Start()
    {
        gameManager = new ClientGameManager();
        gameManager.Initialize(PEER_NAME);
        menuController = FindObjectOfType<MenuController>();
        menuController.ChatWindow.gameObject.SetActive(true);
        menuController.messageHandler = gameManager.messageHandler;
    }

    void Update()
    {
        gameManager.Update();
        if (Input.GetKeyDown(KeyCode.F9) )
            gameManager.HideNameTags();
    }
}
