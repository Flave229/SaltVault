using System;
using System.Collections.Generic;
using HouseFinance.Core.FileManagement;

namespace HouseFinance.Core.Shopping
{
    public class ShoppingItem : IPersistedData
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool Purchased { get; set; }
        public DateTime Added { get; set; }
        public Guid AddedBy { get; set; }
        public List<Guid> ItemFor { get; set; }

        public ShoppingItem()
        {
            Id = Guid.NewGuid();
            Added = DateTime.Now;
            Purchased = false;
            ItemFor = new List<Guid>();
        }
    }

    public class ShoppingItemV2
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Purchased { get; set; }
        public DateTime Added { get; set; }
        public Guid AddedBy { get; set; }
        public List<int> ItemFor { get; set; }

        public ShoppingItemV2()
        {
            ItemFor = new List<int>();
        }
    }
}