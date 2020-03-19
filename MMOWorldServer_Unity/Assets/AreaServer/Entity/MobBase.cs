using MMOGameServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.AreaServer.Entity
{
    class MobBase : Entity
    {
        MobBehaviour Behaviour;
        public int MobTypeID;
        public float MoveTimer = 5;
        public float MoveTimerDefault = 5;
        public float RespawnTimer = 5;
        public float RespawnTimerDefault = 5;
        public bool Alive = true;
        MobAreaSpawner spawner;
        //public MobBase(int mobID, int entityID, MobBehaviour mobBehaviour, MobAreaSpawner mobAreaSpawner)
        //{
        //    this.EntityID = entityID;
        //    this.MobTypeID = mobID;
        //    Behaviour = mobBehaviour;
        //    spawner = mobAreaSpawner;
        //}

        public override void Update()
        {
            if (Alive)
            {
                base.Update();
                MoveTimer -= Time.deltaTime;
                if (MoveTimer < 0)
                {
                    MoveTimer = MoveTimerDefault;
                    MoveHere(RandomVector(-3, 3));
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
                    RespawnTimer = RespawnTimerDefault;
                    Alive = true;
                    EntityHealth = EntityMaxHealth;
                    EntityMana = EntityMaxMana;
                }
            }
        }

        internal void Set(int mobID, int entityID, MobBehaviour mobBehaviour, MobAreaSpawner mobAreaSpawner)
        {
            this.EntityID = entityID;
            this.MobTypeID = mobID;
            Behaviour = mobBehaviour;
            spawner = mobAreaSpawner;
            EntityHealth = 100;
        }

        public void MoveHere(Vector3 target)
        {
            this.transform.position = target;
        }
        private Vector3 RandomVector(float min, float max)
        {
            var x = UnityEngine.Random.Range(this.transform.position.x + min, this.transform.position.x + max);
            var z = UnityEngine.Random.Range(this.transform.position.z + min, this.transform.position.z + max);
            return new Vector3(x, 1, z);
        }
        public override string ToString()
        {
            return "MobTypeID: " + MobTypeID + " Pos: " + this.transform.position.ToString();
        }
    }
}
