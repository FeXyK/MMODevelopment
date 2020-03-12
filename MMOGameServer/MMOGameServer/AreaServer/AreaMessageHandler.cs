using Lidgren.Network;
using Lidgren.Network.ServerFiles;
using Lidgren.Network.ServerFiles.Data;
using MMOLoginServer.ServerData;
using System;
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
        public bool hideNames = false;
        public AreaMessageHandler(NetServer server)
        {
            netServer = server;
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
        public void NewLoginToken(NetIncomingMessage msgIn)
        {
            AuthenticationTokenData newLoginToken = messageReader.ReadLoginToken(msgIn);
            dataHandler.loginTokens.Add(newLoginToken);
        }
        //public void LoginServerAuthentication(NetIncomingMessage msgIn)
        //{
        //    byte[] encoded = PacketHandler.ReadEncryptedByteArray(msgIn);
        //    if (Encoding.UTF8.GetString(encoded) == "HARDCODEDKEY")
        //    {
        //        //loginServer = dataHandler.FindConnection(msgIn.SenderConnection) as LoginServerData;
        //        //Console.WriteLine(loginServer.connection.ToString());
        //        Console.WriteLine("LoginServerConnectionApproved");
        //    }
        //    else
        //    {
        //        loginServer = null;
        //        msgIn.SenderConnection.Disconnect("Bad AuthenticationKey");
        //    }
        //}
        public NetOutgoingMessage CreateHideNamesMessage()
        {
            NetOutgoingMessage msgOut = netServer.CreateMessage();
            if (hideNames)
                msgOut.Write((byte)MessageType.HideNames);
            else
                msgOut.Write((byte)MessageType.ShowNames);
            return msgOut;
        }
        //public void KeyExchange(NetIncomingMessage msgIn)
        //{
        //    ConnectionData connection = messageReader.ReadKeyExchangeMessage(msgIn);
        //    dataHandler.connections.Add(connection);
        //    loginServer = connection;

        //    NetOutgoingMessage msgOut = messageCreate.CreateRSAKeyMessage(netServer, ConnectionType.GameServer, dataHandler.serverName);
        //    msgIn.SenderConnection.Approve(msgOut);
        //}

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
            string username = Encoding.UTF8.GetString(PacketHandler.ReadEncryptedByteArray(msgIn));
            Console.WriteLine("Token length: " + clientLoginToken.Length);
            Console.WriteLine(BitConverter.ToString(clientLoginToken));
            Console.WriteLine(username);
            AuthenticationTokenData validToken = null;
            if ((validToken = dataHandler.CheckLoginToken(clientLoginToken, username)) != null)
            {
                msgIn.SenderConnection.Approve();

                CharacterData characterData = new CharacterData();
                characterData.connection = msgIn.SenderConnection;
                //characterData.id = validToken.characterData.id;
                //characterData.name = validToken.characterData.name;
                characterData.currentHealth = 100;
                characterData.position = new System.Numerics.Vector3(0, 0, 0);
                characterData.rotation = 0;
                if (dataHandler.characters.ContainsKey(characterData.id))
                    dataHandler.characters.Remove(characterData.id);
                dataHandler.characters.Add(characterData.id, characterData);
                dataHandler.netConnections.Add(msgIn.SenderConnection);
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
            dataHandler.characters[id].position = new System.Numerics.Vector3(x, y, z);
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
            //for (int i = dataHandler.loginTokens.Count; i >= 0; i++)
            //{
            //    if (dataHandler.loginTokens[i].expireDate < DateTime.Now)//== NetConnectionStatus.Disconnected)
            //    {
            //        dataHandler.loginTokens.RemoveAt(i);
            //    }
            //}
        }
    }
}