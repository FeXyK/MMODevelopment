using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Handlers
{
    class MessageCreater
    {
        private NetClient netClient;

        public MessageCreater(NetClient netClient)
        {
            this.netClient = netClient;
        }
    }
}
