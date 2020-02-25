using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterButtonContainer : MonoBehaviour
{
    public Text Name;
    public Text Level;
    public Text AccounID;
    public Text CharacterID;
    public Text CharacterType;
    public Text Gold;
    public Text Selected;
    public void Load(string data)
    {
        string[] characterData = data.Split(';');
        Name.text = characterData[0];
        CharacterID.text = "ChID: " + characterData[1];
        AccounID.text = "AccID: " + characterData[2];
        Level.text = "Level: " + characterData[3];
        Gold.text = "Gold: " + characterData[4] + "g";
        CharacterType.text = "ChType: " + characterData[5];
    }

    public void SetSelection()
    {
        LoginSceneInputs loginSceneInputs = FindObjectOfType<LoginSceneInputs>();
        if (loginSceneInputs.selectedCharacter != null)
        {
            loginSceneInputs.selectedCharacter.GetComponent<CharacterButtonContainer>().Selected.text = "";
        }
        loginSceneInputs.selectedCharacter = this.gameObject;

        Selected.text = "SELECTED";
    }
}
