
using System;
using System.Collections.Generic;
using UnityEngine;
using Utility_dotNET_Framework.Models;

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
                    EntityMana += 20;
                }
                RegenTimer = 1;
            }
            RegenTimer -= Time.deltaTime;
        }

    }
}
