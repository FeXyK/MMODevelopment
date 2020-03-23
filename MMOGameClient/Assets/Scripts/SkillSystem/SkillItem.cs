using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.SkillSystem
{
    [CreateAssetMenu(fileName = "SkillItem", menuName = "Scriptable Objects/New Skill Item")]
    public class SkillItem : ScriptableObject
    {
        [SerializeField] int skillID;
        public int SkillID { get => skillID; }
        [SerializeField] float cooldownTime;
        public float CooldownTime { get => cooldownTime; }
        [SerializeField] float cooldownTimeDefault;
        public float CooldownTimeDefault { get => cooldownTimeDefault; }
        [SerializeField] int cost;
        public int Cost { get => cost; }
        [SerializeField] Sprite artSprite;
        public Sprite ArtSprite { get => artSprite; }

        internal void SetSprite(Sprite art)
        {
            artSprite = art;
        }
        internal void Set(SkillItem skill)
        {
            skillID = skill.SkillID;
            cooldownTime = skill.CooldownTime;
            cooldownTimeDefault = skill.CooldownTimeDefault;
            artSprite = skill.ArtSprite;
        }
    }
}