using System;
using System.Collections.Generic;
using SaltVault.Core.Bills;

namespace SaltVault.WebApp.Models.Bills
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

    public class AddBillResponse : CommunicationResponse
    {
        public int Id { get; set; }
    }
}