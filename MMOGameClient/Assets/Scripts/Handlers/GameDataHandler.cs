using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Handlers
{
  public  class GameDataHandler
    {
        public Dictionary<int, Entity> otherCharacters = new Dictionary<int, Entity>();
        public Entity myCharacter;
        public GameDataHandler()
        {
            myCharacter = GameObject.FindObjectOfType<Entity>();
            myCharacter.Set(LoginDataHandler.GetInstance().selectedCharacter);
            myCharacter.Health = myCharacter.MaxHealth;
        }
        public Entity GetEntity(int entityID)
        {
            if (entityID == myCharacter.id)
                return myCharacter;
            else
                return otherCharacters[entityID];

        }
    }
}
