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
        public Collider coll;
        public int Health
        {
            get { return entity.health; }
            set
            {
                entity.health = value;
                if (entity.health > 0)
                    gameObject.layer = 0;
                else
                    gameObject.layer = 2;
            }
        }
        public int MaxHealth
        {
            get { return entity.maxHealth; }
            set
            {
                entity.maxHealth = value;
            }
        }

        public int Mana
        {
            get { return entity.mana; }
            set
            {
                entity.mana = value;
            }
        }

        public int MaxMana
        {
            get { return entity.maxMana; }
            set
            {
                entity.maxMana = value;
            }
        }
        private void Awake()
        {

            if (this.gameObject.tag == "PlayerCharacter")
            {
                if (HealthBar == null)
                {
                    HealthBar = GameObject.FindGameObjectWithTag("Player_HealthBar").GetComponent<Slider>();
                }
                if (ManaBar == null)
                {
                    ManaBar = GameObject.FindGameObjectWithTag("Player_ManaBar").GetComponent<Slider>();
                }
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

            if (HealthBar == null)
            {
                HealthBar = GameObject.FindGameObjectWithTag("Player_HealthBar").GetComponent<Slider>();
            }

            HealthBar.maxValue = entity.maxHealth;
            HealthBar.value = entity.health;
            if (this.gameObject.tag == "PlayerCharacter")
            {
                ManaBar.maxValue = 1000;
                ManaBar.value = entity.mana;
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
        public void Set(Entity entity)
        {
            this.entity.id = entity.id;
            this.entity.level = entity.level;
            this.entity.characterType = entity.characterType;
            this.entity.characterName = entity.characterName;
            NameText.text = this.entity.id + ": " + this.entity.characterName;
            MaxHealth = entity.maxHealth;
            Health = entity.health;
            MaxMana = entity.maxMana;
            Mana = entity.mana;
            foreach (var skill in entity.skills)
            {
                this.entity.skills.Add(skill.Key, skill.Value);
            }
            foreach (var item in entity.inventory)
            {
                this.entity.inventory.Add(item.Key, item.Value);
            }
        }
    }
}