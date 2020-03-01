//using System.Collections.Generic;
//using UnityEngine;
//using Lidgren.Network;
//using Lidgren.Network.ServerFiles;
//using System;
//using System.Text;
//using System.Threading;

//public class ClientLoginManager : MonoBehaviour
//{
//    public string chname;
//    public int chid;

//    string SERVER_IP = "79.121.125.23";
//    int SERVER_PORT = 52221;
//    SceneLoader sceneLoader;
//    private static NetClient netClient;
//    NetPeerConfiguration netPeerConfiguration;
//    public Transform myCharacter;
//    public GameObject type1Character;
//    public GameObject type2Character;
//    public LoginSceneInputs input;
//    public byte[] authToken;
//    public Character character;
//    public Dictionary<int, Character> otherCharacters = new Dictionary<int, Character>();

//    public List<GameServerData> gameServerDatas = new List<GameServerData>();
//    private void Start()
//    {
//        if (input == null)
//            input = FindObjectOfType<LoginSceneInputs>();
//        InitializeLoginSocket(SERVER_IP, SERVER_PORT);
//        if (netClient.Status == NetPeerStatus.NotRunning)
//            netClient.Start();
//        if (sceneLoader == null)
//            sceneLoader = FindObjectOfType<SceneLoader>();
//        SetupConnection();
//        input.netClient = netClient;
//    }
//    float stopFlagDelay = 2f;
//    float stopFlagTimer = 0;
//    bool stopFlag = true;
//    private void Update()
//    {
//        ReceiveMessages();
//        stopFlagTimer -= Time.deltaTime;
//        if (stopFlagTimer < 0)
//        {
//            stopFlag = true;
//        }
//        if (sceneLoader.gameSceneLoaded && (Input.anyKey || !stopFlag))
//        {
//            stopFlag = false;
//            stopFlagTimer = stopFlagDelay;
//            SendPositionUpdate();
//        }
//    }
//    public void SceneLoaded()
//    {

//        myCharacter = GameObject.FindGameObjectWithTag("PlayerCharacter").transform;
//        myCharacter.gameObject.GetComponent<Character>().characterName = chname;
//        myCharacter.gameObject.GetComponent<Character>().id = chid;
//        myCharacter.GetComponentInChildren<Character>().NameText.text = chid + ": " + chname;

//        Thread.Sleep(500);
//        NetOutgoingMessage msgReady = netClient.CreateMessage();
//        msgReady.Write((byte)MessageType.ClientReady);
//        netClient.SendMessage(msgReady, NetDeliveryMethod.ReliableOrdered);
//    }
//    public void SendPositionUpdate()
//    {
//        NetOutgoingMessage msgOut = netClient.CreateMessage();
//        msgOut.Write((byte)MessageType.CharacterMovement);
//        msgOut.Write(myCharacter.GetComponent<Character>().id, 16);
//        msgOut.Write(myCharacter.position.x);
//        msgOut.Write(myCharacter.position.y);
//        msgOut.Write(myCharacter.position.z);
//        msgOut.Write(myCharacter.rotation.eulerAngles.y);
//        netClient.ServerConnection.SendMessage(msgOut, NetDeliveryMethod.ReliableOrdered, 1);
//    }
//    public void InitializeLoginSocket(string IP, int port)
//    {
//        netPeerConfiguration = new NetPeerConfiguration("NetLidgrenLogin");
//        netPeerConfiguration.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
//        netClient = new NetClient(netPeerConfiguration);
//    }
//    public void SetupConnection()
//    {
//        NetOutgoingMessage msgLogin = netClient.CreateMessage();

//        msgLogin.Write((byte)ConnectionType.Client);
//        msgLogin.Write(DataEncryption.publicKey);
//        netClient.Connect(SERVER_IP, SERVER_PORT, msgLogin);
//        NetIncomingMessage msgIn = null;
//        netClient.MessageReceivedEvent.WaitOne();
//        while ((msgIn = netClient.ReadMessage()) != null)
//        {
//            switch (msgIn.MessageType)
//            {
//                case NetIncomingMessageType.StatusChanged:
//                    switch ((NetConnectionStatus)msgIn.ReadByte())
//                    {
//                        case NetConnectionStatus.Connected:
//                            DataEncryption.publicKey = netClient.ServerConnection.RemoteHailMessage.ReadString();
//                            //Debug.Log(DataEncryption.publicKey);
//                            break;
//                        case NetConnectionStatus.Disconnected:
//                            {
//                                string reason = msgIn.ReadString();
//                                if (string.IsNullOrEmpty(reason))
//                                    input.DText.text += ("Disconnected\n");
//                                else
//                                    input.DText.text += ("Disconnected, Reason: " + reason + "\n");
//                            }
//                            break;
//                    }
//                    break;
//            }
//        }
//    }
//    private void ReceiveMessages()
//    {
//        NetIncomingMessage msgIn;
//        MessageType msgType;
//        while ((msgIn = netClient.ReadMessage()) != null)
//        {
//            if (msgIn.MessageType == NetIncomingMessageType.Data)
//            {
//                msgType = (MessageType)msgIn.ReadByte();
//                switch (msgType)
//                {
//                    case MessageType.CharacterData:
//                        input.HandleCharacterData(msgIn);
//                        break;
//                    case MessageType.ServerLoginSuccess:
//                        if (!sceneLoader.gameSceneLoaded)
//                        {
//                            input.LoginForm.SetActive(false);
//                            input.RegisterForm.SetActive(false);
//                            input.SwitchFormsButton.SetActive(false);
//                            input.CharacterSelectForm.SetActive(true);
//                            input.ServerSelectForm.SetActive(true);
//                        }
//                        PrintFeedBack(msgIn);
//                        break;
//                    case MessageType.Notification:
//                        PrintFeedBack(msgIn);
//                        break;
//                    case MessageType.AuthToken:
//                        input.HandleAuthenticationToken(msgIn, ref authToken);
//                        break;
//                    case MessageType.GameServersData:
//                        input.HandleGameServerData(msgIn, gameServerDatas);
//                        break;
//                    case MessageType.NewLoginToken:
//                        authToken = PacketHandler.ReadEncryptedByteArray(msgIn);
//                        input.DText.text += "\n" + Encoding.UTF8.GetString(PacketHandler.ReadEncryptedByteArray(msgIn));
//                        input.DText.text += "\n" + Encoding.UTF8.GetString(PacketHandler.ReadEncryptedByteArray(msgIn));
//                        Debug.Log(BitConverter.ToString(authToken));

//                        netClient.Disconnect("connectingtogameserver");
//                        netClient.ServerConnection.Disconnect("");

//                        break;
//                    ///GameNetCode
//                    ///
//                    case MessageType.NewCharacter:
//                        if (!sceneLoader.gameSceneLoaded) break;

//                        Debug.Log(msgType);
//                        int characterID = msgIn.ReadInt16();
//                        if (characterID == myCharacter.GetComponent<Character>().id)
//                        {
//                            break;
//                        }
//                        int characterLevel = msgIn.ReadInt16();
//                        int characterHealth = msgIn.ReadInt16();
//                        int characterType = msgIn.ReadInt16();
//                        string characterName = msgIn.ReadString();
//                        GameObject newCharacterObj = null;
//                        switch (characterType)
//                        {
//                            case 1:
//                                newCharacterObj = Instantiate(type1Character);
//                                break;
//                            case 2:
//                                newCharacterObj = Instantiate(type2Character);
//                                break;
//                            default:
//                                newCharacterObj = Instantiate(type1Character);
//                                break;
//                        }
//                        Character newCharacter = newCharacterObj.GetComponent<Character>();
//                        newCharacter.Set(characterID, characterLevel, characterHealth, characterType, characterName);

//                        otherCharacters.Add(newCharacter.id, newCharacter);
//                        break;
//                    case MessageType.CharacterMovement:
//                        if (!sceneLoader.gameSceneLoaded) break;
//                        int cId = msgIn.ReadInt16();
//                        if (cId == myCharacter.GetComponent<Character>().id) { break; }
//                        if (otherCharacters.ContainsKey(cId))
//                        {
//                            float posX = msgIn.ReadFloat();
//                            float posY = msgIn.ReadFloat();
//                            float posZ = msgIn.ReadFloat();
//                            float rot = msgIn.ReadFloat();
//                            otherCharacters[cId].posX = posX;
//                            otherCharacters[cId].posY = posY;
//                            otherCharacters[cId].posZ = posZ;
//                            otherCharacters[cId].rot = rot;

//                        }
//                        break;
//                    case MessageType.OtherCharacterRemove:
//                        if (!sceneLoader.gameSceneLoaded) break;
//                        int cRemoveId = msgIn.ReadInt16();
//                        Destroy(otherCharacters[cRemoveId].gameObject);
//                        otherCharacters.Remove(cRemoveId);
//                        break;
//                }
//            }
//            if (msgIn.MessageType == NetIncomingMessageType.StatusChanged)
//            {
//                NetConnectionStatus msgStat = (NetConnectionStatus)msgIn.ReadByte();
//                switch (msgStat)
//                {
//                    case NetConnectionStatus.Connected:
//                        Debug.Log("Connected to GameServer");
//                        sceneLoader.LoadGameScene();
//                        break;
//                    case NetConnectionStatus.Disconnected:
//                        ConnectToGameServer();
//                        Debug.Log("Disconnecting from LoginServer");
//                        break;
//                }
//            }
//        }
//    }
//    private void ConnectToGameServer()
//    {
//        NetOutgoingMessage msgOut = netClient.CreateMessage();
//        msgOut.Write((byte)MessageType.ClientAuthentication);
//        GameServerData serverData = input.selectedServer.GetComponent<ServerSelectData>().serverData;
//        PacketHandler.WriteEncryptedByteArray(msgOut, authToken, serverData.publicKey);
//        Debug.Log(authToken.Length);
//        PacketHandler.WriteEncryptedByteArray(msgOut, input.selectedCharacter.GetComponent<CharacterButtonContainer>().Name.text, serverData.publicKey);
//        netClient.Connect(serverData.ip, serverData.port, msgOut);
//    }
//    private void PrintFeedBack(NetIncomingMessage msgIn)
//    {
//        input.DText.text += msgIn.ReadString() + "\n";
//    }
//}