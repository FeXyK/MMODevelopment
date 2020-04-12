using Assets.Scripts.Character;
using System.Collections.Generic;
using UnityEngine;

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

    public Dictionary<int,int> skills = new Dictionary<int, int>();

    public float tickRate = 0;
}
