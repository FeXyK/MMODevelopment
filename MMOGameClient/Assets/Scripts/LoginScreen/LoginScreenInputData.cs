using TMPro;
using UnityEngine;

public class LoginScreenInputData : MonoBehaviour
{
    private int characterType;
    private string characterName;

    private string usernameReg;
    private string emailReg;
    private string passwordReg;
    private string passwordRegConfirm;

    private string username="qwe";
    private string password="qwe";
    public int selectedCharacterNumber { get; set; }
    public int selectedServerNumber { get; set; }
    public string Password { get => password; set => password = value; }
    public string Username { get => username; set => username = value; }
    public string PasswordRegConfirm { get => passwordRegConfirm; set => passwordRegConfirm = value; }
    public string PasswordReg { get => passwordReg; set => passwordReg = value; }
    public string EmailReg { get => emailReg; set => emailReg = value; }
    public string UsernameReg { get => usernameReg; set => usernameReg = value; }
    public string CharacterName { get => characterName; set => characterName = value; }
    public int CharacterType { get => characterType; set { characterType = value;Debug.LogError(characterType); } }
    
    public void OnDropDownChanged(TMP_Dropdown dropdown)
    {
        CharacterType = dropdown.value;
    }

}
