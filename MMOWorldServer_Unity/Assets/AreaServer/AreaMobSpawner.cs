using Assets.AreaServer.Entity;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MMOGameServer
{
    public class MobAreaSpawner
    {
        public int MobID = 50;
        public int MaximumSpawnCount = 30;
        public float SpawnRadius = 30;
        public Dictionary<int, MobBase> SpawnedMobs = new Dictionary<int, MobBase>();
        public Vector3 position;

        public MobAreaSpawner(Vector3 position)
        {
            this.position = position;
            InitializeSpawner();
        }
        public void Event_MobDied(int mobID)
        {
            SpawnedMobs.Remove(mobID);
        }
        public void InitializeSpawner()
        {
            for (int i = 0; i < MaximumSpawnCount; i++)
            {
                SpawnMob(i);
            }
        }
        public void SpawnMob(int i)
        {
            MobBase mob;// = new MobBase(MobID, MobID + i, MobBehaviour.Passive, this);
            GameObject gameObject = GameObject.Instantiate(Resources.Load<GameObject>("Mob"));
            gameObject.transform.position = GetRandomPosition();

            mob = gameObject.AddComponent<MobBase>();
            int maxHealth = 1000;
            mob.Set(MobID, MobID + i, maxHealth, EMobBehaviour.Passive);
            mob.GenerateInventory();
            mob.EntityName = "Mob_" + MobID + i;
            mob.Spawner = this;
            SpawnedMobs.Add(mob.EntityID, mob);
        }
        public Vector3 GetRandomPosition()
        {
            Vector3 pos = UnityEngine.Random.insideUnitSphere * SpawnRadius;
            pos.y = 0;
            return position + pos;
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