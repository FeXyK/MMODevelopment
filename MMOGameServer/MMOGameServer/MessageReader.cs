using Lidgren.Network;
using Lidgren.Network.ServerFiles;
using Lidgren.Network.ServerFiles.Data;
using MMOLoginServer.ServerData;

namespace MMOGameServer
{
    public class MessageReader
    {
        public  ConnectionData ReadKeyExchangeMessage(NetIncomingMessage msgIn)
        {
            ConnectionData connection = new ConnectionData();
            System.Console.WriteLine(msgIn.SenderConnection.ToString());
            connection.port = msgIn.SenderEndPoint.Port;
            connection.ip = msgIn.SenderEndPoint.Address.ToString();
            connection.connection = msgIn.SenderConnection;
            connection.type = (ConnectionType)msgIn.ReadByte();
            connection.publicKey = msgIn.ReadString();
            return connection;
        }
        public LoginTokenData ReadLoginToken(NetIncomingMessage msgIn)
        {
            LoginTokenData newLoginToken = new LoginTokenData();
            newLoginToken.characterData = new CharacterData();

            newLoginToken.token = PacketHandler.ReadEncryptedByteArray(msgIn);
            newLoginToken.expireDate = msgIn.ReadString();
            newLoginToken.characterData.id = msgIn.ReadInt16();
            newLoginToken.characterData.level = msgIn.ReadInt16();
            newLoginToken.characterData.currentHealth = msgIn.ReadInt16();
            newLoginToken.characterData.characterType = msgIn.ReadInt16();
            newLoginToken.characterData.name = msgIn.ReadString();
            return newLoginToken;
        }
    }
}
