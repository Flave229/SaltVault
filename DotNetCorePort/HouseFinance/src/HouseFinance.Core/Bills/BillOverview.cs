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

    public class BillDetails
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime FullDateDue { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal AmountPaid { get; set; }
        public List<PersonBillDetailsV2> People { get; set; }
        public List<BillPayment> Payments { get; set; }
        public RecurringType RecurringType { get; set; }

        public BillDetails()
        {
            People = new List<PersonBillDetailsV2>();
            Payments = new List<BillPayment>();
        }
    }

    public class PersonBillDetails
    {
        public string ImageLink { get; set; }
        public bool Paid { get; set; }
    }

    public class PersonBillDetailsV2
    {
        public int Id { get; set; }
        public string ImageLink { get; set; }
        public bool Paid { get; set; }
    }
}
