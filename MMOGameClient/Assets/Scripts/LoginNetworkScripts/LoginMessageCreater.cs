using Lidgren.Network;

namespace Assets.Scripts.Handlers
{
    class LoginMessageCreater
    {
        private NetClient netClient;

        public LoginMessageCreater(NetClient netClient)
        {
            this.netClient = netClient;
        }
    }
}
