using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace MMOLoginServer.LoginServerLogic
{
    public class GameServerMessageHandler
    {
        NetServer netServer;
        public GameServerMessageHandler(NetServer server)
        {
            netServer = server;
        }
    }
}
