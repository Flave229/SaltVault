using System;
using System.Collections.Generic;

namespace SaltVault.Core.Bills
{
    public enum RecurringType
    {
        None,
        Monthly
    }

    public class Bill
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime FullDateDue { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal AmountPaid { get; set; }
        public List<BillPersonDetails> People { get; set; }
        public List<Payment> Payments { get; set; }
        public RecurringType RecurringType { get; set; }

        public Bill()
        {
            People = new List<BillPersonDetails>();
            Payments = new List<Payment>();
        }
    }

    public class BillPersonDetails
    {
        public int Id { get; set; }
        public string ImageLink { get; set; }
        public bool Paid { get; set; }
    }
}