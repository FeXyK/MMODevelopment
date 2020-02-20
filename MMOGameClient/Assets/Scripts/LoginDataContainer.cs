using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginDataContainer : MonoBehaviour
{
    public byte[] authToken;
    public Character character;
    public List<GameServerData> gameServerDatas = new List<GameServerData>();
}
