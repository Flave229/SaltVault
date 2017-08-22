using System;
using System.Collections.Generic;

namespace HouseFinance.Core.Shopping
{
    public class ShoppingItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Purchased { get; set; }
        public DateTime Added { get; set; }
        public int AddedBy { get; set; }
        public List<int> ItemFor { get; set; }

        public ShoppingItem()
        {
            ItemFor = new List<int>();
        }
    }
}