using System;
using Services.Models.FinanceModels;

namespace Services.Models.Helpers
{
    public class PaymentFormHelper
    {
        public Guid BillId { get; set; }
        public Payment Payment { get; set; }

        public PaymentFormHelper()
        {
            BillId = new Guid();
        }
    }
}
