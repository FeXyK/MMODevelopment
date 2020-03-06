using Assets.Scripts.Handlers;
using Assets.Scripts.LoginNetworkScripts;
using Assets.Scripts.LoginScreen;
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

    LoginMessageHandler messageHandler;
    LoginClientManager loginClient;

    string PEER_NAME = "NetLidgrenLogin";
    string SERVER_IP = "86.101.120.217";
    int SERVER_PORT = 52221;
    private void Start()
    {
        loginClient = new LoginClientManager();
        loginClient.Initialize(PEER_NAME);
        messageHandler = LoginMessageHandler.GetInstance();
    }
    private void Update()
    {
        if (loginClient != null)
            loginClient.ReceiveMessages();
    }
    public void Login()
    {
        messageHandler.SetupConnection(SERVER_IP, SERVER_PORT);
        messageHandler.Login();
    }
    public void Register()
    {
        messageHandler.SetupConnection(SERVER_IP, SERVER_PORT);
        messageHandler.Register();
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
        messageHandler.PlayCharacter();
    }
    public void ShowCharacterCreation()
    {
        ClearCharacterSelection();
        CharacterCreateForm.SetActive(true);
    }
    public void CreateCharacter()
    {
        messageHandler.CreateCharacter();
    }
    public void DeleteCharacter()
    {
        messageHandler.DeleteCharacter();
    }
}
