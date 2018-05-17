using System;
using System.Collections.Generic;
using SaltVault.Core.People;

namespace SaltVault.Core.Shopping.Models
{
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime DateAdded { get; set; }
        public Person AddedBy { get; set; }
        public List<Person> AddedFor { get; set; }
        public bool Purchased { get; set; }

        public Item()
        {
            AddedFor = new List<Person>();
        }
    }
}