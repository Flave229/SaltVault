using System;

namespace HouseFinance.Models.Shopping
{
    public class DeleteShoppingItemRequest
    {
        public Guid ShoppingItemId { get; set; }
    }

    public class DeleteShoppingItemRequestV2
    {
        public int ShoppingItemId { get; set; }
    }
}