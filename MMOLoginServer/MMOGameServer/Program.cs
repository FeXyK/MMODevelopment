using System;
using Lidgren.Network.ServerFiles;
using MMOLoginServer.LoginServerLogic;
using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace MMOLoginServer
{
    class Program
    {
        static LoginMaster loginMaster;

        const string LOGIN_SERVER_NAME = "NetLidgrenLogin";
        const int LOGIN_SERVER_PORT = 52222;
        const int LOGIN_SERVER_FRAMERATE = 5;
        const bool DEBUG_ENABLED = true;
        static RSACryptoServiceProvider endServer = new RSACryptoServiceProvider(512);
        static RSACryptoServiceProvider endClient = new RSACryptoServiceProvider(1024);

        static void Main(string[] args)
        {
            Debug.enable = DEBUG_ENABLED;


            loginMaster = new LoginMaster();

            loginMaster.InitializeLoginServer(LOGIN_SERVER_NAME, LOGIN_SERVER_PORT);
            loginMaster.StartLoginServer(LOGIN_SERVER_FRAMERATE);

        }
    }
}
