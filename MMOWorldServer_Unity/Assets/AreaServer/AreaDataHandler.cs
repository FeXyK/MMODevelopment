using Assets.AreaServer.Entity;
using Lidgren.Network;
using Lidgren.Network.ServerFiles;
using Lidgren.Network.ServerFiles.Data;
using MMOLoginServer.ServerData;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MMOGameServer
{
    public class AreaDataHandler
    {
        public string serverName = "Europe";
        public Dictionary<int, CharacterData> characters = new Dictionary<int, CharacterData>();
        public Dictionary<int, Entity> entities = new Dictionary<int, Entity>();
        public List<NetConnection> netConnections = new List<NetConnection>();
        public List<CharacterData> connections = new List<CharacterData>();
        public List<MobAreaSpawner> mobAreas = new List<MobAreaSpawner>();

        public AreaDataHandler()
        {

        }
        public CharacterData GetCharacter(NetIncomingMessage msgIn)
        {
            foreach (var character in characters)
            {
                if (msgIn.SenderConnection == character.Value.connection)
                {
                    return character.Value;
                }
            }
            return null;
        }
        public bool CheckLoginToken(byte[] clientLoginToken, int characterId)
        {
            foreach (var character in connections)
            {
                Console.WriteLine("AUTH TOKENS: ");
                Console.WriteLine(BitConverter.ToString(character.authToken));
                Console.WriteLine(BitConverter.ToString(clientLoginToken));
                Console.WriteLine(character.name + " END");
                Console.WriteLine("---------------------");

                if (ByteArrayCompare(character.authToken, clientLoginToken) && characterId == character.id)
                {
                    Console.WriteLine("ClientAuthenticated");
                    return true;
                }
            }
            return false;
        }
        public ConnectionData FindConnection(NetConnection senderConnection)
        {
            foreach (var connection in connections)
            {
                if (connection.connection == senderConnection)
                {
                    return connection;
                }
            }
            return null;
        }
        //public void RemoveExpiredLoginTokens()
        //{
        //    AuthenticationTokenData remove = null;
        //    foreach (var token in loginTokens)
        //    {
        //        if (DateTime.Parse(token.expireDate) < DateTime.Now)
        //        {
        //            remove = token;
        //            break;
        //        }
        //    }
        //    if (remove != null)
        //        loginTokens.Remove(remove);
        //}

        [System.Runtime.InteropServices.DllImport("msvcrt.dll", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        static extern int memcmp(byte[] b1, byte[] b2, long count);
        public static bool ByteArrayCompare(byte[] b1, byte[] b2)
        {
            // Validate buffers are the same length.
            // This also ensures that the count does not exceed the length of either buffer.  
            return b1.Length == b2.Length && memcmp(b1, b2, b1.Length) == 0;
        }

        internal MobBase GetMob(int targetID)
        {
            int mobID = (int)((float)targetID / 100);
            foreach (var mobArea in mobAreas)
            {
                if(mobArea.MobID == mobID)
                {
                    return mobArea.SpawnedMobs[targetID];
                    Debug.Log("MOBID: " + mobID);
                }
            }
            return null;
        }
    }
}
