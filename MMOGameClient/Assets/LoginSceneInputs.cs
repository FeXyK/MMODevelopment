using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginSceneInputs : MonoBehaviour
{
    public GameObject SwitchFormsButton;
    public TMP_Text SwitchFormsText;

    public GameObject LoginForm;
    public GameObject RegisterForm;
    public GameObject CharacterCreateForm;
    public GameObject CharacterSelectForm;

    public TMP_Text DText;

    public Button CharacterButton;

    private string characterType;
    private string characterName;

    private string usernameReg;
    private string emailReg;
    private string passwordReg;
    private string passwordRegConfirm;

    private string username;
    private string password;
    
    public string Password { get => password; set => password = value; }
    public string Username { get => username; set => username = value; }
    public string PasswordRegConfirm { get => passwordRegConfirm; set => passwordRegConfirm = value; }
    public string PasswordReg { get => passwordReg; set => passwordReg = value; }
    public string EmailReg { get => emailReg; set => emailReg = value; }
    public string UsernameReg { get => usernameReg; set => usernameReg = value; }
    public string CharacterName { get => characterName; set => characterName = value; }
    public string CharacterType { get => characterType; set => characterType = value; }
    public string MyProperty { get; set; }
    private void Start()
    {

    }
}
