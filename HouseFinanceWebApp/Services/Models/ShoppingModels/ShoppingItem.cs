using System;
using System.Collections.Generic;

namespace Services.Models.ShoppingModels
{
    public class ShoppingItem
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
}
