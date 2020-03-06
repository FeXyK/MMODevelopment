using Lidgren.Network;
using Lidgren.Network.ServerFiles;
using MMOLoginServer.ServerData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Handlers
{
    class LoginMessageHandler : Lidgren.Network.Message.MessageHandler
    {
        NetClient netClient;
        LoginDataHandler dataHandler;
        LoginScreenHandler loginScreenHandler;

        TMP_Text Notification;

        private static LoginMessageHandler instance;
        public static LoginMessageHandler GetInstance()
        {
            return instance;

        }
        public void HandleNotification(NetIncomingMessage msgIn)
        {
            Notification.text += msgIn.ReadString() + "\n";
        }
        public LoginMessageHandler(NetClient _netClient)
        {
            netClient = _netClient;
            dataHandler = LoginDataHandler.GetInstance();
            loginScreenHandler = GameObject.FindObjectOfType<LoginScreenHandler>();
            Notification = GameObject.FindGameObjectWithTag("Notification").GetComponent<TMP_Text>();
            instance = this;
        }

        public void Register()
        {
            if (dataHandler.inputData.PasswordReg == dataHandler.inputData.PasswordRegConfirm)
            {
                NetOutgoingMessage msgRegister = netClient.CreateMessage();
                byte[] hashPassword = DataEncryption.HashString(dataHandler.inputData.PasswordReg);

                msgRegister.Write((byte)MessageType.RegisterRequest);

                PacketHandler.WriteEncryptedByteArray(msgRegister, dataHandler.inputData.UsernameReg);
                PacketHandler.WriteEncryptedByteArray(msgRegister, hashPassword);
                PacketHandler.WriteEncryptedByteArray(msgRegister, dataHandler.inputData.EmailReg);

                netClient.SendMessage(msgRegister, NetDeliveryMethod.ReliableOrdered);
                Debug.Log("Registration sent");
            }
        }

        public void CreateCharacter()
        {
            NetOutgoingMessage msgCreate = netClient.CreateMessage();

            msgCreate.Write((byte)MessageType.CreateCharacter);

            PacketHandler.WriteEncryptedByteArray(msgCreate, dataHandler.inputData.CharacterName);
            PacketHandler.WriteEncryptedByteArray(msgCreate, "male");
            Debug.Log(dataHandler.inputData.CharacterName);
            Debug.Log(dataHandler.inputData.CharacterType);
            netClient.SendMessage(msgCreate, NetDeliveryMethod.ReliableOrdered);
            loginScreenHandler.CharacterCreateForm.SetActive(false);
        }

        public void HandleNewLoginToken(NetIncomingMessage msgIn)
        {
            dataHandler.authenticationToken = PacketHandler.ReadEncryptedByteArray(msgIn);
            LoginDataController loginData = new LoginDataController();
            loginData.authToken = dataHandler.authenticationToken;
            loginData.characterName = dataHandler.myCharacters[dataHandler.selectionController.SelectedCharacter].name;
            loginData.characterID = dataHandler.myCharacters[dataHandler.selectionController.SelectedCharacter].id;
            loginData.serverIP = dataHandler.gameServers[dataHandler.selectionController.SelectedServerID].ip;
            loginData.serverPort = dataHandler.gameServers[dataHandler.selectionController.SelectedServerID].port;
            loginData.publicKey = dataHandler.gameServers[dataHandler.selectionController.SelectedServerID].publicKey;
            loginData.characterData = dataHandler.myCharacters[dataHandler.selectionController.SelectedCharacter];

            dataHandler.selectionController.loginDataController = loginData;
            //input.DText.text += "\n" + Encoding.UTF8.GetString(PacketHandler.ReadEncryptedByteArray(msgIn));
            //input.DText.text += "\n" + Encoding.UTF8.GetString(PacketHandler.ReadEncryptedByteArray(msgIn));
            Debug.Log(BitConverter.ToString(dataHandler.authenticationToken));

            netClient.Disconnect("connectingtogameserver");
        }

        public void DeleteCharacter()
        {
            NetOutgoingMessage msgDelete = netClient.CreateMessage();

            msgDelete.Write((byte)MessageType.DeleteCharacter);

            PacketHandler.WriteEncryptedByteArray(msgDelete, dataHandler.myCharacters[dataHandler.selectionController.SelectedCharacter].name);
            netClient.SendMessage(msgDelete, NetDeliveryMethod.ReliableOrdered);
            loginScreenHandler.ClearCharacterSelection();
        }

        public void PlayCharacter()
        {
            NetOutgoingMessage msgOut = netClient.CreateMessage();
            msgOut.Write((byte)MessageType.CharacterLogin);

            msgOut.Write(dataHandler.gameServers[dataHandler.selectionController.SelectedServerID].name);
            msgOut.Write(dataHandler.myCharacters[dataHandler.selectionController.SelectedCharacter].name);
            netClient.ServerConnection.SendMessage(msgOut, NetDeliveryMethod.ReliableOrdered, 1);
        }

        public void Login()
        {
            NetOutgoingMessage msgLogin = netClient.CreateMessage();

            byte[] hashPassword = DataEncryption.HashString(dataHandler.inputData.Password);

            msgLogin.Write((byte)MessageType.ClientAuthentication);
            PacketHandler.WriteEncryptedByteArray(msgLogin, dataHandler.inputData.Username);
            PacketHandler.WriteEncryptedByteArray(msgLogin, hashPassword);

            netClient.SendMessage(msgLogin, NetDeliveryMethod.ReliableOrdered);
        }

        public void HandleGameServerData(NetIncomingMessage msgIn)
        {
            dataHandler.LoadGameServerData(msgIn);
        }

        public void HandleAuthenticationToken(NetIncomingMessage msgIn)
        {
            dataHandler.authenticationToken = PacketHandler.ReadEncryptedByteArray(msgIn);
        }

        public void HandleCharacterData(NetIncomingMessage msgIn)
        {
            dataHandler.LoadCharacterData(msgIn);
        }

        public void HandleSuccessfullLogin()
        {
            loginScreenHandler.LoginForm.SetActive(false);
            loginScreenHandler.RegisterForm.SetActive(false);
            loginScreenHandler.SwitchForm.SetActive(false);
            loginScreenHandler.CharacterSelectForm.SetActive(true);
            loginScreenHandler.ServerSelectForm.SetActive(true);
        }
        public void SetupConnection(string SERVER_IP = "123123123", int SERVER_PORT = 2)
        {
            string[] lines = File.ReadAllLines(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\MMOConfig\ClientConfig.txt");
            string[] data;
            foreach (var line in lines)
            {
                data = line.Split('=');
                switch (data[0].Trim().ToLower())
                {
                    case "server":
                        string[] conn = data[1].Split(':');
                        SERVER_IP = conn[0];
                        SERVER_PORT = int.Parse(conn[1]);
                        break;
                }
            }
            if (netClient.ServerConnection != null)
                return;
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
                                DataEncryption.publicKey = (netClient as NetClient).ServerConnection.RemoteHailMessage.ReadString();
                                Debug.Log(DataEncryption.publicKey);
                                break;
                            case NetConnectionStatus.Disconnected:
                                {
                                    string reason = msgIn.ReadString();
                                    if (string.IsNullOrEmpty(reason))
                                        Debug.Log("Disconnected\n");
                                    else
                                        Debug.Log("Disconnected, Reason: " + reason + "\n");
                                }
                                break;
                        }
                        break;
                }
            }
        }
    }
}
