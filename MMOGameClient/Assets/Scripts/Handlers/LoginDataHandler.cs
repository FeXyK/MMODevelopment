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

                entity.health =  msgIn.ReadInt32();
                entity.maxHealth =  msgIn.ReadInt32();
                entity.mana = msgIn.ReadInt32();
                entity.maxMana = msgIn.ReadInt32();

                float x = msgIn.ReadFloat();
                float y = msgIn.ReadFloat();
                float z = msgIn.ReadFloat();
                entity.position = new Vector3(x, y, z);

                Dictionary<int, int> skills = new Dictionary<int, int>();
                int skillCount = msgIn.ReadInt16();

                for (int k = 0; k < skillCount; k++)
                {
                    int id = msgIn.ReadInt16();
                    int level = msgIn.ReadInt16();

                    skills.Add(id, level);
                }
                //Debug.Log("---");
                //Debug.Log(entity.characterName);
                //entity.skillTree = new SkillTree(skills);
                //Debug.Log("CHARACTER SKILLS: ");
                //foreach (var skill in entity.skillTree.skills)
                //{
                //    Debug.Log(skill.ID + " " + skill.Name + " " + skill.level);
                //}
                myCharacters.Add(entity);
            }
            selectionController.DrawCharacterItems(myCharacters);
        }
        public void LoadSkillList(NetIncomingMessage msgIn)
        {

            Debug.Log("SKILLDATA: ");
            Skill skill;
            int skillCount = msgIn.ReadInt16();

            int effectCount;

            string effectName;
            int effectID;
            int effectValue;
            float effectMultiplier;
            int effectMinLevel;

            float range;
            float rangeMultiplier;

            float levelingCost;
            float levelingCostMultiplier;

            float useCost;
            float useCostMultiplier;

            Effect effect;
            for (int k = 0; k < skillCount; k++)
            {
                skill = new Skill();

                skill.ID = msgIn.ReadInt16();
                skill.Name = msgIn.ReadString();
                skill.SkillType = msgIn.ReadInt16();

                range = msgIn.ReadFloat();
                rangeMultiplier = msgIn.ReadFloat();

                useCost = msgIn.ReadFloat();
                useCostMultiplier = msgIn.ReadFloat();

                levelingCost = msgIn.ReadFloat();
                levelingCostMultiplier = msgIn.ReadFloat();

                skill.SetRange(range, rangeMultiplier);
                skill.SetUseCost(useCost, useCostMultiplier);
                skill.SetLevelingCost(levelingCost, levelingCostMultiplier);


                int l1 = msgIn.ReadInt16();
                int l2 = msgIn.ReadInt16();
                int l3 = msgIn.ReadInt16();
                skill.SetRequiredLevel(l1, l2, l3);
                skill.RequiredSkillID = msgIn.ReadInt16();

                effectCount = msgIn.ReadInt16();
                for (int f = 0; f < effectCount; f++)
                {
                    //effectName = msgIn.ReadString(); //////////REMOVE NAME SEND AND DELETE THIS
                    effectID = msgIn.ReadInt16();
                    effectValue = msgIn.ReadInt16();
                    effectMinLevel = msgIn.ReadInt16();
                    effectMultiplier = msgIn.ReadFloat();

                    effect = new Effect((EffectValue)effectID, effectValue, effectMinLevel, effectMultiplier);
                    skill.effects.Add(effect);
                }
                SkillLibrary.Skills().Add(skill);
            }
            //Debug.Log("----------------------------------------------------------");
            //foreach (var s in SkillLibrary.Skills())
            //{
            //    Debug.Log(s);
            //}
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
