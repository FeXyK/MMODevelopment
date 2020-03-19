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
    public int health;
    public int type;
    public int gold;
    public float posX;
    public float posY;
    public float posZ;
    public float rot;
    public Transform character;
    public TMPro.TMP_Text NameText;
    public Slider HealthBar;
    public Entity(int cId, int cLevel, int cHealth, int cType, string cName)
    {
        id = cId;
        level = cLevel;
        health = cHealth;
        type = cType;
        characterName = cName;
        NameText.text = id + ": " + characterName;
    }
    public void Set(int cId, int cLevel, int cHealth, int cType, string cName)
    {
        id = cId;
        level = cLevel;
        health = cHealth;
        type = cType;
        characterName = cName;
        NameText.text = id + ": " + characterName;
    }
    public void Set(CharacterData characterData)
    {
        id = characterData.id;
        level = characterData.level;
        health = 100;
        type = characterData.characterType;
        characterName = characterData.name;
        NameText.text = id + ": " + characterName;
    }
    public void Set(Entity ch)
    {
        id = ch.id;
        level = ch.level;
        health = ch.health;
        type = ch.type;
        characterName = ch.name;
        NameText.text = id + ": " + characterName;
    }
    public void SpawnCharacter()
    {
        character = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;

    }
    float tickRate;
    float time;
    public void Update()
    {
        time += Time.deltaTime;
        if (time > 1)
        {
            health--;
            if (health == 0)
                health = 100;
            time = 0;
        }
        tickRate = (1000f / 20f) * Time.deltaTime;
        HealthBar.value = health;

        if (this.gameObject.tag != "PlayerCharacter")
        {
            character.position = Vector3.Lerp(character.position, new Vector3(posX, posY, posZ), tickRate);
            //Quaternion toRotation = Quaternion.LookRotation((new Vector3(posX, 0, posZ) - character.position).normalizedsZ));
            //transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, speed * Time.time);
            character.LookAt(new Vector3(posX, character.position.y, posZ));// new Vector3(0, rot, 0);
        }
    }
}
