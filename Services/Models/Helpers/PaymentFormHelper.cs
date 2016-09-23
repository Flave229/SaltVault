using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services.FileIO;
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
