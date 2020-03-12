using Lidgren.Network.Message;
using Lidgren.Network.ServerFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace Lidgren.Network.Wrapper
{
    public class NetPeerOverride
    {
        public NetPeer netPeer;
        public NetPeerConfiguration netPeerConfiguration;
        public List<ConnectionData> connections = new List<ConnectionData>();
        public MessageHandler messageHandler;
        public virtual void Initialize(string PEER_NAME = "", int PEER_PORT = 0, bool IS_SERVER = true, bool simulateLatency = true)
        {
            Console.WriteLine("Initialize from parameters.");
            messageHandler = new MessageHandler();

            netPeerConfiguration = new NetPeerConfiguration(PEER_NAME);
            netPeerConfiguration.EnableMessageType(NetIncomingMessageType.ConnectionApproval);


            if (simulateLatency)
            {
                netPeerConfiguration.SimulatedMinimumLatency = 0.250f;
                netPeerConfiguration.SimulatedRandomLatency = 0.010f;
                netPeerConfiguration.SimulatedDuplicatesChance = 0.01f;
                netPeerConfiguration.SimulatedLoss = 0.01f;
            }
            if (IS_SERVER)
            {
                netPeerConfiguration.Port = PEER_PORT;
                netPeer = new NetServer(netPeerConfiguration);
            }
            else
                netPeer = new NetClient(netPeerConfiguration);
            netPeer.Start();
            //Console.WriteLine("Server started!");
        }
        public virtual void Initialize(string source)
        {
            Console.WriteLine("Initialize from source.");
            string peerName = "NetLidgrenLogin";
            int peerPort = 52221;
            bool peerIsServer = true;
            bool peerSimulateLatency = false;

            messageHandler = new MessageHandler();
            string[] lines = File.ReadAllLines(source);
            string[] data;

            foreach (var line in lines)
            {
                data = line.Split('=');
                switch (data[0].ToLower().Trim())
                {
                    case "peername":
                        peerName = data[1];
                        break;

                    case "peerport":
                        peerPort = int.Parse(data[1]);
                        break;

                    case "peerisserver":
                        peerIsServer = bool.Parse(data[1]);
                        break;

                    case "peersimulatelatency":
                        peerSimulateLatency = bool.Parse(data[1]);
                        break;
                    default:
                        break;
                }
            }
            Console.WriteLine(peerName);
            Console.WriteLine("PortNumber: " + peerPort);
            Console.WriteLine("Server: " + peerIsServer);
            Console.WriteLine("Latency Simulateion: " + peerSimulateLatency);

            netPeerConfiguration = new NetPeerConfiguration(peerName);
            netPeerConfiguration.EnableMessageType(NetIncomingMessageType.ConnectionApproval);


            if (peerSimulateLatency)
            {
                netPeerConfiguration.SimulatedMinimumLatency = 0.040f;
                netPeerConfiguration.SimulatedRandomLatency = 0.010f;
                netPeerConfiguration.SimulatedDuplicatesChance = 0.01f;
                netPeerConfiguration.SimulatedLoss = 0.01f;
            }
            if (peerIsServer)
            {
                netPeerConfiguration.Port = peerPort;
                netPeer = new NetServer(netPeerConfiguration);
            }
            else
                netPeer = new NetClient(netPeerConfiguration);
            netPeer.Start();
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
                Update();
                Thread.Sleep(1000 / desiredFPS);
            }
        }

        public virtual void ReceiveMessages() { Console.WriteLine("Override me!"); }
        public virtual void Update() { Console.WriteLine("Override me!"); }

    }
}
