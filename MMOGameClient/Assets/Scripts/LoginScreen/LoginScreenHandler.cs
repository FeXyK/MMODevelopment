using Assets.Scripts.Handlers;
using Assets.Scripts.LoginNetworkScripts;
using Assets.Scripts.WorldServerNetworkScripts;
using System;
using System.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginScreenHandler : MonoBehaviour
{
    public GameObject SwitchForm;
    public GameObject LoginForm;
    public GameObject RegisterForm;
    public GameObject ServerSelectForm;
    public GameObject CharacterSelectForm;
    public GameObject CharacterCreateForm;

    public Button LoginButton;
    public Button RegisterButton;
    public Button SwitchFormsButton;
    public Button LoginWorldServerButton;
    public Button PlayCharacterButton;


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
    public void LoginWorldServer()
    {
        LoginWorldServerButton.interactable = false;
        loginMessageHandler.WorldServerAuthenticationTokenRequest();
        LoginWorldServerButton.interactable = true;

    }
    public void Login()
    {
        LoginButton.interactable = false;
        loginMessageHandler.SetupConnection(SERVER_IP, SERVER_PORT);
        loginMessageHandler.Login();
        LoginButton.interactable = true;
    }
    public void Register()
    {
        RegisterButton.interactable = false;
        loginMessageHandler.SetupConnection(SERVER_IP, SERVER_PORT);
        loginMessageHandler.Register();
        SwitchForms();
        RegisterButton.interactable = true;
    }
    public void ClearCharacterSelection()
    {
    }
    public void SwitchForms()
    {
        SwitchFormsButton.interactable = false;
        LoginForm.SetActive(!LoginForm.activeSelf);
        RegisterForm.SetActive(!RegisterForm.activeSelf);

        if (!LoginForm.activeSelf)
            SwitchForm.GetComponentInChildren<TMP_Text>().text = "Login";
        else
            SwitchForm.GetComponentInChildren<TMP_Text>().text = "Registration";
        SwitchFormsButton.interactable = true;
    }
    public void PlayCharacter()
    {
        PlayCharacterButton.interactable = false;
        worldMessageHandler.PlayCharacter();
        PlayCharacterButton.interactable = true;
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
