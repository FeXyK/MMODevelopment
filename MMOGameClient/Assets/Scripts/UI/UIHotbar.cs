﻿using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.UI
{

    [CreateAssetMenu(menuName = "Hotbar", fileName = "New Hotbar")]
    class UIHotbar : ScriptableObject
    {
        public List<KeyValuePair<int, UIItem>> items = new List<KeyValuePair<int, UIItem>>();
        public void Modify(int key, UIItem item)
        {
            items[key] = new KeyValuePair<int, UIItem>(key, item);
        }
    }
}
