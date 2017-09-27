using System.Collections.Generic;

namespace SaltVault.Core.Shopping.Models
{
    public class UpdateShoppingItemRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool? Purchased { get; set; }
        public List<int> ItemFor { get; set; }
    }
}