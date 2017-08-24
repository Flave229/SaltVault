using System;

namespace SaltVault.WebApp.Models.Bills
{
    public class DeletePaymentRequest
    {
        public Guid BillId { get; set; }
        public Guid PaymentId { get; set; }
    }

    public class DeletePaymentRequestV2
    {
        public int PaymentId { get; set; }
    }
}