using SaltVault.Core.Shopping.Models;

namespace SaltVault.WebApp.Models.Shopping
{
    public class GetShoppingResponse : CommunicationResponse
    {
        public ShoppingListResponse Items { get; set; }
    }
}