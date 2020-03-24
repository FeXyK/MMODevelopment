using Lidgren.Network.ServerFiles.Data;

public class LoginDataController
{
    public byte[] authToken;
    public string characterName;
    public int characterID;
    public string publicKey;
    public string serverIP;
    public int serverPort;

    public CharacterData characterData;
}
