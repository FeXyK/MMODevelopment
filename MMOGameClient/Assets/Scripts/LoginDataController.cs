using Assets.Scripts.LoginScreen;
using MMOLoginServer.ServerData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
