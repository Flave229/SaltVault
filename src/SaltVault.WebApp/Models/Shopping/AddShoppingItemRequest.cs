using System.Collections.Generic;

namespace SaltVault.WebApp.Models.Shopping
{
    public class AddShoppingItemRequest
    {
        public string Name { get; set; }
        public List<int> ItemFor { get; set; }

        public AddShoppingItemRequest()
        {
            ItemFor = new List<int>();
        }
    }
}
