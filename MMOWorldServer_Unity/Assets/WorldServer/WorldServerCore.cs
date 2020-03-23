using Lidgren.Network;
using Lidgren.Network.ServerFiles;
using Lidgren.Network.Wrapper;
using MMOGameServer.WorldServer;
using UnityEngine;

namespace MMOGameServer
{
    class WorldServerCore : NetPeerOverride
    {
        ///connect to login server via RSA 
        ///implement area servers
        ///implement connection via area servers
        ///all logged on characters in dictionary IN MEMORY DB
        ///character select screen 
        ///accept token from fresh connected account
        ///load in fresh connected account characters
        ///delete token after use
        ///delete token after x time
        ///
        public AreaServerCore areaServer;

        WorldDataHandler dataHandler;
        new WorldMessageHandler messageHandler;
        string HARDCODEDKEY = "LOGINSERVERKEY";
        public override void Initialize(string source)
        {
            base.Initialize(source);
            dataHandler = new WorldDataHandler();
            messageHandler = new WorldMessageHandler(netPeer as NetServer, dataHandler);
        }
        public override void ReceiveMessages()
        {
            MessageType msgType;
            NetIncomingMessage msgIn;
            while ((msgIn = netPeer.ReadMessage()) != null)
            {
                if (msgIn.MessageType == NetIncomingMessageType.ConnectionApproval)
                {
                    msgType = (MessageType)msgIn.ReadByte();
                    Debug.Log(((MessageType)msgType).ToString());
                    switch (msgType)
                    {
                        case MessageType.KeyExchange:
                            messageHandler.KeyExchange(msgIn);
                            break;
                    }
                }
                if (msgIn.MessageType == NetIncomingMessageType.Data)
                {
                    msgType = (MessageType)msgIn.ReadByte();
                    Debug.Log(((MessageType)msgType).ToString());
                    switch (msgType)
                    {
                        case MessageType.WorldServerAuthenticated:
                            messageHandler.SuccessfullAuthentication(msgIn);
                            break;
                        case MessageType.NewAuthenticationToken:
                            messageHandler.NewAuthenticationToken(msgIn);
                            break;
                        case MessageType.ClientAuthentication:
                            messageHandler.ClientAuthentication(msgIn);
                            break;
                        case MessageType.CharacterListRequest:
                            messageHandler.SendCharacterList(msgIn);
                            break;
                        case MessageType.DeleteCharacter:
                            messageHandler.DeleteCharacter(msgIn);
                            break;
                        case MessageType.CreateCharacter:
                            messageHandler.CreateCharacter(msgIn);
                            break;
                        case MessageType.PlayCharacter:
                            messageHandler.PlayCharacter(msgIn, areaServer);
                            break;
                        case MessageType.Alive:
                            messageHandler.Alive(msgIn);
                            break;
                    }
                }
            }
        }
        public bool ConnectToLoginServer(string ip, int port)
        {
            Debug.Log("Connecting");
            NetOutgoingMessage msgOut = netPeer.CreateMessage();
            msgOut.Write((byte)MessageType.KeyExchange);
            msgOut.Write(DataEncryption.publicKey);
            netPeer.Connect(ip, port, msgOut);
            Debug.Log("Connecting sent");
            NetIncomingMessage msgIn = netPeer.WaitMessage(30);
            bool success = false;
            int timeout = 0;
            while (success != true)
            {
                timeout++;
                msgIn = netPeer.WaitMessage(500);
                Debug.Log("Waiting");
                if (msgIn != null)
                {
                    if (msgIn.SenderConnection != null)
                    {
                        if (msgIn.SenderConnection.RemoteHailMessage != null)
                        {
                            MessageType msgType = (MessageType)msgIn.SenderConnection.RemoteHailMessage.ReadByte();
                            Debug.Log((MessageType)msgType);
                            ConnectionData connection = new ConnectionData();
                            connection.publicKey = msgIn.SenderConnection.RemoteHailMessage.ReadString();
                            connection.connection = msgIn.SenderConnection;
                            dataHandler.loginServer = connection;

                            Debug.Log(connection.publicKey);
                            Debug.Log("Login server key OK");

                            messageHandler.SendAuthenticationToken(msgIn, dataHandler.loginServerAuthToken, dataHandler.worldServerName);

                            success = true;
                        }
                    }
                }
                if (timeout > 5)
                    break;
            }
            return success;
        }
        public override void Update()
        {
            ///SendAliveMessageToLoginServer X period
        }
    }
}
