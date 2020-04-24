using Assets.AreaServer.InventorySystem;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.AreaServer.Entity
{
    public class Character : Entity
    {
        public int AccountID;
        public ECharacterApperance CharacterType;

        public int EntityExp { get; internal set; }

        float RegenTimer = 1;

        public Dictionary<int, SlotItem> Storage = new Dictionary<int, SlotItem>();
        private void Update()
        {
            if (RegenTimer <= 0)
            {
                if (EntityMana < EntityMaxMana)
                {
                    EntityMana += 4;
                }
                RegenTimer = 1;
            }
            RegenTimer -= Time.deltaTime;
        }
    }
}
