using SaltVault.Core.Shopping;

namespace SaltVault.WebApp.Models.Shopping
{
    public class GetShoppingResponse : CommunicationResponse
    {
        public ShoppingListResponse Items { get; set; }
    }
}