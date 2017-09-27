using System;

namespace SaltVault.Core.Bills.Models
{
    public class UpdatePaymentRequest
    {
        public int Id { get; set; }
        public decimal? Amount { get; set; }
        public DateTime? Created { get; set; }
    }
}