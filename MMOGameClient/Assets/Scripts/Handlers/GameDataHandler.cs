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
        public Dictionary<int, Character> otherCharacters = new Dictionary<int, Character>();
        public Character myCharacter;
        public GameDataHandler()
        {
            myCharacter = GameObject.FindObjectOfType<Character>();
            myCharacter.Set(LoginDataHandler.GetInstance().selectedCharacter);
        }
    }
}
