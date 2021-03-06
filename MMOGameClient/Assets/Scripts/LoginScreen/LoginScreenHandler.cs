﻿using Assets.Scripts.Handlers;
using Assets.Scripts.LoginNetworkScripts;
using Assets.Scripts.WorldServerNetworkScripts;
using Lidgren.Network;
using Lidgren.Network.ServerFiles;
using System;
using System.Collections;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginScreenHandler : MonoBehaviour
{
    static string configFile = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\MMOConfig\ClientConfig.cfg";

    public GameObject SwitchForm;
    public GameObject LoginForm;
    public GameObject RegisterForm;
    public GameObject ServerSelectForm;
    public GameObject CharacterSelectForm;
    public GameObject CharacterCreateForm;

    LoginMessageHandler loginMessageHandler;
    WorldServerMessageHandler worldMessageHandler;
    LoginClientManager loginClient;
    WorldServerNetwork worldServerClient;

    Thread connectingThread;

    bool flag = false;
    float tickTime;

    float playButtonTimer;
    float playButtonTimerDefault = 1f;

    float worldServerAuthTimer;
    float worldServerAuthTimerDefault = 1f;
    String publicKey;
    private void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        worldServerClient = new WorldServerNetwork();
        loginClient = new LoginClientManager();

        loginClient.Initialize(configFile);
        worldServerClient.Initialize(configFile);

        loginMessageHandler = LoginMessageHandler.GetInstance();
        worldMessageHandler = WorldServerMessageHandler.GetInstance();
        publicKey = DataEncryption.publicKey;
    }
    private void Update()
    {
        if (LoginForm != null)
        {
            if (connectingThread != null)
            {
                LoginForm.GetComponent<FormHandler>().SetInteractable(!connectingThread.IsAlive);
            }
            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (LoginForm.activeSelf)
                    Login();
                if (RegisterForm.activeSelf)
                    Register();
                if (CharacterSelectForm.activeSelf)
                    PlayCharacter();
                if (ServerSelectForm.activeSelf)
                    LoginWorldServer();
            }
        }
        if (loginClient != null)
            loginClient.ReceiveMessages();
        if (worldServerClient != null)
            worldServerClient.ReceiveMessages();

        if (worldServerClient.dataHandler.authenticationToken != null && flag == false)
        {
            Debug.Log("RUNNING: " + worldServerClient.dataHandler.authenticationToken);
            worldServerClient.SetupConnection(worldServerClient.dataHandler.selectedWorldServer.ip, worldServerClient.dataHandler.selectedWorldServer.port);
            flag = true;
        }

        tickTime += Time.deltaTime;
        if (tickTime > 10f)
        {
            loginMessageHandler.SendAlive();
            worldMessageHandler.SendAlive();
            tickTime = 0;
        }
    }
    public void LoginWorldServer()
    {
        if (playButtonTimer - Time.time < 0)
        {
            playButtonTimer = playButtonTimerDefault;
            loginMessageHandler.WorldServerAuthenticationTokenRequest();
        }
    }
    private void LoginThread()
    {
        LoginMessageHandler.GetInstance().SetupConnection();
        LoginMessageHandler.GetInstance().Login();
    }
    public void Login()
    {
        var atnow = DateTime.Now;


        if (connectingThread != null && !connectingThread.IsAlive)
        {
            connectingThread.Abort();
            connectingThread = null;
        }
        if (connectingThread == null)
        {
            connectingThread = new Thread(new ThreadStart(LoginThread));
            //loginMessageHandler.SetupConnection(SERVER_IP, SERVER_PORT);
            //loginMessageHandler.Login();
            connectingThread.IsBackground = true;
            connectingThread.Start();
        }
    }
    private void RegisterThread()
    {
        loginMessageHandler.SetupConnection();
        loginMessageHandler.Register();
    }

    public void Register()
    {
        SwitchForms();
        if (connectingThread != null && !connectingThread.IsAlive)
        {
            connectingThread.Abort();
            connectingThread = null;
        }
        if (connectingThread == null)
        {
            connectingThread = new Thread(new ThreadStart(RegisterThread));

            //loginMessageHandler.SetupConnection(SERVER_IP, SERVER_PORT);
            //loginMessageHandler.Login();
            connectingThread.IsBackground = true;
            connectingThread.Start();
        }
    }
    public void ClearCharacterSelection()
    {
    }
    public void SwitchForms()
    {
        LoginForm.SetActive(!LoginForm.activeSelf);
        RegisterForm.SetActive(!RegisterForm.activeSelf);

        if (!LoginForm.activeSelf)
            SwitchForm.GetComponentInChildren<TMP_Text>().text = "Login";
        else
            SwitchForm.GetComponentInChildren<TMP_Text>().text = "Registration";
    }
    public void PlayCharacter()
    {
        if (playButtonTimer - Time.time < 0)
        {
            playButtonTimer = playButtonTimerDefault;
            worldMessageHandler.PlayCharacter();
        }
    }
    public void ShowCharacterCreation()
    {
        ClearCharacterSelection();
        CharacterCreateForm.SetActive(true);
    }
    public void CreateCharacter()
    {
        ClearCharacterSelection();
        worldMessageHandler.CreateCharacter();
    }
    public void DeleteCharacter()
    {
        ClearCharacterSelection();
        worldMessageHandler.DeleteCharacter();
    }
}
