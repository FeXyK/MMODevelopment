
using System.Collections.Generic;
using Utility_dotNET_Framework.Models;

namespace Assets.AreaServer.Entity
{
    public class ItemLibrary
    {
        public Dictionary<int, InventoryItem> Items = new Dictionary<int, InventoryItem>();

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
    }
}
