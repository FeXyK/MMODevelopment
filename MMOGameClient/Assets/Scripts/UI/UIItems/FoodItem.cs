using UnityEngine;

namespace Assets.Scripts.UI.UIItems
{
    [CreateAssetMenu(fileName = "New Food", menuName = "Food")]
    class FoodItem : UIItem
    {
        public override void CallOnAwake()
        {
            base.CallOnAwake();
            ID = 3001;
            ItemType = EItemType.Food;
        }
    }
}
