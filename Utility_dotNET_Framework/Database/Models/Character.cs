using System;
using System.Collections.Generic;

namespace Utility_dotNET_Framework.Models
{
    public partial class Character
    {
        public int CharacterID { get; set; }
        public int AccountID { get; set; }
        public string Name { get; set; }
        public int? Health { get; set; }
        public int? Mana { get; set; }
        public int? MaxHealth { get; set; }
        public int? MaxMana { get; set; }
        public int? Exp { get; set; }
        public int? Level { get; set; }
        public float? PosX { get; set; }
        public float? PosY { get; set; }
        public float? PosZ { get; set; }
        public int? CharSkills { get; set; }
        public int? CharType { get; set; }
        public int? Gold { get; set; }
        public Dictionary<int, int> Skills = new Dictionary<int, int>();
        public List<KeyValuePair<int, CharacterItem>> Inventory = new List<KeyValuePair<int, CharacterItem>>();
        public override string ToString()
        {
            return CharacterID + " " + AccountID + " " + Name;
        }
    }
}
