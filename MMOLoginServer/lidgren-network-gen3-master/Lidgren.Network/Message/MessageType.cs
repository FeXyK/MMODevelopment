
namespace Lidgren.Network.ServerFiles
{
    public enum MessageType
    {
        ServerLoginRequest,
        ServerLoginSuccess,

        RegisterRequest,

        CharacterData,
        CreateCharacter,
        DeleteCharacter,

        LoginToken,
        AuthToken,
        LoginServerConnect,
        LoginServerAuthentication,
        ClientAuthentication,   

        CharacterLogin,

        NewLoginToken,
        GameServersData,
        KeyExchange,

        ClientReady,
        NewCharacter,
        OtherCharacterRemove,
        CharacterMovement ,
        Notification,

        HideNames,
        ShowNames,

        AdminChatMessage,
        PublicChatMessage,
        PrivateChatMessage
    }
}
