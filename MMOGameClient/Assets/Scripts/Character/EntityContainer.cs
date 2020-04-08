using Lidgren.Network.ServerFiles.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Character
{
    public class EntityContainer : MonoBehaviour
    {
        public TMPro.TMP_Text NameText;
        public Slider HealthBar;
        public Slider ManaBar;

        public float tickRate;
        public Entity entity = new Entity();

        public int Health
        {
            get { return entity.health; }
            set
            {
                entity.health = value;
                if (HealthBar != null)
                    HealthBar.value = entity.health;
            }
        }
        public int MaxHealth
        {
            get { return entity.maxHealth; }
            set
            {
                entity.maxHealth = value;
                if (HealthBar != null)
                {
                    HealthBar.maxValue = entity.maxHealth;
                }
            }
        }
        private int mana;

        public int Mana
        {
            get { return mana; }
            set
            {
                mana = value;
                ManaBar.value = mana;
            }
        }
        private int maxMana;

        public int MaxMana
        {
            get { return maxMana; }
            set
            {
                maxMana = value;
                ManaBar.maxValue = maxMana;
            }
        }
        private void Awake()
        {
            if (HealthBar == null)
            {
                HealthBar = GameObject.FindGameObjectWithTag("Player_HealthBar").GetComponent<Slider>();
                Debug.LogWarning("Player_HealthBar");
            }
            if (ManaBar == null)
            {
                ManaBar = GameObject.FindGameObjectWithTag("Player_ManaBar").GetComponent<Slider>();
            }
        }
        public void Update()
        {
            tickRate = (1000f / 20f) * Time.deltaTime;
            if (this.gameObject.tag != "PlayerCharacter")
            {
                this.transform.position = Vector3.Lerp(this.transform.position, entity.position, tickRate);
                this.transform.LookAt(new Vector3(entity.position.x, entity.position.y, entity.position.y));
            }
        }
        public void Set(int cId, int cLevel, int cHealth, CharacterApperance cType, string cName, int cMaxHealth, int cMaxMana = 100, int cMana = 100)
        {
            entity.id = cId;
            entity.level = cLevel;
            entity.characterType = cType;
            entity.characterName = cName;
            NameText.text = entity.id + ": " + entity.characterName;
            MaxHealth = cMaxHealth;
            Health = cHealth;
            MaxMana = cMaxMana;
            Mana = cMana;
        }
        public void Set(CharacterData characterData)
        {
            entity.id = characterData.id;
            entity.level = characterData.level;
            entity.characterType = (CharacterApperance)characterData.characterType;
            entity.characterName = characterData.name;
            MaxHealth = characterData.maxHealth;
            Health = characterData.currentHealth;
            MaxMana = characterData.maxMana;
            Mana = characterData.currentMana;
        }
        public void Set(Entity ch)
        {
            entity.id = ch.id;
            entity.level = ch.level;
            entity.characterType = ch.characterType;
            entity.characterName = ch.characterName;
            NameText.text = entity.id + ": " + entity.characterName;
            MaxHealth = ch.maxHealth;
            Health = ch.health;
            MaxMana = ch.maxMana;
            Mana = ch.mana;


            Debug.LogWarning(ch.maxHealth);
            Debug.LogWarning(ch.health);
        }
    }
}