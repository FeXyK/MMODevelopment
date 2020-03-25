
using System;

namespace Assets.AreaServer.Entity
{
  public class Character : Entity
    {
        public int AccountID;
        public CharacterApperance CharacterType;
        public Gear CharacterGear;

        public int EntityExp { get; internal set; }

        public  void Update()
        {
            //base.Update();
        }

    }
}
