using UnityEngine;

namespace Assets.AreaServer.InventorySystem
{
    public class DropItem
    {
        public SlotItem Item;
        public Vector3 Position;

        public DropItem(Vector3 position, SlotItem item)
        {
            this.Position = position;
            this.Item = item;
        }
    }
}
