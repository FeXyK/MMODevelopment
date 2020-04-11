
using System;
using UnityEngine;

namespace Assets.AreaServer.Entity
{
    public class Character : Entity
    {
        public int AccountID;
        public CharacterApperance CharacterType;
        public Gear CharacterGear;

        public int EntityExp { get; internal set; }

        float RegenTimer = 1;
        private void Update()
        {
            if (RegenTimer <= 0)
            {
                if (EntityMana < EntityMaxMana)
                {
                    EntityMana += 15;
                    if (EntityMana > EntityMaxMana)
                        EntityMana = EntityMaxMana;
                }
                RegenTimer = 1;
            }
            RegenTimer -= Time.deltaTime;
        }

    }
}
