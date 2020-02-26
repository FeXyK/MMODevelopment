using Lidgren.Network.ServerFiles;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace MMOLoginServer.ServerData
{
    public class CharacterData : ConnectionData
    {
        public int maxHealth;
        public int maxMana;

        public int id;
        public int accountID;
        public int currentHealth;
        public int currentMana;
        public int currentExp;
        public int level;
        public Vector3 position;
        public float rotation;
        //  public CharacterSkillTree skillTree;
        public int characterType;
        public int gold;
        public CharacterData()
        {

        }
        public CharacterData(params string[] param)
        {
            id = int.Parse(param[0]);
            accountID = int.Parse(param[1]);
            name = param[2];
            currentHealth = int.Parse(param[3]);
            currentMana = int.Parse(param[4]);
            currentExp = int.Parse(param[5]);
            level = int.Parse(param[6]);
            position = new Vector3(float.Parse(param[7]), float.Parse(param[8]), float.Parse(param[9]));
            rotation = int.Parse(param[10]);
            // skillTree = new CharacterSkillTree(param[11]);
            characterType = int.Parse(param[12]);
            gold = int.Parse(param[13]);
        }
        public override string ToString()
        {
            return String.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10}", name, id, accountID, level, gold, characterType, position.ToString(), rotation, currentExp, currentHealth, currentMana);
        }
    }
}
