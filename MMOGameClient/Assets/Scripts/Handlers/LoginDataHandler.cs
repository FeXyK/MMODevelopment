using Assets.Scripts.LoginScreen;
using Lidgren.Network;
using Lidgren.Network.ServerFiles;
using MMOLoginServer.ServerData;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;

namespace Assets.Scripts.Handlers
{
    public class LoginDataHandler
    {
        private static LoginDataHandler Instance;
        public byte[] authenticationToken;
        
        public List<CharacterData> myCharacters = new List<CharacterData>();
        public List<GameServerData> worldServers = new List<GameServerData>();

        public CharacterData selectedCharacter;
        public GameServerData selectedWorldServer;

        public SelectionController selectionController;
        public  LoginScreenInputData inputData;
        public byte[] GetAuthToken()
        {
            return authenticationToken;
        }
        public static LoginDataHandler GetInstance()
        {
            if (Instance == null)
                Instance = new LoginDataHandler();
            return Instance;
        }
        public LoginDataHandler()
        {
            if (Instance == null)
            {
                Instance = this;
                selectionController = GameObject.FindObjectOfType<SelectionController>();
                inputData = GameObject.FindObjectOfType<LoginScreenInputData>();
            }
        }
        public void LoadCharacterData(NetIncomingMessage msgIn)
        {
            myCharacters.Clear();
            Debug.Log("MYCHARACTERS COUNT: " + myCharacters.Count);
            CharacterData character;

            int count = msgIn.ReadInt16();
            for (int i = 0; i < count; i++)
            {
                character = new CharacterData();

                character.name = msgIn.ReadString();
                character.id = msgIn.ReadInt32();
                character.accountID = msgIn.ReadInt32();
                character.level = msgIn.ReadInt32();
                character.gold = msgIn.ReadInt32();
                character.characterType = msgIn.ReadInt32();

                myCharacters.Add(character);
            }
            selectionController.DrawCharacterItems(myCharacters);
        }
        public void LoadGameServerData(NetIncomingMessage msgIn)
        {
            worldServers.Clear();
            GameServerData gameServer;

            int count = msgIn.ReadInt16();
            for (int i = 0; i < count; i++)
            {
                gameServer = new GameServerData();
                gameServer.name = msgIn.ReadString();
                gameServer.ip = msgIn.ReadString();
                gameServer.port = msgIn.ReadInt32();
                gameServer.publicKey = msgIn.ReadString();

                worldServers.Add(gameServer);
            }
            selectionController.DrawServerItems(worldServers);
        }
    }
}
