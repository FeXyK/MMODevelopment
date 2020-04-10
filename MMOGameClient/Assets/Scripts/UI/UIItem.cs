using Assets.Scripts.SkillSystem.SkillSys;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.UI
{
    [CreateAssetMenu(fileName = "New Scrap", menuName = "Scrap")]
    [SerializeField]
    public class UIItem : ScriptableObject
    {
        [SerializeField]
        public int ID;
        [SerializeField]
        public string Name;
        [SerializeField]
        [TextArea(3,5)]
        public string Details;
        [SerializeField]
        public int Level;
        [SerializeField]
        public int Tier;
        [SerializeField]
        public UIItemType ItemType;
        [SerializeField]
        public int Value;
        [SerializeField]
        public int MaxAmount;
        [SerializeField]
        public UIItemRarity Rarity;
        public List<Effect> effects = new List<Effect>();
        public GameObject Prefab;
        [SerializeField]
        public Sprite Sprite;

        public Sprite GetSprite()
        {
            if (Sprite != null)
                return Sprite;
            else
                return null;
        }
        private void Awake()
        {
        }
        public virtual void CallOnAwake()
        {
            Name = this.name;
            Rarity = UIItemRarity.Common;
            ItemType = UIItemType.Scrap;
            Level = 0;
            Tier = 1;
            MaxAmount = 100;
        }
        public float GetCooldown()
        {
            foreach (var effect in effects)
            {
                if (effect.EffectType == EffectValue.Cooldown)
                {
                    return effect.Value * Mathf.Pow(effect.Multiplier, Level - 1);
                }
            }
            return 0;
        }
    }
}
