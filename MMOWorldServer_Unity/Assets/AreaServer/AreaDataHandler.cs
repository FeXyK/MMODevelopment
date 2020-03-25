using Assets.AreaServer.Entity;
using System;
using System.Collections.Generic;
using Lidgren.Network;
using Lidgren.Network.ServerFiles.Data;
using UnityEngine;
using Assets;

namespace MMOGameServer
{
    public class AreaDataHandler
    {
        public string serverName = "Europe";

        public List<CharacterWrapper> waitingForAuth = new List<CharacterWrapper>();

        public Dictionary<int, Entity> entitiesByID = new Dictionary<int, Entity>();
        public Dictionary<NetConnection, Entity> entitiesByConnection = new Dictionary<NetConnection, Entity>();

        public List<MobAreaSpawner> mobAreas = new List<MobAreaSpawner>();

        public AreaDataHandler()
        {

        }
        public void AddEntity(Entity newEntity, NetConnection connection)
        {
            if (entitiesByID.ContainsKey(newEntity.EntityID))
            {
                entitiesByID.Remove(newEntity.EntityID);
            }
            entitiesByID.Add(newEntity.EntityID, newEntity);

            if (entitiesByConnection.ContainsKey(connection))
            {
                entitiesByConnection.Remove(connection);
            }
            entitiesByConnection.Add(connection, newEntity);
        }
        public void RemoveEntity(Entity newEntity, NetConnection connection)
        {
            if (entitiesByID.ContainsKey(newEntity.EntityID))
            {
                entitiesByID.Remove(newEntity.EntityID);
            }
            if (entitiesByConnection.ContainsKey(connection))
            {
                entitiesByConnection.Remove(connection);
            }
        }
        public Entity GetEntity(NetConnection connection)
        {
            return entitiesByConnection[connection];
        }
        public Entity GetEntity(int entityID)
        {
            return entitiesByID[entityID];
        }
        //public CharacterData GetCharacter(NetIncomingMessage msgIn)
        //{
        //    foreach (var character in characters)
        //    {
        //        if (msgIn.SenderConnection == character.Value.connection)
        //        {
        //            return character.Value;
        //        }
        //    }
        //    return null;
        //}
        public bool CheckLoginToken(byte[] clientLoginToken, int characterId)
        {
            foreach (var character in waitingForAuth)
            {
                Console.WriteLine("AUTH TOKENS: ");
                Console.WriteLine(BitConverter.ToString(character.authToken));
                Console.WriteLine(BitConverter.ToString(clientLoginToken));
                Console.WriteLine(character.character.EntityName + " END");
                Console.WriteLine("---------------------");

                if (ByteArrayCompare(character.authToken, clientLoginToken) && characterId == character.character.EntityID)
                {
                    Console.WriteLine("ClientAuthenticated");
                    return true;
                }
            }
            return false;
        }

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
                if (mobArea.MobID == mobID)
                {
                    Debug.Log("MOBID: " + mobID);
                    return mobArea.SpawnedMobs[targetID];
                }
            }
            return null;
        }
    }
}
