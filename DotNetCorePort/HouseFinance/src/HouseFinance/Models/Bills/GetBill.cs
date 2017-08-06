using HouseFinance.Core.Bills;
using HouseFinance.Models.API;

namespace HouseFinance.Models.Bills
{
    public class GetBillRequest
    {
        public string BillId { get; set; }
    }

    public class GetBillResponse : CommunicationResponse
    {
        public BillDetailsResponse Bill { get; set; }
    }
}