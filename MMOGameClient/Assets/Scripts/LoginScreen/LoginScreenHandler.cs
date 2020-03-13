using Assets.Scripts.Handlers;
using Assets.Scripts.LoginNetworkScripts;
using Assets.Scripts.WorldServerNetworkScripts;
using System;
using System.Data;
using TMPro;
using UnityEngine;

public class LoginScreenHandler : MonoBehaviour
{
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
    bool flag = false;

    static string configFile = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\MMOConfig\ClientConfig.cfg";
    string SERVER_IP = "127.0.0.1";
    int SERVER_PORT = 52221;
    float tickTime;
    private void Start()
    {
        worldServerClient = new WorldServerNetwork();
        loginClient = new LoginClientManager();

        loginClient.Initialize(configFile);
        worldServerClient.Initialize(configFile);

        loginMessageHandler = LoginMessageHandler.GetInstance();
        worldMessageHandler = WorldServerMessageHandler.GetInstance();
    }
    private void Update()
    {
        if (loginClient != null)
            loginClient.ReceiveMessages();
        if(worldServerClient != null)
            worldServerClient.ReceiveMessages();

        if (worldServerClient.dataHandler.authenticationToken != null && flag == false)
        {
            Debug.Log("RUNNING: " + worldServerClient.dataHandler.authenticationToken);
            worldServerClient.SetupConnection(worldServerClient.dataHandler.selectedWorldServer.ip, worldServerClient.dataHandler.selectedWorldServer.port);
            flag = true;
        }

        tickTime += Time.deltaTime;
        if(tickTime > 10f)
        {
            loginMessageHandler.SendAlive();
            worldMessageHandler.SendAlive();
            tickTime = 0;
        }
    }
    public void LoginWorldServer()
    {
        loginMessageHandler.WorldServerAuthenticationTokenRequest();

    }
    public void Login()
    {
        loginMessageHandler.SetupConnection(SERVER_IP, SERVER_PORT);
        loginMessageHandler.Login();
    }
    public void Register()
    {
        loginMessageHandler.SetupConnection(SERVER_IP, SERVER_PORT);
        loginMessageHandler.Register();
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
        worldMessageHandler.PlayCharacter();
    }
    public void ShowCharacterCreation()
    {
        ClearCharacterSelection();
        CharacterCreateForm.SetActive(true);
    }
    public void CreateCharacter()
    {
        worldMessageHandler.CreateCharacter();
    }
    public void DeleteCharacter()
    {
        worldMessageHandler.DeleteCharacter();
    }
}
