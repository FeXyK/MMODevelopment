using Lidgren.Network;
using Lidgren.Network.ServerFiles;
using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginSceneInputs : MonoBehaviour
{
    public NetClient netClient;
    public GameObject SwitchFormsButton;
    public TMP_Text SwitchFormsText;

    public GameObject LoginForm;
    public GameObject RegisterForm;
    public GameObject CharacterCreateForm;
    public GameObject CharacterSelectForm;
    public GameObject ServerSelectForm;
    public TMP_Text DText;

    public Button CharacterButton;
    public Button ServerButton;
    private int characterNumber = 0;
    private int serverNumber = 0;
    public GameObject selectedCharacter = null;
    private List<Button> characterButtons = new List<Button>();
    private List<Button> serverButtons = new List<Button>();
    public GameObject selectedServer = null;


    public SceneLoader sceneLoader;

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
    public void PlayCharacter()
    {
        NetOutgoingMessage msgOut = netClient.CreateMessage();
        msgOut.Write((byte)MessageType.CharacterLogin);
        msgOut.Write("Europe");
        msgOut.Write(selectedCharacter.GetComponent<CharacterButtonContainer>().Name.text);
        netClient.ServerConnection.SendMessage(msgOut, NetDeliveryMethod.ReliableOrdered, 1);
    }
    public void SwitchForms()
    {
        LoginForm.SetActive(!LoginForm.activeSelf);
        RegisterForm.SetActive(!RegisterForm.activeSelf);

        if (!LoginForm.activeSelf)
            SwitchFormsText.text = "Login";
        else
            SwitchFormsText.text = "Registration";
    }
    public void ShowCharacterCreation()
    {
        ClearCharacterSelection();
        CharacterCreateForm.SetActive(true);
    }
    public void ClearCharacterSelection()
    {
        foreach (var characterButton in characterButtons)
        {
            Destroy(characterButton.gameObject);
        }
        characterButtons.Clear();
        characterNumber = 0;
        selectedCharacter = null;
    }
    public void HandleCharacterData(NetIncomingMessage msgIn)
    {
        byte[] characterData = PacketHandler.ReadEncryptedByteArray(msgIn);
        DText.text += Encoding.UTF8.GetString(characterData) + "\n";

        Button characterButton = Instantiate(CharacterButton);
        characterButton.transform.SetParent(CharacterSelectForm.transform);
        RectTransform rt = characterButton.GetComponent<RectTransform>();
        rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 10, 300);
        rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 10 + characterNumber * 110, 100);
        characterNumber++;
        characterButton.GetComponent<CharacterButtonContainer>().Load(Encoding.UTF8.GetString(characterData));
        characterButtons.Add(characterButton);
    }
    public void Register()
    {
        if (PasswordReg == PasswordRegConfirm)
        {
            NetOutgoingMessage msgRegister = netClient.CreateMessage();
            byte[] hashPassword = DataEncryption.HashString(PasswordReg);

            msgRegister.Write((byte)MessageType.RegisterRequest);

            PacketHandler.WriteEncryptedByteArray(msgRegister, UsernameReg);
            PacketHandler.WriteEncryptedByteArray(msgRegister, hashPassword);
            PacketHandler.WriteEncryptedByteArray(msgRegister, EmailReg);

            netClient.SendMessage(msgRegister, NetDeliveryMethod.ReliableOrdered);
            Debug.Log("Registration sent");
        }
    }
    public void HandleGameServerData(NetIncomingMessage msgIn, List<GameServerData> gameServerDatas)
    {
        GameServerData gameServerData;

        int count = msgIn.ReadInt16();
        for (int i = 0; i < count; i++)
        {
            gameServerData = new GameServerData();
            gameServerData.name = msgIn.ReadString();
            gameServerData.ip = msgIn.ReadString();
            gameServerData.port = msgIn.ReadInt32();
            gameServerData.publicKey = msgIn.ReadString();
            gameServerDatas.Add(gameServerData);
            LoadServerData(gameServerData);
        }
    }
    public void HandleAuthenticationToken(NetIncomingMessage msgIn, ref byte[] authToken)
    {
        authToken = PacketHandler.ReadEncryptedByteArray(msgIn);
        DText.text += authToken.Length + "\n";
        DText.text += BitConverter.ToString(authToken);
    }
    private void HandleCharacterData(string dataEncrypted)
    {
        Debug.Log(dataEncrypted);
    }
    public void CreateCharacter()
    {
        NetOutgoingMessage msgCreate = netClient.CreateMessage();

        msgCreate.Write((byte)MessageType.CreateCharacter);

        PacketHandler.WriteEncryptedByteArray(msgCreate, CharacterName);
        PacketHandler.WriteEncryptedByteArray(msgCreate, "male");
        Debug.Log(CharacterName);
        Debug.Log(CharacterType);
        netClient.SendMessage(msgCreate, NetDeliveryMethod.ReliableOrdered);
        CharacterCreateForm.SetActive(false);
    }
    public void DeleteCharacter()
    {
        NetOutgoingMessage msgDelete = netClient.CreateMessage();

        msgDelete.Write((byte)MessageType.DeleteCharacter);

        PacketHandler.WriteEncryptedByteArray(msgDelete, selectedCharacter.GetComponent<CharacterButtonContainer>().Name.text);
        ClearCharacterSelection();
        netClient.SendMessage(msgDelete, NetDeliveryMethod.ReliableOrdered);
    }
    public void Login()
    {
        NetOutgoingMessage msgLogin = netClient.CreateMessage();

        byte[] hashPassword = DataEncryption.HashString(Password);

        msgLogin.Write((byte)MessageType.ClientAuthentication);

        PacketHandler.WriteEncryptedByteArray(msgLogin, Username);
        PacketHandler.WriteEncryptedByteArray(msgLogin, hashPassword);

        netClient.SendMessage(msgLogin, NetDeliveryMethod.ReliableOrdered);
    }
    public void LoadServerData(GameServerData data)
    {
        Button serverButton = Instantiate(ServerButton);
        serverButton.transform.SetParent(ServerSelectForm.transform);
        RectTransform rt = serverButton.GetComponent<RectTransform>();
        rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 10, 300);
        rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 10 + serverNumber * 110, 100);
        serverNumber++;
        serverButton.GetComponent<ServerSelectData>().Load(data);
        serverButtons.Add(serverButton);
    }
}
