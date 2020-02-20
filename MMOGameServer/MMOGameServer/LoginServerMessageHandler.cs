using Lidgren.Network;

namespace MMOGameServer
{
    public class LoginServerMessageHandler
    {
        NetServer netServer;
        public LoginServerMessageHandler(NetServer server)
        {
            netServer = server;
        }
    }
}