using MMOLoginServer.ServerData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lidgren.Network.ServerFiles.Data
{
    public class AuthenticationTokenData
    {
        public byte[] token;
        public string expireDate;
        public string accountName;
        public string validIP;
        public int id;
    }
}
