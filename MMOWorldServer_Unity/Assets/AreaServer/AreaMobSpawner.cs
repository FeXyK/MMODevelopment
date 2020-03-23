using Assets.AreaServer.Entity;
using System.Collections.Generic;
using UnityEngine;

namespace MMOGameServer
{
    public class MobAreaSpawner
    {
        public int MobID = 50;
        public int MaximumSpawnCount = 30;
        public float SpawnRadius = 30;
        public Dictionary<int,MobBase> SpawnedMobs = new Dictionary<int, MobBase>();
        public Vector3 position;
        public MobAreaSpawner(Vector3 position)
        {
            this.position = position;
            InitializeSpawner();
        }
        public void InitializeSpawner()
        {
            for (int i = 0; i < MaximumSpawnCount; i++)
            {
                MobBase mob;// = new MobBase(MobID, MobID + i, MobBehaviour.Passive, this);
                GameObject gameObject = GameObject.Instantiate(Resources.Load<GameObject>("Mob"));
                gameObject.transform.position = RandomVector(-SpawnRadius / 2, SpawnRadius / 2);

                mob = gameObject.AddComponent<MobBase>();
                mob.Set(MobID, MobID + i, MobBehaviour.Passive, this);
                mob.EntityName = "Mob_" + MobID + i;
                SpawnedMobs.Add(mob.EntityID,mob);
            }
        }
        private Vector3 RandomVector(float min, float max)
        {
            var x = Random.Range(position.x + min, position.x + max);
            var z = Random.Range(position.z + min, position.z + max);
            return new Vector3(x, position.y, z);
        }
        public override string ToString()
        {
            string mobinfo = "";
            foreach (var mob in SpawnedMobs)
            {
                mobinfo += mob.ToString() + "\n";
            }
            return mobinfo;
        }
    }
}