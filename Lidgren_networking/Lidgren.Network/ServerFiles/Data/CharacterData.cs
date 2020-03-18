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

        public int accountID;
        public int currentHealth;
        public int currentMana;
        public int currentExp;
        public int level;
        public float positionX;
        public float positionY;
        public float positionZ;
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
            positionX = float.Parse(param[7]);
            positionY = float.Parse(param[8]);
            positionZ = float.Parse(param[9]);
            rotation = float.Parse(param[10]);
            // skillTree = new CharacterSkillTree(param[11]);
            characterType = int.Parse(param[12]);
            gold = int.Parse(param[13]);
        }
        public override string ToString()
        {
            return String.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};{11};{12}", name, id, accountID, level, gold, characterType, positionX,positionY,positionZ, rotation, currentExp, currentHealth, currentMana);
        }
    }
}
