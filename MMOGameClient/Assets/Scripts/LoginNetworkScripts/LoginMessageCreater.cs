using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
