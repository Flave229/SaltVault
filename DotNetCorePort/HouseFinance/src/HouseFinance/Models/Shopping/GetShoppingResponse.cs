using HouseFinance.Core.Shopping;
using HouseFinance.Models.API;

namespace HouseFinance.Models.Shopping
{
    public class GetShoppingResponse : CommunicationResponse
    {
        public ShoppingListResponse Items { get; set; }
    }

    public class GetShoppingResponseV2 : CommunicationResponse
    {
        public ShoppingListResponseV2 Items { get; set; }
    }
}