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
using Lidgren.Network.Wrapper;
using UnityEditor.VersionControl;

public class ClientManager : MonoBehaviour
{
    string SERVER_IP = "127.0.0.1";
    int SERVER_PORT = 52222;

    private static NetClient netClient;
    NetPeerConfiguration netPeerConfiguration;


    public LoginSceneInputs input;
    public LoginDataContainer loginDataContainer;

    public byte[] authToken;
    public Character character;
    public List<GameServerData> gameServerDatas = new List<GameServerData>();
    private void Start()
    {
        if (input == null)
            input = FindObjectOfType<LoginSceneInputs>();
        if (loginDataContainer == null)
            loginDataContainer = FindObjectOfType<LoginDataContainer>();
        InitializeLoginSocket(SERVER_IP, SERVER_PORT);
        if (netClient.Status == NetPeerStatus.NotRunning)
            netClient.Start();
        SetupConnection();
        input.netClient = netClient;
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
                        input.HandleCharacterData(msgIn);
                        break;
                    case MessageType.ServerLoginAnswerOk:
                        input.LoginForm.SetActive(false);
                        input.RegisterForm.SetActive(false);
                        input.SwitchFormsButton.SetActive(false);
                        input.CharacterSelectForm.SetActive(true);
                        input.ServerSelectForm.SetActive(true);
                        PrintFeedBack(msgIn);
                        break;
                    case MessageType.RegisterAnswerOk:
                        PrintFeedBack(msgIn);
                        break;
                    case MessageType.ServerLoginError:
                        PrintFeedBack(msgIn);
                        break;
                    case MessageType.AuthToken:
                        input.HandleAuthenticationToken(msgIn, authToken);
                        break;
                    case MessageType.GameServersData:
                        input.HandleGameServerData(msgIn, gameServerDatas);
                        break;
                    case MessageType.NewLoginToken:
                        input.DText.text = BitConverter.ToString(PacketHandler.ReadEncryptedByteArray(msgIn));
                        input.DText.text += "\n" + Encoding.UTF8.GetString(PacketHandler.ReadEncryptedByteArray(msgIn));
                        input.DText.text += "\n" + Encoding.UTF8.GetString(PacketHandler.ReadEncryptedByteArray(msgIn));
                        NetOutgoingMessage msgOut = netClient.CreateMessage();
                        msgOut.Write((byte)MessageType.ClientAuthentication);
                        GameServerData serverData = input.selectedServer.GetComponent<ServerSelectData>().serverData;
                        PacketHandler.WriteEncryptedByteArray(msgOut, authToken, serverData.publicKey);
                        PacketHandler.WriteEncryptedByteArray(msgOut, input.selectedCharacter.GetComponent<CharacterButtonContainer>().Name.text, serverData.publicKey);
                        Debug.LogError(netClient.Connect(serverData.ip, serverData.port, msgOut));
                        Debug.LogError(serverData.ip);
                        Debug.LogError(serverData.port);
                        break;
                }
            }
        }
    }
    private void PrintFeedBack(NetIncomingMessage msgIn)
    {
        input.DText.text += msgIn.ReadString() + "\n";
    }
}