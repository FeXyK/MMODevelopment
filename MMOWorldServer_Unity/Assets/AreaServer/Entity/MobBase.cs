using Assets.AreaServer.InventorySystem;
using Assets.Scripts.Handlers;
using MMOGameServer;
using UnityEngine;
using Utility.Models;

namespace Assets.AreaServer.Entity
{
    public class MobBase : Entity
    {
        EMobBehaviour Behaviour;
        public int MobTypeID;
        public float MoveTimer = 5;
        public float MoveTimerDefault = 5;
        public float RespawnTimer = 30;
        public float RespawnTimerDefault = 30;
        public bool Alive = true;
        Vector3 Target;
        public float MoveForce = 500;
        Rigidbody rigi;
        public MobAreaSpawner Spawner;

        private void Start()
        {
            rigi = this.gameObject.GetComponent<Rigidbody>();
            MoveTimerDefault = Random.Range(2, 10);
        }
        public void Update()
        {
            if (Alive)
            {
                MoveTimer -= Time.deltaTime;
                if (MoveTimer < 0)
                {
                    MoveTimer = MoveTimerDefault;
                    Target = MoveHere(RandomVector(-3, 3));
                    rigi.AddForce(RandomVector(-MoveForce, MoveForce));
                }
                if (EntityHealth <= 0)
                {
                    Alive = false;
                }
            }
            else
            {
                RespawnTimer -= Time.deltaTime;
                if (RespawnTimer < 0)
                {
                    OnRespawn();
                }
            }
        }

        internal void GenerateInventory()
        {
            Inventory.Clear();
            EntityGold = Random.Range(50 * EntityLevel, 100 * EntityLevel);
            InventoryItem item = null;
            for (int i = 0; i < 2; i++)
            {
                item = ItemLibrary.Instance.GetRandomItem();
                Debug.LogWarning(item.Name);
                if (!Inventory.ContainsKey(item.ID))
                {
                    SlotItem newItem = new SlotItem(item);
                    if (newItem.ItemType == EItemType.Potion || newItem.ItemType == EItemType.Food)
                        newItem.Amount = Random.Range(1, 5);
                    else
                        newItem.Amount = 1;
                    Inventory.Add(item.ID, newItem);
                }
            }
        }
        internal void Set(int mobID, int entityID, int maxHealth, EMobBehaviour mobBehaviour)
        {
            this.EntityID = mobID * 100 + entityID;
            this.MobTypeID = mobID;
            Behaviour = mobBehaviour;
            EntityMaxHealth = maxHealth;
            EntityHealth = EntityMaxHealth;
            EntityLevel = 1;
        }
        public Vector3 MoveHere(Vector3 target)
        {
            return target;
        }
        private Vector3 RandomVector(float min, float max)
        {
            var x = Random.Range(min, max);
            var z = Random.Range(min, max);
            return new Vector3(x, 0, z);
        }
        public override string ToString()
        {
            return "MobTypeID: " + MobTypeID + " Pos: " + this.transform.position.ToString();
        }
        public override void OnDie(Entity source)
        {
            AreaMessageSender.Instance.DropLootTo(source, this);
        }
        public override void OnRespawn()
        {
            this.transform.position = Spawner.GetRandomPosition();
            RespawnTimer = RespawnTimerDefault;
            EntityHealth = EntityMaxHealth;
            EntityMana = EntityMaxMana;
            GenerateInventory();
            Alive = true;
        }
    }
}
