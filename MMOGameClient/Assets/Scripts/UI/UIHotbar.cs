using Assets.Scripts.UI.UIItems;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.UI
{

    [CreateAssetMenu(menuName = "Hotbar", fileName = "New Hotbar")]
    public class UIHotbar : ScriptableObject
    {
        public List<UIContainer> items = new List<UIContainer>();
    }
}
