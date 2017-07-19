using System;
using System.Collections.Generic;

namespace HouseFinance.Core.Bills
{
    public class BillOverview
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime FullDateDue { get; set; }
        public string DateDue { get; set; }
        public bool Overdue { get; set; }
        public bool Paid { get; set; }
        public decimal AmountDue { get; set; }
        public List<PersonBillDetails> People { get; set; }
        public decimal TotalAmount { get; set; }

        public BillOverview()
        {
            Name = "";
            FullDateDue = new DateTime();
            DateDue = new DateTime().ToString("yyyy-MM-dd");
            AmountDue = 0;
            People = new List<PersonBillDetails>();
        }
    }

    public class PersonBillDetails
    {
        public string ImageLink { get; set; }
        public bool Paid { get; set; }
    }
}
