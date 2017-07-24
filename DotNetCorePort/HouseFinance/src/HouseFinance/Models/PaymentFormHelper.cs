using System;
using HouseFinance.Core.Bills.Payments;

namespace HouseFinance.Models
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