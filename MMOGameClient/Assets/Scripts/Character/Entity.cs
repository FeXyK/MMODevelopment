using Assets.Scripts.Character;
using MMOLoginServer.ServerData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Entity : MonoBehaviour
{
    public string characterName;
    public int id;
    public int level;
    public int exp;
    public int mana;
    private int health;

    public int Health
    {
        get { return health; }
        set
        {
            health = value;
            HealthBar.value = health;
        }
    }
    private int maxHealth;
    public int MaxHealth
    {
        get { return maxHealth; }
        set
        {
            maxHealth = value;
            HealthBar.maxValue = maxHealth;
        }
    }
    public CharacterApperance characterType;
    public int gold;
    public float posX;
    public float posY;
    public float posZ;
    public float rot;
    public Transform character;
    public TMPro.TMP_Text NameText;
    public Slider HealthBar;
    public float tickRate = 0;
    public void Set(int cId, int cLevel, int cHealth, CharacterApperance cType, string cName, int cMaxHealth)
    {
        id = cId;
        level = cLevel;
        Health = cHealth;
        characterType = cType;
        characterName = cName;
        NameText.text = id + ": " + characterName;
        MaxHealth = cMaxHealth;
    }
    public void Set(CharacterData characterData)
    {
        id = characterData.id;
        level = characterData.level;
        Health = characterData.currentHealth;
        characterType = (CharacterApperance)characterData.characterType;
        characterName = characterData.name;
        NameText.text = id + ": " + characterName;
        MaxHealth = characterData.maxHealth;
    }
    public void Set(Entity ch)
    {
        id = ch.id;
        level = ch.level;
        Health = ch.health;
        characterType = ch.characterType;
        characterName = ch.name;
        NameText.text = id + ": " + characterName;
        MaxHealth = ch.MaxHealth;
    }
    public void Update()
    {
        tickRate = (1000f / 20f) * Time.deltaTime;
        if (this.gameObject.tag != "PlayerCharacter")
        {
            character.position = Vector3.Lerp(character.position, new Vector3(posX, posY, posZ), tickRate);
            character.LookAt(new Vector3(posX, character.position.y, posZ));// new Vector3(0, rot, 0);
        }
    }
}
