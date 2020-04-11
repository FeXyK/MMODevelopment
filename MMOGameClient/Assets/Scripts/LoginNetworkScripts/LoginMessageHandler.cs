using Lidgren.Network;
using Lidgren.Network.Message;
using Lidgren.Network.ServerFiles;
using System;
using System.IO;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Handlers
{
    class LoginMessageHandler : Lidgren.Network.Message.MessageHandler
    {
        NetClient netClient;
        LoginDataHandler dataHandler;
        LoginScreenHandler loginScreenHandler;
        string publicKey;
        TMP_Text Notification;




        private static LoginMessageHandler instance;
        public static LoginMessageHandler GetInstance()
        {
            if (instance == null)
                Debug.Log("Message Handler is NULL");
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

                PacketHandler.WriteEncryptedByteArray(msgRegister, dataHandler.inputData.UsernameReg, publicKey);
                PacketHandler.WriteEncryptedByteArray(msgRegister, hashPassword, publicKey);
                PacketHandler.WriteEncryptedByteArray(msgRegister, dataHandler.inputData.EmailReg, publicKey);

                netClient.SendMessage(msgRegister, NetDeliveryMethod.ReliableOrdered);
                Debug.Log("Registration sent");
            }
        }

        internal void SendAlive()
        {
            if (netClient.ConnectionStatus == NetConnectionStatus.Connected)
            {
                NetOutgoingMessage msgOut = netClient.CreateMessage();
                msgOut.Write((byte)MessageType.Alive);
                netClient.SendMessage(msgOut, NetDeliveryMethod.ReliableOrdered);
            }
        }
        internal void WorldServerAuthenticationTokenRequest()
        {
            if ( dataHandler.worldServers[dataHandler.selectionController.SelectedServerID] != null)
            {
                dataHandler.selectedWorldServer = dataHandler.worldServers[dataHandler.selectionController.SelectedServerID];
                NetOutgoingMessage msgOut = netClient.CreateMessage();

                msgOut.Write((byte)MessageType.WorldServerAuthenticationTokenRequest);
                PacketHandler.WriteEncryptedByteArray(msgOut, dataHandler.selectedWorldServer.name, publicKey);

                netClient.SendMessage(msgOut, NetDeliveryMethod.ReliableOrdered);
            }
        }
        public void HandleNewLoginToken(NetIncomingMessage msgIn)
        {
            dataHandler.authenticationToken = PacketHandler.ReadEncryptedByteArray(msgIn);
            LoginDataController loginData = new LoginDataController();
            loginData.authToken = dataHandler.authenticationToken;

            dataHandler.selectionController.loginDataController = loginData;
            Debug.Log(BitConverter.ToString(dataHandler.authenticationToken));
        }
        public void Login()
        {
            {
                NetOutgoingMessage msgLogin = netClient.CreateMessage();

                byte[] hashPassword = DataEncryption.HashString(dataHandler.inputData.Password);

                msgLogin.Write((byte)MessageType.ClientAuthentication);
                PacketHandler.WriteEncryptedByteArray(msgLogin, dataHandler.inputData.Username, publicKey);
                PacketHandler.WriteEncryptedByteArray(msgLogin, hashPassword, publicKey);

                netClient.SendMessage(msgLogin, NetDeliveryMethod.ReliableOrdered);
            }
        }
        public void HandleGameServerData(NetIncomingMessage msgIn)
        {
            dataHandler.LoadGameServerData(msgIn);
        }
        public void HandleSuccessfullLogin()
        {
            loginScreenHandler.LoginForm.SetActive(false);
            loginScreenHandler.RegisterForm.SetActive(false);
            loginScreenHandler.SwitchForm.SetActive(false);
            loginScreenHandler.CharacterSelectForm.SetActive(false);
            loginScreenHandler.ServerSelectForm.SetActive(true);
        }
        public void SetupConnection()
        {
            string SERVER_IP = "";
            int SERVER_PORT = 0;
            if (netClient.ServerConnection != null)
                return;

            string[] lines = File.ReadAllLines(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\MMOConfig\ClientConfig.cfg");
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
            NetOutgoingMessage msgLogin = netClient.CreateMessage();

            msgLogin.Write((byte)MessageType.KeyExchange);
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
                                (netClient as NetClient).ServerConnection.RemoteHailMessage.ReadByte();
                                publicKey = (netClient as NetClient).ServerConnection.RemoteHailMessage.ReadString();
                                Debug.Log(publicKey);
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
