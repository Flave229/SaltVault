using System;
using System.Collections.Generic;
using SaltVault.Core.Bills;

namespace SaltVault.WebApp.Models.Bills
{
    public class UpdateBillRequest
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal? AmountOwed { get; set; }
        public DateTime? Due { get; set; }
        public List<Guid> People { get; set; }
        public RecurringType? RecurringType { get; set; }

        public UpdateBillRequest()
        {
            People = new List<Guid>();
        }
    }
}
