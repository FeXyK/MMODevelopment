
namespace Assets.AreaServer.Entity
{
  public class Character : Entity
    {
        public int AccountID;
        public CharacterApperance CharacterType;
        public Gear CharacterGear;

        public override void Update()
        {
            base.Update();
        }
    }
}
