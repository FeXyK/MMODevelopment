using Lidgren.Network;
using Lidgren.Network.Message;
using Lidgren.Network.Override;
using Lidgren.Network.ServerFiles.Data;

namespace MMOLoginServer
{
    public class LoginServerCore : NetPeerOverride
    {
        ConnectionData currentAccount = null;
        new MessageHandler messageHandler;
        int serverTick = 0;
        public override void Initialize(string SERVER_NAME, int LOGIN_SERVER_PORT, bool IS_SERVER = true, bool simulateLatency = true)
        {
            base.Initialize(SERVER_NAME, LOGIN_SERVER_PORT);
            messageHandler = new MessageHandler((NetServer)netPeer);
        }
        public override void ReceiveMessages()
        {
            NetIncomingMessage msgIn;
            MessageType msgType;
            while ((msgIn = netPeer.ReadMessage()) != null)
            {
                Debug.Log(msgIn.ToString());
                if (msgIn.MessageType == NetIncomingMessageType.ConnectionApproval)
                {
                    //messageHandler.HandleConnectionApproval(msgIn);
                    msgType = (MessageType)msgIn.ReadByte();
                    Debug.Log(((MessageType)msgType).ToString());
                    switch (msgType)
                    {
                        case MessageType.KeyExchange:
                            messageHandler.KeyExchange(msgIn);
                            break;

                    }
                }
                else if (msgIn.MessageType == NetIncomingMessageType.Data)
                {
                    msgType = (MessageType)msgIn.ReadByte();
                    Debug.Log(((MessageType)msgType).ToString());

                    currentAccount = DataHandler.Instance.GetAccount(msgIn);
                    switch (msgType)
                    {
                        case MessageType.ClientAuthentication:
                            messageHandler.AuthenticateClient(msgIn);
                            break;
                        case MessageType.WorldServerAuthenticationTokenRequest:
                            messageHandler.SendWorldServerAuthenticationToken(msgIn);
                            break;
                        case MessageType.WorldServerAuthentication:
                            messageHandler.AuthenticateWorldServer(msgIn);
                            break;
                        case MessageType.RegisterRequest:
                            messageHandler.RegisterAccount(msgIn, currentAccount);
                            break;
                        case MessageType.Alive:
                            messageHandler.Alive(msgIn);
                            break;
                    }
                }
                netPeer.Recycle(msgIn);
            }
        }
        public override void Update()
        {
            if (serverTick > 500)
            {
                DataHandler.Instance.ClearConnections();
                serverTick = 0;
            }
            serverTick++;
        }
    }
}
