using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.SkillSystem.SkillSys
{
    class SkillBarContainer : MonoBehaviour
    {
        private Skill skill;

        public Image Art;
        public Image CooldownArt;
        public Image LevelArt;
        public TMP_Text Amount;
        public TMP_Text Cooldown;
        public TMP_Text Level;
        public TMP_Text Name;
        public TMP_Text Value;
        public TMP_Text ManaCost;
        public TMP_Text Hotkey;

        public Skill Skill
        {
            get => skill; set
            {
                skill = value;
                RefreshUI();
            }
        }

        private void RefreshUI()
        {

        }
    }
}