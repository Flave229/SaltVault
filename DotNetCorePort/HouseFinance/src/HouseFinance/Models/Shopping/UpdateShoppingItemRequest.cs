using System;
using System.Collections.Generic;

namespace HouseFinance.Models.Shopping
{
    public class UpdateShoppingItemRequest
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool Purchased { get; set; }
        public List<Guid> ItemFor { get; set; }
    }
}