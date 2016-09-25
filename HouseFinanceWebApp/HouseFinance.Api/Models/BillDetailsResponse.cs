using System;
using System.Collections.Generic;

namespace HouseFinance.Api.Models
{
    public class BillDetailsResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime FullDateDue { get; set; }
        public string DateDue { get; set; }
        public decimal AmountDue { get; set; }
        public decimal AmountPaid { get; set; }
        public List<BillPayments> Payments { get; set; }

        public BillDetailsResponse()
        {
            Id = Guid.NewGuid();
            Name = "";
            FullDateDue = new DateTime();
            DateDue = new DateTime().ToString("yyyy-MM-dd");
            AmountDue = 0;
            Payments = new List<BillPayments>();
        }
    }

    public class BillPayments
    {
        public string PersonName { get; set; }
        public string DatePaid { get; set; }
        public decimal AmountPaid { get; set; }
    }
}
