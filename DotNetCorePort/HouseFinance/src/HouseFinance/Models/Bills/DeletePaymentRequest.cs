using System;

namespace HouseFinance.Models.Bills
{
    public class DeletePaymentRequest
    {
        public Guid BillId { get; set; }
        public Guid PaymentId { get; set; }
    }
}