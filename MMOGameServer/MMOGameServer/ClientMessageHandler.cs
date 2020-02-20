using Lidgren.Network;

namespace MMOGameServer
{
    public class ClientMessageHandler
    {
        private NetServer netServer;
        public ClientMessageHandler(NetServer server)
        {
            netServer = server;
        }
    }
}