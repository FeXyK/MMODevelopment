using System.Collections.Generic;

namespace Utility_dotNET_Framework.Models
{
    public class Skill
    {
        const int damage = 10;
        const int heal = 20;
        const int attack_damage = 30;
        const int spell_damage = 40;
        const int magic_resist = 50;
        const int armor = 60;
        const int duration = 70;
        const int health = 80;
        const int mana = 90;
        const int move_speed = 100;
        const int move_jump = 110;
        const int cooldown_reduction = 120;
        const int cooldown = 130;
        const int attr_strength = 140;
        const int attr_intelligence = 150;
        const int attr_dexterity = 160;
        const int attr_constitution = 170;
        const int attr_knowledge = 180;
        const int attr_luck = 190;
        public int SkillType { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public int SkillID { get; set; }
        public float Cooldown { get; set; }
        public int ManaCost { get; set; }
        public float ManaCostMultiplier { get; set; }
        public int GoldCost { get; set; }
        public float GoldCostMultiplier { get; set; }
        public float Range { get; set; }
        public float RangeMultiplier { get; set; }
        public int RequiredSkillID { get; internal set; }

        public int RequiredLevel1;
        public int RequiredLevel2;
        public int RequiredLevel3;

        public Dictionary<int, Effect> Effects = new Dictionary<int, Effect>();
        public override string ToString()
        {
            string msg = "";
            msg += SkillID + " " + Name + " " + "\n";
            foreach (var effect in Effects)
            {
                msg += effect.Value.EffectID + ":  " + effect.Value.Value + "\n";
            }
            return msg;
        }

        public int Damage()
        {
            return Effects[damage].Value;
        }
        public int Heal()
        {
            return Effects[heal].Value;
        }
        public int AttackDamage()
        {
            return Effects[attack_damage].Value;
        }
        public int SpellDamage()
        {
            return Effects[spell_damage].Value;
        }
        public int MagicResist()
        {
            return Effects[magic_resist].Value;
        }
        public int Armor()
        {
            return Effects[armor].Value;
        }
        public int Duration()
        {
            return Effects[duration].Value;
        }
        public int Health()
        {
            return Effects[health].Value;
        }
        public int Mana()
        {
            return Effects[mana].Value;
        }
        public int MoveSpeed()
        {
            return Effects[move_speed].Value;
        }
        public int MoveJump()
        {
            return Effects[move_jump].Value;
        }
        public int CooldownReduction()
        {
            return Effects[cooldown_reduction].Value;
        }
        public int CooldownGet()
        {
            return Effects[cooldown].Value;
        }
        public int Strength()
        {
            return Effects[attr_strength].Value;
        }
        public int Intelligence()
        {
            return Effects[attr_intelligence].Value;
        }
        public int Dexterity()
        {
            return Effects[attr_dexterity].Value;
        }
        public int Constitution()
        {
            return Effects[attr_constitution].Value;
        }
        public int Knowledge()
        {
            return Effects[attr_knowledge].Value;
        }
        public int Luck()
        {
            return Effects[attr_luck].Value;
        }

    }
}
