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


    private int characterNumber = 0;

    public GameObject selectedCharacter = null;
    private List<Button> characterButtons = new List<Button>();


    public LoginSceneInputs input;
    public LoginDataContainer loginDataContainer;

    private byte[] authToken;

    private void Start()
    {
        input = FindObjectOfType<LoginSceneInputs>();
        loginDataContainer = FindObjectOfType<LoginDataContainer>();
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

        msgLogin.Write((byte)ConnectionType.Client);
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
                            //Debug.Log(DataEncryption.publicKey);
                            break;
                        case NetConnectionStatus.Disconnected:
                            {
                                string reason = msgIn.ReadString();
                                if (string.IsNullOrEmpty(reason))
                                    input.DText.text += ("Disconnected\n");
                                else
                                    input.DText.text += ("Disconnected, Reason: " + reason + "\n");
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
                switch (msgType)
                {
                    case MessageType.CharacterData:
                        HandleCharacterData(msgIn);
                        break;
                    case MessageType.ServerLoginAnswerOk:
                        input.LoginForm.SetActive(false);
                        input.RegisterForm.SetActive(false);
                        input.SwitchFormsButton.SetActive(false);
                        input.CharacterSelectForm.SetActive(true);
                        PrintFeedBack(msgIn);
                        break;
                    case MessageType.RegisterAnswerOk:
                        PrintFeedBack(msgIn);
                        break;
                    case MessageType.ServerLoginError:
                        PrintFeedBack(msgIn);
                        break;
                    case MessageType.AuthToken:
                        HandleAuthenticationToken(msgIn);
                        break;
                    case MessageType.GameServersData:
                        HandleGameServerData(msgIn);
                        break;
                    case MessageType.NewLoginToken:
                        input.DText.text = BitConverter.ToString(PacketHandler.ReadEncryptedByteArray(msgIn));
                        input.DText.text += "\n" + Encoding.UTF8.GetString(PacketHandler.ReadEncryptedByteArray(msgIn));
                        input.DText.text += "\n" + Encoding.UTF8.GetString(PacketHandler.ReadEncryptedByteArray(msgIn));
                        break;
                }
            }
        }
    }
    private void HandleGameServerData(NetIncomingMessage msgIn)
    {
        GameServerData gameServerData;

        int count = msgIn.ReadInt16();
        for (int i = 0; i < count; i++)
        {
            gameServerData = new GameServerData();
            gameServerData.name = msgIn.ReadString();
            gameServerData.ip = msgIn.ReadString();
            gameServerData.port = msgIn.ReadInt16();
            loginDataContainer.gameServerDatas.Add(gameServerData);
        }
    }
    private void HandleAuthenticationToken(NetIncomingMessage msgIn)
    {
        loginDataContainer.authToken = PacketHandler.ReadEncryptedByteArray(msgIn);
        input.DText.text += loginDataContainer.authToken.Length + "\n";
        input.DText.text += BitConverter.ToString(loginDataContainer.authToken);
    }
    private void HandleCharacterData(string dataEncrypted)
    {
        Debug.Log(dataEncrypted);
    }
    public void CreateCharacter()
    {
        NetOutgoingMessage msgCreate = netClient.CreateMessage();

        msgCreate.Write((byte)MessageType.CreateCharacter);

        PacketHandler.WriteEncryptedByteArray(msgCreate, input.CharacterName);
        PacketHandler.WriteEncryptedByteArray(msgCreate, "male");
        Debug.Log(input.CharacterName);
        Debug.Log(input.CharacterType);
        netClient.SendMessage(msgCreate, NetDeliveryMethod.ReliableOrdered);
        input.CharacterCreateForm.SetActive(false);
    }
    public void DeleteCharacter()
    {
        NetOutgoingMessage msgDelete = netClient.CreateMessage();

        msgDelete.Write((byte)MessageType.DeleteCharacter);

        PacketHandler.WriteEncryptedByteArray(msgDelete, selectedCharacter.GetComponent<CharacterButtonContainer>().Name.text);
        ClearCharacterSelection();
        netClient.SendMessage(msgDelete, NetDeliveryMethod.ReliableOrdered);
    }
    public void Login()
    {
        NetOutgoingMessage msgLogin = netClient.CreateMessage();

        byte[] hashPassword = DataEncryption.HashString(input.Password);

        msgLogin.Write((byte)MessageType.ClientAuthentication);

        PacketHandler.WriteEncryptedByteArray(msgLogin, input.Username);
        PacketHandler.WriteEncryptedByteArray(msgLogin, hashPassword);

        netClient.SendMessage(msgLogin, NetDeliveryMethod.ReliableOrdered);
    }
    public void Register()
    {
        if (input.PasswordReg == input.PasswordRegConfirm)
        {
            NetOutgoingMessage msgRegister = netClient.CreateMessage();
            byte[] hashPassword = DataEncryption.HashString(input.PasswordReg);

            msgRegister.Write((byte)MessageType.RegisterRequest);

            PacketHandler.WriteEncryptedByteArray(msgRegister, input.UsernameReg);
            PacketHandler.WriteEncryptedByteArray(msgRegister, hashPassword);
            PacketHandler.WriteEncryptedByteArray(msgRegister, input.EmailReg);

            netClient.SendMessage(msgRegister, NetDeliveryMethod.ReliableOrdered);
            Debug.Log("Registration sent");
        }
    }
    private void PrintFeedBack(NetIncomingMessage msgIn)
    {
        input.DText.text += msgIn.ReadString() + "\n";
    }
    private void HandleCharacterData(NetIncomingMessage msgIn)
    {
        byte[] characterData = PacketHandler.ReadEncryptedByteArray(msgIn);
        input.DText.text += Encoding.UTF8.GetString(characterData) + "\n";

        Button characterButton = Instantiate(input.CharacterButton);
        characterButton.transform.SetParent(input.CharacterSelectForm.transform);
        RectTransform rt = characterButton.GetComponent<RectTransform>();
        rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 10, 300);
        rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 10 + characterNumber * 110, 100);
        characterNumber++;
        characterButton.GetComponent<CharacterButtonContainer>().Load(Encoding.UTF8.GetString(characterData));
        characterButtons.Add(characterButton);

    }
    public void ShowCharacterCreation()
    {
        ClearCharacterSelection();
        input.CharacterCreateForm.SetActive(true);
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
        input.LoginForm.SetActive(!input.LoginForm.activeSelf);
        input.RegisterForm.SetActive(!input.RegisterForm.activeSelf);

        if (!input.LoginForm.activeSelf)
            input.SwitchFormsText.text = "Login";
        else
            input.SwitchFormsText.text = "Registration";
    }
    public void PlayCharacter()
    {
        NetOutgoingMessage msgOut = netClient.CreateMessage();
        msgOut.Write((byte)MessageType.CharacterLogin);
        msgOut.Write("Europe");
        msgOut.Write(selectedCharacter.GetComponent<CharacterButtonContainer>().Name.text);
        netClient.Connections[0].SendMessage(msgOut, NetDeliveryMethod.ReliableOrdered, 1);
    }
}