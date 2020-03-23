using Assets.Scripts.Handlers;
using Lidgren.Network;
using Lidgren.Network.ServerFiles;
using System;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.WorldServerNetworkScripts
{
    class WorldServerMessageHandler : Lidgren.Network.Message.MessageHandler
    {
        NetClient netClient;
        private LoginDataHandler dataHandler;
        private static WorldServerMessageHandler instance;
        LoginScreenHandler loginScreenHandler;
        TMP_Text Notification;
        public static WorldServerMessageHandler GetInstance()
        {
            if (instance == null)
                Debug.Log("Message Handler is NULL");
            return instance;

        }
        public WorldServerMessageHandler(NetClient netClient)
        {
            Notification = GameObject.FindGameObjectWithTag("Notification").GetComponent<TMP_Text>();
            loginScreenHandler = GameObject.FindObjectOfType<LoginScreenHandler>();
            this.netClient = netClient;
            this.dataHandler = LoginDataHandler.GetInstance();
            instance = this;
        }

        internal void AreaServerConnectionData(NetIncomingMessage msgIn)
        {
            dataHandler.selectedWorldServer.areaServerAuthToken = PacketHandler.ReadEncryptedByteArray(msgIn);
            dataHandler.selectedWorldServer.areaServerPort = PacketHandler.ReadEncryptedInt(msgIn);
            dataHandler.selectionController.loginDataController.authToken = dataHandler.selectedWorldServer.areaServerAuthToken;
            dataHandler.selectionController.loginDataController.publicKey = dataHandler.selectedWorldServer.publicKey;
            dataHandler.selectionController.loginDataController.serverIP = dataHandler.selectedWorldServer.ip;
            dataHandler.selectionController.loginDataController.serverPort = dataHandler.selectedWorldServer.areaServerPort;

        }

        internal void HandleNotification(NetIncomingMessage msgIn)
        {
            Notification.text += "\n" + msgIn.ReadString();
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
        public void HandleCharacterData(NetIncomingMessage msgIn)
        {
            dataHandler.LoadCharacterData(msgIn);
            dataHandler.selectionController.CharacterForm.gameObject.SetActive(true);
            dataHandler.selectionController.ServerForm.gameObject.SetActive(false);
        }

        internal void SendAuthenticationToken(NetIncomingMessage msgIn)
        {
            NetOutgoingMessage msgOut = netClient.CreateMessage();
            msgOut.Write((byte)MessageType.ClientAuthentication);
            PacketHandler.WriteEncryptedByteArray(msgOut, dataHandler.authenticationToken, dataHandler.selectedWorldServer.publicKey);
            PacketHandler.WriteEncryptedByteArray(msgOut, dataHandler.inputData.Username, dataHandler.selectedWorldServer.publicKey);
            msgIn.SenderConnection.SendMessage(msgOut, NetDeliveryMethod.ReliableOrdered, 1);

            Debug.Log(dataHandler.selectedWorldServer.publicKey);
        }

        internal void ClientAuthenticated(NetIncomingMessage msgIn)
        {
            NetOutgoingMessage msgOut = netClient.CreateMessage();
            msgOut.Write((byte)MessageType.CharacterListRequest);
            netClient.SendMessage(msgOut, NetDeliveryMethod.ReliableOrdered, 1);
        }

        public void PlayCharacter()
        {
            dataHandler.selectedCharacter = dataHandler.myCharacters[dataHandler.selectionController.SelectedCharacter];
            dataHandler.selectedWorldServer = dataHandler.worldServers[dataHandler.selectionController.SelectedServerID];

            NetOutgoingMessage msgOut = netClient.CreateMessage();

            msgOut.Write((byte)MessageType.PlayCharacter);
            PacketHandler.WriteEncryptedByteArray(msgOut, dataHandler.inputData.Username, dataHandler.selectedWorldServer.publicKey);
            PacketHandler.WriteEncryptedByteArray(msgOut, dataHandler.selectedCharacter.name, dataHandler.selectedWorldServer.publicKey);
            msgOut.Write(dataHandler.selectedCharacter.id, 16);
            Debug.Log(dataHandler.selectedCharacter.id);
            netClient.ServerConnection.SendMessage(msgOut, NetDeliveryMethod.ReliableOrdered, 1);
        }
        public void CreateCharacter()
        {
            NetOutgoingMessage msgCreate = netClient.CreateMessage();

            msgCreate.Write((byte)MessageType.CreateCharacter);

            PacketHandler.WriteEncryptedByteArray(msgCreate, dataHandler.inputData.CharacterName, dataHandler.selectedWorldServer.publicKey);
            msgCreate.Write(dataHandler.inputData.CharacterType, 16);
            Debug.Log(dataHandler.inputData.CharacterName);
            Debug.Log(dataHandler.inputData.CharacterType);
            netClient.SendMessage(msgCreate, NetDeliveryMethod.ReliableOrdered);
            loginScreenHandler.CharacterCreateForm.SetActive(false);
        }
        public void DeleteCharacter()
        {
            NetOutgoingMessage msgDelete = netClient.CreateMessage();

            msgDelete.Write((byte)MessageType.DeleteCharacter);

            PacketHandler.WriteEncryptedByteArray(msgDelete, dataHandler.myCharacters[dataHandler.selectionController.SelectedCharacter].name, dataHandler.selectedWorldServer.publicKey);
            netClient.SendMessage(msgDelete, NetDeliveryMethod.ReliableOrdered);
            loginScreenHandler.ClearCharacterSelection();
        }
    }
}
