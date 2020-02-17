using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Lidgren.Network;
using Lidgren.Network.ServerFiles;
using System.Threading;
using System.Security.Cryptography;
using UnityEngine.UI;
using System;
using System.Text;

public class LoginMaster : MonoBehaviour
{
    string SERVER_IP = "127.0.0.1";
    int SERVER_PORT = 52222;

    private static NetClient netClient;
    NetPeerConfiguration netPeerConfiguration;

    public GameObject RegisterForm;
    public GameObject CharacterCreateForm;
    public GameObject LoginForm;
    public GameObject CharacterSelectForm;
    public GameObject SwitchFormsButton;

    public Button CharacterButton;
    private int characterNumber = 0;

    public GameObject selectedCharacter = null;
    private List<Button> characterButtons = new List<Button>();

    public TMP_InputField username;
    public TMP_InputField password;

    public TMP_InputField characterName;
    public TMP_Dropdown characterType;

    public TMP_InputField usernameReg;
    public TMP_InputField emailReg;
    public TMP_InputField passwordReg;
    public TMP_InputField passwordRegConfirm;
    public TMP_Text SwitchFormsText;
    public TMP_Text DText;


    private void Start()
    {
        InitializeLoginSocket(SERVER_IP, SERVER_PORT);
        if (netClient.Status == NetPeerStatus.NotRunning)
            netClient.Start();
        SetupConnection();
    }
    private void Update()
    {
        ReceiveMessages();
    }
    public void InitializeLoginSocket(string IP, int port)
    {
        netPeerConfiguration = new NetPeerConfiguration("NetLidgrenLogin");
        netPeerConfiguration.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
        netClient = new NetClient(netPeerConfiguration);
    }
    public void SetupConnection()
    {
        NetOutgoingMessage msgLogin = netClient.CreateMessage();

        msgLogin.Write((byte)MessageType.Client);
        msgLogin.Write(DataEncryption.publicKey);
        netClient.Connect(SERVER_IP, SERVER_PORT, msgLogin);
        NetIncomingMessage msgIn = null;
        netClient.MessageReceivedEvent.WaitOne();
        while ((msgIn = netClient.ReadMessage()) != null)
        {
            switch (msgIn.MessageType)
            {
                case NetIncomingMessageType.StatusChanged:
                    switch ((NetConnectionStatus)msgIn.ReadByte())
                    {
                        case NetConnectionStatus.Connected:
                            DataEncryption.publicKey = netClient.ServerConnection.RemoteHailMessage.ReadString();
                            Debug.Log(DataEncryption.publicKey);
                            break;
                        case NetConnectionStatus.Disconnected:
                            {
                                string reason = msgIn.ReadString();
                                if (string.IsNullOrEmpty(reason))
                                    DText.text += ("Disconnected\n");
                                else
                                    DText.text += ("Disconnected, Reason: " + reason + "\n");
                            }
                            break;
                    }
                    break;
            }
        }
    }
    private void ReceiveMessages()
    {
        NetIncomingMessage msgIn;
        MessageType msgType;
        while ((msgIn = netClient.ReadMessage()) != null)
        {
            if (msgIn.MessageType == NetIncomingMessageType.Data)
            {
                msgType = (MessageType)msgIn.ReadByte();
                if (msgType == MessageType.Encrypted)
                {
                    msgType = (MessageType)msgIn.ReadByte();
                }
                switch (msgType)
                {
                    case MessageType.CharacterData:
                        HandleCharacterData(msgIn);
                        break;
                    case MessageType.ServerLoginAnswerOk:
                        LoginForm.SetActive(false);
                        RegisterForm.SetActive(false);
                        SwitchFormsButton.SetActive(false);
                        CharacterSelectForm.SetActive(true);
                        PrintFeedBack(msgIn);
                        break;
                    case MessageType.RegisterAnswerOk:
                        PrintFeedBack(msgIn);
                        break;
                    case MessageType.ServerLoginError:
                        PrintFeedBack(msgIn);
                        break;
                }
            }
        }
    }

    private void HandleCharacterData(string dataEncrypted)
    {
        Debug.Log(dataEncrypted);
    }
    public void CreateCharacter()
    {
        NetOutgoingMessage msgCreate = netClient.CreateMessage();

        msgCreate.Write((byte)MessageType.Encrypted);
        msgCreate.Write((byte)MessageType.CreateCharacter);

        PacketHandler.WriteEncryptedByteArray(msgCreate, characterName.text);
        PacketHandler.WriteEncryptedByteArray(msgCreate, characterType.itemText.text);
        Debug.Log(characterName.text);
        Debug.Log(characterType.itemText.text);
        netClient.SendMessage(msgCreate, NetDeliveryMethod.ReliableOrdered);
        CharacterCreateForm.SetActive(false);
    }
    public void DeleteCharacter()
    {
        NetOutgoingMessage msgDelete = netClient.CreateMessage();

        msgDelete.Write((byte)MessageType.Encrypted);
        msgDelete.Write((byte)MessageType.DeleteCharacter);

        PacketHandler.WriteEncryptedByteArray(msgDelete, selectedCharacter.GetComponent<CharacterButtonContainer>().Name.text);
        ClearCharacterSelection();
        netClient.SendMessage(msgDelete, NetDeliveryMethod.ReliableOrdered);
    }
    public void Login()
    {
        NetOutgoingMessage msgLogin = netClient.CreateMessage();

        byte[] hashPassword = DataEncryption.HashString(password.text);

        msgLogin.Write((byte)MessageType.Encrypted);
        msgLogin.Write((byte)MessageType.ServerLoginRequest);

        PacketHandler.WriteEncryptedByteArray(msgLogin, username.text);
        PacketHandler.WriteEncryptedByteArray(msgLogin, hashPassword);

        netClient.SendMessage(msgLogin, NetDeliveryMethod.ReliableOrdered);
    }
    public void Register()
    {
        if (passwordReg.text == passwordRegConfirm.text)
        {
            NetOutgoingMessage msgRegister = netClient.CreateMessage();
            byte[] hashPassword = DataEncryption.HashString(passwordReg.text);

            msgRegister.Write((byte)MessageType.Encrypted);
            msgRegister.Write((byte)MessageType.RegisterRequest);

            PacketHandler.WriteEncryptedByteArray(msgRegister, usernameReg.text);
            PacketHandler.WriteEncryptedByteArray(msgRegister, hashPassword);
            PacketHandler.WriteEncryptedByteArray(msgRegister, emailReg.text);

            netClient.SendMessage(msgRegister, NetDeliveryMethod.ReliableOrdered);
            Debug.Log("Registration sent");
        }
    }
    private void PrintFeedBack(NetIncomingMessage msgIn)
    {
        DText.text += msgIn.ReadString() + "\n";
    }
    private void HandleCharacterData(NetIncomingMessage msgIn)
    {
        byte[] characterData = PacketHandler.ReadEncryptedByteArray(msgIn);
        DText.text += Encoding.UTF8.GetString(characterData) + "\n";

        Button characterButton = Instantiate(CharacterButton);
        characterButton.transform.SetParent(CharacterSelectForm.transform);
        characterButton.transform.localPosition = new Vector3(0, -60 - (110 * characterNumber), 0);
        RectTransform rt = characterButton.GetComponent<RectTransform>();
        rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 10, 300);
        rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 10 + characterNumber * 110, 100);

        characterButton.GetComponent<CharacterButtonContainer>().Load(Encoding.UTF8.GetString(characterData));
        characterButtons.Add(characterButton);
        Debug.Log(characterNumber++);
    }
    public void ShowCharacterCreation()
    {
        ClearCharacterSelection();
        CharacterCreateForm.SetActive(true);
    }
    private void ClearCharacterSelection()
    {
        foreach (var characterButton in characterButtons)
        {
            Destroy(characterButton.gameObject);
        }
        characterButtons.Clear();
        characterNumber = 0;
        selectedCharacter = null;
    }
    public void SwitchForms()
    {
        LoginForm.SetActive(!LoginForm.activeSelf);
        RegisterForm.SetActive(!RegisterForm.activeSelf);

        if (!LoginForm.activeSelf)
            SwitchFormsText.text = "Login";
        else
            SwitchFormsText.text = "Register";
    }
}