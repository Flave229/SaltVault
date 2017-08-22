using System;
using System.Collections.Generic;

namespace HouseFinance.Core.Shopping
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
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime DateAdded { get; set; }
        public string AddedByImage { get; set; }
        public List<string> AddedForImages { get; set; }
        public bool Purchased { get; set; }

        public Item()
        {
            AddedForImages = new List<string>();
        }
    }
}