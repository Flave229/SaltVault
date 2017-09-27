using System;

namespace SaltVault.Core.Bills.Models
{
    public class AddPaymentRequest
    {
        public int BillId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Created { get; set; }
        public int PersonId { get; set; }
    }
}