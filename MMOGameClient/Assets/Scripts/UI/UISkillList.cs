using Assets.Scripts.UI.UIItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.UI
{
    [CreateAssetMenu(fileName = "Skill List", menuName = "New SkillList")]
    [Serializable]
    public class UISkillList : ScriptableObject
    {
        public List<SkillItem> items = new List<SkillItem>();
    }
}