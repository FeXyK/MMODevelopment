using Assets.Scripts.Character;
using Assets.Scripts.LoginScreen;
using Assets.Scripts.SkillSystem.SkillSys;
using Lidgren.Network;
using Lidgren.Network.ServerFiles.Data;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Handlers
{
    public class LoginDataHandler
    {
        private static LoginDataHandler Instance;
        public byte[] authenticationToken;

        public List<Entity> myCharacters = new List<Entity>();
        public List<GameServerData> worldServers = new List<GameServerData>();

        public Entity selectedCharacter;
        public GameServerData selectedWorldServer;

        public SelectionController selectionController;
        public LoginScreenInputData inputData;
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
            Entity entity;
            int accountID;
            int count = msgIn.ReadInt16();
            Debug.Log("MYCHARACTERS COUNT: " + myCharacters.Count + " " + count);
            for (int i = 0; i < count; i++)
            {
                entity = new Entity();
                entity.characterName = msgIn.ReadString();
                entity.id = msgIn.ReadInt32();
                accountID = msgIn.ReadInt32();
                entity.level = msgIn.ReadInt32();
                entity.exp = msgIn.ReadInt32();
                entity.gold = msgIn.ReadInt32();
                entity.characterType = (CharacterApperance)msgIn.ReadInt32();

                entity.health = msgIn.ReadInt32();
                entity.maxHealth = msgIn.ReadInt32();
                entity.mana = msgIn.ReadInt32();
                entity.maxMana = msgIn.ReadInt32();

                float x = msgIn.ReadFloat();
                float y = msgIn.ReadFloat();
                float z = msgIn.ReadFloat();
                entity.position = new Vector3(x, y, z);

                entity.skills = new Dictionary<int, int>();

                int skillCount = msgIn.ReadInt16();
                for (int k = 0; k < skillCount; k++)
                {
                    int id = msgIn.ReadInt16();
                    int level = msgIn.ReadInt16();

                    entity.skills.Add(id, level);
                }

                int inventoryCount = msgIn.ReadInt16();
                for (int k = 0; k < inventoryCount; k++)
                {
                    int id = msgIn.ReadInt32();
                    int[] values = new int[6];
                    values[0] = msgIn.ReadInt32();
                    values[1] = msgIn.ReadInt16();
                    values[2] = msgIn.ReadInt16();
                    values[3] = msgIn.ReadInt16();
                    values[4] = msgIn.ReadInt16();
                    values[5] = msgIn.ReadInt16();

                    entity.inventory.Add(id, values);
                }
                myCharacters.Add(entity);
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
