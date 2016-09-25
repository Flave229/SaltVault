using System;
using System.Collections.Generic;

namespace HouseFinance.Api.Models
{
    public class ShoppingListResponse
    {
        public List<Item> ShoppingList { get; set; }

        public ShoppingListResponse()
        {
            ShoppingList = new List<Item>();
        }
    }

    public class Item
    {
        public string Name { get; set; }
        public DateTime AddedOn { get; set; }
        public string AddedByImage { get; set; }
        public List<string> AddedForImages { get; set; }
        public bool Purchased { get; set; }

        public Item()
        {
            Name = "";
            AddedOn = new DateTime();
            AddedByImage = "";
            AddedForImages = new List<string>();
        }
    }
}
