using Lidgren.Network;
using Lidgren.Network.ServerFiles;
using Lidgren.Network.ServerFiles.Data;
using MMOLoginServer.ServerData;
using System;
using System.Collections.Generic;
using System.Text;

namespace MMOGameServer
{
    public class MessageHandler
    {
        NetServer netServer;

        DataHandler dataHandler;

        MessageReader messageReader;
        MessageCreater messageCreate;
        ConnectionData loginServer;
        public MessageHandler(NetServer server)
        {
            netServer = server;
            dataHandler = new DataHandler();
            messageCreate = new MessageCreater(netServer);
            messageReader = new MessageReader();
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
        }
        public void NewLoginToken(NetIncomingMessage msgIn)
        {
            LoginTokenData newLoginToken = messageReader.ReadLoginToken(msgIn);
            dataHandler.loginTokens.Add(newLoginToken);
        }
        public void LoginServerAuthentication(NetIncomingMessage msgIn)
        {
            byte[] encoded = PacketHandler.ReadEncryptedByteArray(msgIn);
            if (Encoding.UTF8.GetString(encoded) == "HARDCODEDKEY")
            {
                //loginServer = dataHandler.FindConnection(msgIn.SenderConnection) as LoginServerData;
                //Console.WriteLine(loginServer.connection.ToString());
                Console.WriteLine("LoginServerConnectionApproved");
            }
            else
            {
                loginServer = null;
                msgIn.SenderConnection.Disconnect("Bad AuthenticationKey");
            }
        }
        public void KeyExchange(NetIncomingMessage msgIn)
        {
            ConnectionData connection = messageReader.ReadKeyExchangeMessage(msgIn);
            dataHandler.connections.Add(connection);
            loginServer = connection ;

            NetOutgoingMessage msgOut = messageCreate.CreateRSAKeyMessage(netServer, ConnectionType.GameServer, dataHandler.serverName);
            msgIn.SenderConnection.Approve(msgOut);
        }
        public void ClientAuthentication(NetIncomingMessage msgIn)
        {
            byte[] clientLoginToken = PacketHandler.ReadEncryptedByteArray(msgIn);
            string username = Encoding.UTF8.GetString(PacketHandler.ReadEncryptedByteArray(msgIn));
            Console.WriteLine("Token length: " + clientLoginToken.Length);
            Console.WriteLine(BitConverter.ToString(clientLoginToken));
            Console.WriteLine(username);
            LoginTokenData validToken = null;
            if ((validToken = dataHandler.CheckLoginToken(clientLoginToken, username)) != null)
            {
                msgIn.SenderConnection.Approve();

                CharacterData characterData = new CharacterData();
                characterData.connection = msgIn.SenderConnection;
                characterData.id = validToken.characterData.id;
                characterData.name = validToken.characterData.name;
                characterData.currentHealth = 100;
                characterData.position = new System.Numerics.Vector3(0, 0, 0);
                characterData.rotation = 0;

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
        public void SendMessages()
        {
            NetOutgoingMessage msgOut;
            foreach (var character in dataHandler.characters)
            {
                msgOut = messageCreate.MovementMessage(character.Value);
                netServer.SendMessage(msgOut, dataHandler.netConnections, NetDeliveryMethod.UnreliableSequenced, 1);
            }
        }
    }
}