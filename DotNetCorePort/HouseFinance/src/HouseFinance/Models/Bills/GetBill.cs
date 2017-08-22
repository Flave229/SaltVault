using HouseFinance.Core.Bills;
using HouseFinance.Models.API;

namespace HouseFinance.Models.Bills
{
    public class GetBillRequest
    {
        public int BillId { get; set; }
    }

    public class GetBillResponse : CommunicationResponse
    {
        public Bill Bill { get; set; }
    }
}