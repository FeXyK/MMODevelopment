using Assets.Scripts.LoginScreen;
using MMOLoginServer.ServerData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            myCharacter.Health = 100;
        }
    }
}
