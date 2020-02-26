
namespace Lidgren.Network.ServerFiles
{
    public enum MessageType
    {
        ServerLoginRequest,
        ServerLoginAnswerOk,
        ServerLoginError,
        
        RegisterRequest,
        RegisterAnswerOk,
        RegisterAnswerError,

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
        CharacterMovement  
    }
}
