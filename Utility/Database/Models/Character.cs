using System;
using System.Collections.Generic;

namespace Utility.Models
{
    public partial class Character
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public string Name { get; set; }
        public int? Health { get; set; }
        public int? Mana { get; set; }
        public int? Exp { get; set; }
        public int? Level { get; set; }
        public double? PosX { get; set; }
        public double? PosY { get; set; }
        public double? PosZ { get; set; }
        public double? Rotation { get; set; }
        public int? CharSkills { get; set; }
        public int? CharType { get; set; }
        public int? Gold { get; set; }
        public override string ToString()
        {
            return Id + " " + AccountId + " " + Name;
        }
    }
}
