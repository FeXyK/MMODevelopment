using Assets.Scripts.SkillSystem.SkillSys;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.UI
{
    [CreateAssetMenu(fileName = "New Scrap", menuName = "Scrap")]
    public class UIItem : ScriptableObject
    {
        [SerializeField]
        public int  ID;
        [SerializeField]
        public string Name;
        [SerializeField]
        public int Level;
        [SerializeField]
        public int Tier;
        [SerializeField]
        public UIItemType ItemType;
        [SerializeField]
        public int Value;
        [SerializeField]
        public UIItemRarity Rarity;
        public List<Effect> effects = new List<Effect>();
        public GameObject Prefab;
        [SerializeField]
        public Sprite Sprite;

        public Sprite GetSprite()
        {
            return Resources.Load<Sprite>(Name);
        }
        private void Awake()
        {
            CallOnAwake();
        }
        public virtual void CallOnAwake()
        {
            Name = this.name;
            Rarity = UIItemRarity.Common;
            ItemType = UIItemType.Scrap;
            Level = 0;
            Tier = 1;
        }
    }
}
