using Lidgren.Network;
using Lidgren.Network.ServerFiles;
using Lidgren.Network.ServerFiles.Data;
using MMOLoginServer.ServerData;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace MMOGameServer
{
    public class AreaMessageHandler
    {
        NetServer netServer;

        AreaDataHandler dataHandler;

        AreaMessageReader messageReader;
        AreaMessageCreater messageCreate;
        ConcurrentQueue<CharacterData> newConnectionsQue;

        public bool hideNames = false;
        public AreaMessageHandler(NetServer server, ConcurrentQueue<CharacterData> newConnections)
        {
            netServer = server;
            newConnectionsQue = newConnections;
            dataHandler = new AreaDataHandler();
            messageCreate = new AreaMessageCreater(netServer);
            messageReader = new AreaMessageReader();
        }
        public void ClientReady(NetIncomingMessage msgIn)
        {
            CharacterData characterData = dataHandler.GetCharacter(msgIn);

            //To already ingame players
            NetOutgoingMessage msgOut = messageCreate.CreateNewCharacterMessage(characterData);
            netServer.SendMessage(msgOut, dataHandler.netConnections, NetDeliveryMethod.ReliableOrdered, 1);

            //To new connected player
            foreach (var character in dataHandler.characters)
            {
                if (character.Value.connection != msgIn.SenderConnection)
                {
                    msgOut = messageCreate.CreateNewCharacterMessage(character.Value);
                    msgIn.SenderConnection.SendMessage(msgOut, NetDeliveryMethod.ReliableOrdered, 1);
                }
            }
            msgOut = CreateHideNamesMessage();
            msgIn.SenderConnection.SendMessage(msgOut, NetDeliveryMethod.ReliableOrdered, 1);
        }
        public NetOutgoingMessage CreateHideNamesMessage()
        {
            NetOutgoingMessage msgOut = netServer.CreateMessage();
            if (hideNames)
                msgOut.Write((byte)MessageType.HideNames);
            else
                msgOut.Write((byte)MessageType.ShowNames);
            return msgOut;
        }
        internal void SendPrivateChatMessage(NetIncomingMessage msgIn)
        {
            NetOutgoingMessage msgOut = netServer.CreateMessage();
            string from = msgIn.ReadString();
            string to = msgIn.ReadString();
            string msg = msgIn.ReadString();

            msgOut.Write((byte)MessageType.PrivateChatMessage);
            msgOut.Write(from);
            msgOut.Write(msg);
            foreach (var character in dataHandler.characters.Values)
            {
                if (character.name.ToLower() == to.ToLower())
                {
                    character.connection.SendMessage(msgOut, NetDeliveryMethod.ReliableUnordered, 0);
                }
            }
        }

        internal void GetNewConnections()
        {
            if (newConnectionsQue.Count > 0)
            {
                CharacterData character = new CharacterData();
                newConnectionsQue.TryDequeue(out character);
                if (character != null)
                {
                    dataHandler.connections.Add(character);
                    Debug.Log(character.name + " added to characters");
                }
            }
        }

        internal void SendPublicChatMessage(NetIncomingMessage msgIn)
        {
            NetOutgoingMessage msgOut = netServer.CreateMessage();
            string from = msgIn.ReadString();
            string msg = msgIn.ReadString();
            msgOut.Write((byte)MessageType.PublicChatMessage);
            msgOut.Write(from);
            msgOut.Write(msg);
            netServer.SendToAll(msgOut, NetDeliveryMethod.Unreliable);
        }

        internal void HandleAdminMessage(NetIncomingMessage msgIn)
        {
            NetOutgoingMessage msgOut = netServer.CreateMessage();
            string msg = msgIn.ReadString();
            switch (msg.ToLower())
            {
                case "hidenames":
                    hideNames = true;
                    msgOut.Write((byte)MessageType.AdminChatMessage);
                    msgOut.Write((byte)MessageType.HideNames);
                    break;
                case "shownames":
                    hideNames = false;
                    msgOut.Write((byte)MessageType.AdminChatMessage);
                    msgOut.Write((byte)MessageType.ShowNames);
                    break;
                default:
                    msgOut.Write((byte)MessageType.AdminChatMessage);
                    msgOut.Write((byte)MessageType.AdminChatMessage);
                    msgOut.Write("Admin");
                    msgOut.Write(msg);
                    break;
            }
            netServer.SendToAll(msgOut, NetDeliveryMethod.ReliableOrdered);
        }

        public void ClientAuthentication(NetIncomingMessage msgIn)
        {
            byte[] clientLoginToken = PacketHandler.ReadEncryptedByteArray(msgIn);
            int characterId = msgIn.ReadInt16();
            CharacterData characterData = dataHandler.connections.Find(x => x.id == characterId);

            Console.WriteLine("Token length: " + clientLoginToken.Length);
            Console.WriteLine(BitConverter.ToString(clientLoginToken));
            Console.WriteLine(BitConverter.ToString(characterData.authToken));
            if (dataHandler.CheckLoginToken(clientLoginToken, characterId))
            {
                msgIn.SenderConnection.Approve();

                characterData.connection = msgIn.SenderConnection;
                //characterData.id = validToken.characterData.id;
                //characterData.name = validToken.characterData.name;
                characterData.currentHealth = 100;
                characterData.positionX = 0;//new System.Numerics.Vector3(0, 0, 0);
                characterData.positionY = 0;//new System.Numerics.Vector3(0, 0, 0);
                characterData.positionZ = 0;//new System.Numerics.Vector3(0, 0, 0);
                characterData.rotation = 0;
                if (dataHandler.characters.ContainsKey(characterData.id))
                    dataHandler.characters.Remove(characterData.id);
                dataHandler.characters.Add(characterData.id, characterData);
                dataHandler.netConnections.Add(msgIn.SenderConnection);
                foreach (var item in dataHandler.characters.Values)
                {
                    Console.WriteLine("CHARACTER: " + item.ToString());
                }
                Console.WriteLine("Authenticated!");
            }
            else
            {
                Console.WriteLine("Bad AuthenticationKey");
            }
        }
        public void CharacterMovementData(NetIncomingMessage msgIn)
        {
            int id = msgIn.ReadInt16();
            float x = msgIn.ReadFloat();
            float y = msgIn.ReadFloat();
            float z = msgIn.ReadFloat();
            float r = msgIn.ReadFloat();
            dataHandler.characters[id].positionX= x;
            dataHandler.characters[id].positionY= y;
            dataHandler.characters[id].positionZ= z;
            dataHandler.characters[id].rotation = r;
        }
        public void SendMovementMessages()
        {
            NetOutgoingMessage msgOut;
            foreach (var character in dataHandler.characters)
            {
                msgOut = messageCreate.MovementMessage(character.Value);
                if (dataHandler.netConnections.Count > 0)
                    netServer.SendMessage(msgOut, dataHandler.netConnections, NetDeliveryMethod.UnreliableSequenced, 1);
            }
        }
        public void SendLogoutMessages(int id)
        {
            NetOutgoingMessage msgOut;
            msgOut = messageCreate.LogoutMessage(id);
            if (dataHandler.netConnections.Count > 0)
                netServer.SendMessage(msgOut, dataHandler.netConnections, NetDeliveryMethod.UnreliableSequenced, 1);
        }
        public void ClearConnections()
        {
            List<int> removeKeys = new List<int>();
            Console.WriteLine("Online: " + dataHandler.characters.Count);
            foreach (var character in dataHandler.characters)
            {
                Console.WriteLine(character.Value.name + ": " + character.Value.connection.Status);
                if (character.Value.connection.Status == NetConnectionStatus.Disconnected)
                {
                    removeKeys.Add(character.Key);
                }
            }
            for (int i = dataHandler.connections.Count - 1; i >= 0; i--)
            {
                if (dataHandler.connections[i].connection.Status == NetConnectionStatus.Disconnected)
                {
                    dataHandler.connections.RemoveAt(i);
                }
            }
            for (int i = dataHandler.netConnections.Count - 1; i >= 0; i--)
            {
                if (dataHandler.netConnections[i].Status == NetConnectionStatus.Disconnected)
                {
                    dataHandler.netConnections.RemoveAt(i);
                }
            }
            foreach (var key in removeKeys)
            {
                SendLogoutMessages(key);
                dataHandler.characters.Remove(key);
            }
        }
    }
}