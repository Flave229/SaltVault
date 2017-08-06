using System;

namespace HouseFinance.Models.Bills
{
    public class AddPaymentRequest
    {
        public Guid BillId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Created { get; set; }
        public Guid PersonId { get; set; }
    }
}