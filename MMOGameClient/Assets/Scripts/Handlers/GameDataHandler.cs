using Assets.Scripts.Character;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Handlers
{
  public  class GameDataHandler
    {
        public Dictionary<int, EntityContainer> otherCharacters = new Dictionary<int, EntityContainer>();
        public EntityContainer myCharacter;

        public GameDataHandler()
        {
            //myCharacter = GameObject.FindObjectOfType<EntityContainer>();

            //myCharacter.Set(LoginDataHandler.GetInstance().selectedCharacter);

            //myCharacter.Health = myCharacter.MaxHealth;
        }
        public EntityContainer GetEntity(int entityID)
        {
            if (entityID == myCharacter.entity.id)
                return myCharacter;
            else
                return otherCharacters[entityID];

        }
    }
}
