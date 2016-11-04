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
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime FullAddedOn { get; set; }
        public string AddedOn { get; set; }
        public string AddedByImage { get; set; }
        public List<string> AddedForImages { get; set; }
        public bool Purchased { get; set; }

        public Item()
        {
            Name = "";
            FullAddedOn = new DateTime();
            AddedOn = new DateTime().ToString("yyyy-MM-dd");
            AddedByImage = "";
            AddedForImages = new List<string>();
        }
    }
}
