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
        
        AddedItem,
        RemovedItem,

        DroppedLootItem,
        PickUpItem,
        
        EquipItem,
        UnequipItem,
        DropItem,
        MoveItem,
        DestroyItem,

        StorageInfo,
        StorageTakeOut,
        StoragePutIn,

        LevelUpItem,
        LeveledItem,
        DisassembleItem,

        
        HideNames,
        ShowNames,

        AdminChatMessage,
        PublicChatMessage,
        PrivateChatMessage,

        SkillListInformation,
        StartSkillCast,
        EndSkillCast,
        SkillCasted,
        SkillLeveled,
        EntityUpdate,

        UsePotion,
        UseFood,
        UseEqippeable,

        Client,
        WorldServer
    }
}
