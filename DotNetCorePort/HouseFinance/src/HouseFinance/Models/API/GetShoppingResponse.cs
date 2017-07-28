using HouseFinance.Core.Shopping;

namespace HouseFinance.Models.API
{
    public class GetShoppingResponse : CommunicationResponse
    {
        public ShoppingListResponse Items { get; set; }
    }
}