using System;

namespace HouseFinance.Core.Bills
{
    public class BillPayment
    {
        public int Id { get; set; }
        public DateTime DatePaid { get; set; }
        public decimal Amount { get; set; }
        public int PersonId { get; set; }
        public string PersonName { get; set; }
    }
}