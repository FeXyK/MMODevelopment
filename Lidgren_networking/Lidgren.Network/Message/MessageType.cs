﻿
namespace Lidgren.Network.Message
{
    public enum MessageType
    {
        Alive,

        ServerLoginRequest,
        ServerLoginSuccess,
        AuthenticationSuccess,

        RegisterRequest,

        CharacterListRequest,
        CharacterData,
        CreateCharacter,
        DeleteCharacter,
        PlayCharacter,

        LoginToken,
        AuthToken,
        LoginServerConnect,
        LoginServerAuthentication,
        ClientAuthentication,
        ClientAuthenticated,
        WorldServerAuthentication,
        WorldServerAuthenticated,
        WorldServerAuthenticationTokenRequest,

        NewAuthenticationToken,
        GameServersData,
        KeyExchange,

        ClientReady,
        NewCharacter,
        OtherCharacterRemove,
        CharacterMovement,
        Notification,
        MobInformation,
        NewMobAreaData,

        HideNames,
        ShowNames,

        AdminChatMessage,
        PublicChatMessage,
        PrivateChatMessage,
        
        StartSkillCast,
        EndSkillCast,
        SkillCasted,
        EntityUpdate,

        Client,
        WorldServer
    }
}
