using Assets.Scripts.Character;
using Assets.Scripts.SkillSystem.SkillSys;
using Lidgren.Network.ServerFiles.Data;
using UnityEngine;
using UnityEngine.UI;

public class Entity
{
    public string characterName;
    public int id;
    public int level;
    public int exp;

    public int health;
    public int maxHealth;

    public int mana;
    public int maxMana;
    public int gold;
    public Vector3 position;

    public CharacterApperance characterType;

    public float tickRate = 0;
    internal SkillTree skillTree;

    public void Set(Entity ch)
    {
        id = ch.id;
        level = ch.level;
        characterType = ch.characterType;
        characterName = ch.characterName;
    }
}
