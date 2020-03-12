using Lidgren.Network;
using Lidgren.Network.ServerFiles;
using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginScreenInputData : MonoBehaviour
{
    private string characterType;
    private string characterName;

    private string usernameReg;
    private string emailReg;
    private string passwordReg;
    private string passwordRegConfirm;

    private string username;
    private string password;
    public int selectedCharacterNumber { get; set; }
    public int selectedServerNumber { get; set; }
    public string Password { get => password; set => password = value; }
    public string Username { get => username; set => username = value; }
    public string PasswordRegConfirm { get => passwordRegConfirm; set => passwordRegConfirm = value; }
    public string PasswordReg { get => passwordReg; set => passwordReg = value; }
    public string EmailReg { get => emailReg; set => emailReg = value; }
    public string UsernameReg { get => usernameReg; set => usernameReg = value; }
    public string CharacterName { get => characterName; set => characterName = value; }
    public string CharacterType { get => characterType; set => characterType = value; }
    public string MyProperty { get; set; }
}
