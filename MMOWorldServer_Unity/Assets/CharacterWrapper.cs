
using Assets.AreaServer.Entity;
using UnityEngine;

namespace Assets
{
    public class CharacterWrapper
    {
        public Character character = new Character();
        public Vector3 position;
        public string publicKey;
        public int accountID;
        public bool authenticated;
        public bool admin;
        public byte[] authToken;
    }
}
