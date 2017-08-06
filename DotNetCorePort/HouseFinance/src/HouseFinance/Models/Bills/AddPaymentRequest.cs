using System;

namespace HouseFinance.Models.Bills
{
    public class AddPaymentRequest
    {
        public string BillId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Created { get; set; }
        public string PersonId { get; set; }
    }
}