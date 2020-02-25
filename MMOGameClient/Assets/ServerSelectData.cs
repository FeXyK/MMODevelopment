using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ServerSelectData : MonoBehaviour
{
    public Text Name;
    public Text Selected;
    public GameServerData serverData;
    public void Load(GameServerData data)
    {
        Name.text = data.name + " [" + data.ip + ":" + data.port + "]";
        serverData = data;
    }
    public void SetSelection()
    {
        LoginSceneInputs loginSceneInputs = FindObjectOfType<LoginSceneInputs>();
        if (loginSceneInputs.selectedServer != null)
        {
            loginSceneInputs.selectedServer.GetComponent<CharacterButtonContainer>().Selected.text = "";
        }
        loginSceneInputs.selectedServer = this.gameObject;

        Selected.text = "SELECTED";
    }
}
