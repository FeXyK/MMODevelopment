using System.Collections.Generic;

namespace Utility.Models
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
        public Dictionary<int, CharacterItem> Inventory = new Dictionary<int, CharacterItem>();
        public Dictionary<int, CharacterItem> Equipped = new Dictionary<int, CharacterItem>();
        public Dictionary<int, CharacterItem> Storage = new Dictionary<int, CharacterItem>();
        public Dictionary<int, CharacterItem> AuctionHouse = new Dictionary<int, CharacterItem>();
        public override string ToString()
        {
            return CharacterID + " " + AccountID + " " + Name;
        }
    }
}
