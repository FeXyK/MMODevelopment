using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.AreaServer.Entity
{
    class Character : Entity
    {
        public int AccountID;
        public CharacterApperance CharacterType;
        public Gear CharacterGear;

        public override void Update()
        {
            base.Update();
        }
        internal bool SkillReady(int skillID)
        {
            return true;
        }
        //internal void ApplyDamage(int incomingDamage)
        //{
        //    EntityHealth -= incomingDamage;
        //}
    }
}
