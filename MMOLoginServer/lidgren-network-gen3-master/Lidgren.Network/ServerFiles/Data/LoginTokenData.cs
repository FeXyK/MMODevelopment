using MMOLoginServer.ServerData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lidgren.Network.ServerFiles.Data
{
    public class LoginTokenData
    {
        public string expireDate;
        public byte[] token;
        public string characterName;
        public int characterId;
        public float posX;
        public float posZ;
        public float rot;
        public CharacterData characterData;
    }
}
