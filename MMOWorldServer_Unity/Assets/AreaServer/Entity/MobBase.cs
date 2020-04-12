using UnityEngine;

namespace Assets.AreaServer.Entity
{
    public class MobBase : Entity
    {
        MobBehaviour Behaviour;
        public int MobTypeID;
        public float MoveTimer = 5;
        public float MoveTimerDefault = 5;
        public float RespawnTimer = 5;
        public float RespawnTimerDefault = 5;
        public bool Alive = true;
        Vector3 Target;
        public float MoveForce = 500;
        Rigidbody rigi;
        //public MobBase(int mobID, int entityID, MobBehaviour mobBehaviour, MobAreaSpawner mobAreaSpawner)
        //{
        //    this.EntityID = entityID;
        //    this.MobTypeID = mobID;
        //    Behaviour = mobBehaviour;
        //    spawner = mobAreaSpawner;
        //}
        private void Start()
        {
            rigi = this.gameObject.GetComponent<Rigidbody>();
            MoveTimerDefault = UnityEngine.Random.Range(2, 10);
        }
        public /*override*/ void Update()
        {
            if (Alive)
            {
                //base.Update();
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
                //this.transform.position = Vector3.Lerp(this.transform.position, Target,  Time.deltaTime);
            }
            else
            {
                RespawnTimer -= Time.deltaTime;
                if (RespawnTimer < 0)
                {
                    RespawnTimer = RespawnTimerDefault;
                    Alive = true;
                    EntityHealth = EntityMaxHealth;
                    EntityMana = EntityMaxMana;
                }
            }
        }

        internal void Set(int mobID, int entityID, int maxHealth, MobBehaviour mobBehaviour)
        {
            //this.EntityID = entityID;
            this.EntityID = mobID * 100 + entityID;
            this.MobTypeID = mobID;
            Behaviour = mobBehaviour;
            EntityMaxHealth = maxHealth;
            EntityHealth = EntityMaxHealth;
        }

        public Vector3 MoveHere(Vector3 target)
        {
            return target;
        }
        private Vector3 RandomVector(float min, float max)
        {
            var x = UnityEngine.Random.Range(min, max);
            var z = UnityEngine.Random.Range(min, max);
            return new Vector3(x, 0, z);
        }
        public override string ToString()
        {
            return "MobTypeID: " + MobTypeID + " Pos: " + this.transform.position.ToString();
        }
    }
}
