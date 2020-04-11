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
        private int mana;

        public int Mana
        {
            get { return mana; }
            set
            {
                mana = value;
            }
        }
        private int maxMana;

        public int MaxMana
        {
            get { return maxMana; }
            set
            {
                maxMana = value;
            }
        }
        private void Awake()
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
        public void Update()
        {
            tickRate = (1000f / 20f) * Time.deltaTime;
            if (this.gameObject.tag != "PlayerCharacter")
            {
                this.transform.position = Vector3.Lerp(this.transform.position, entity.position, tickRate);
                this.transform.LookAt(new Vector3(entity.position.x, entity.position.y, entity.position.y));
            }
            else
            {
                HealthBar.maxValue = entity.maxHealth;
                HealthBar.value = entity.health;
                ManaBar.maxValue = maxMana;
                ManaBar.value = mana;
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
            Debug.Log("MANAAAA" + Mana);
            Debug.Log("SETTING SKILLS FOR PLAYER111");
            foreach (var skill in entity.skills)
            {
                this.entity.skills.Add(skill.Key, skill.Value);
                Debug.Log("SETTING SKILLS FOR PLAYER");
            }
        }
    }
}