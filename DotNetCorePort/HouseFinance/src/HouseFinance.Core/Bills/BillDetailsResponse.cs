using System;
using System.Collections.Generic;

namespace HouseFinance.Core.Bills
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

    public class BillDetailsResponseV2
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime FullDateDue { get; set; }
        public double TotalAmount { get; set; }
        public double AmountPaid { get; set; }
        public List<BillPaymentsV2> Payments { get; set; }

        public BillDetailsResponseV2()
        {
            Payments = new List<BillPaymentsV2>();
        }
    }

    public class BillPayments
    {
        public Guid Id { get; set; }
        public string PersonName { get; set; }
        public string DatePaid { get; set; }
        public decimal AmountPaid { get; set; }
    }

    public class BillPaymentsV2
    {
        public int Id { get; set; }
        public string PersonName { get; set; }
        public DateTime DatePaid { get; set; }
        public double Amount { get; set; }
    }
}