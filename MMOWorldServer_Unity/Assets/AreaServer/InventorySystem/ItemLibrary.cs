using System.Collections.Generic;
using Utility.Models;

namespace Assets.AreaServer.InventorySystem
{
    public class ItemLibrary
    {
        public Dictionary<int, InventoryItem> Items = new Dictionary<int, InventoryItem>();
        public List<InventoryItem> ItemsList = new List<InventoryItem>();
        System.Random r = new System.Random();

        private static ItemLibrary instance;
        public static ItemLibrary Instance
        {
            get
            {
                if (instance == null)
                    instance = new ItemLibrary();
                return instance;
            }
        }

        internal InventoryItem GetRandomItem()
        {
            return ItemsList[r.Next(0, ItemsList.Count-1)];
        }
    }
}
