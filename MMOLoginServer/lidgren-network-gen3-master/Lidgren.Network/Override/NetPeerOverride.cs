using Lidgren.Network.Message;
using Lidgren.Network.ServerFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lidgren.Network.Wrapper
{
    public class NetPeerOverride
    {
        public NetPeer netPeer;
        public NetPeerConfiguration netPeerConfiguration;
        public List<ConnectionData> connections = new List<ConnectionData>();
        public MessageHandler messageHandler;
        public virtual void Initialize(string PEER_NAME, int PEER_PORT = 0)
        {
            messageHandler = new MessageHandler();

            netPeerConfiguration = new NetPeerConfiguration(PEER_NAME);
            netPeerConfiguration.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
            if (PEER_PORT != 0)
                netPeerConfiguration.Port = PEER_PORT;

            netPeer = new NetServer(netPeerConfiguration);
            netPeer.Start();
            Console.WriteLine("Server started!");
        }

        public void StartServer(int LOOP_FRAMERATE)
        {

            MainLoop(LOOP_FRAMERATE);

            netPeer.Shutdown("bye");
        }
        private void MainLoop(int desiredFPS)
        {
            while (true)
            {
                ReceiveMessages();
                Thread.Sleep(1000 / desiredFPS);
            }
        }
        
        public virtual void ReceiveMessages() { Console.WriteLine("Override me!"); }

    }
}
