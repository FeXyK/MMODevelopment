
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
        Encrypted,
        CharacterData,
        CreateCharacter,
        DeleteCharacter,
        GameServer,
        Client
    }
}
