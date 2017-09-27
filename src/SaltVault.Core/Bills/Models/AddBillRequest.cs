using System;
using System.Collections.Generic;

namespace SaltVault.Core.Bills.Models
{
    public class AddBillRequest
    {
        public string Name { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime Due { get; set; }
        public List<int> PeopleIds { get; set; }
        public RecurringType RecurringType { get; set; }

        public AddBillRequest()
        {
            PeopleIds = new List<int>();
        }
    }
}