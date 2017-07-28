using HouseFinance.Core.Bills;

namespace HouseFinance.Models.API
{
    public class GetBillResponse : CommunicationResponse
    {
        public BillDetailsResponse Bill { get; set; }
    }
}