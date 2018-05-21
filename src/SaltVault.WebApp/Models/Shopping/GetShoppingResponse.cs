using System.Collections.Generic;
using SaltVault.Core.Shopping.Models;

namespace SaltVault.WebApp.Models.Shopping
{
    public class GetShoppingResponse : CommunicationResponse
    {
        public List<Item> ShoppingList { get; set; }

        public GetShoppingResponse()
        {
            ShoppingList = new List<Item>();
        }
    }
}