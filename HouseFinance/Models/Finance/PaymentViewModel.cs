using Services.Models.FinanceModels;

namespace HouseFinance.Models.Finance
{
    public class PaymentViewModel
    {
        public Bill Bill { get; set; }
        public Payment Payment { get; set; }
    }
}