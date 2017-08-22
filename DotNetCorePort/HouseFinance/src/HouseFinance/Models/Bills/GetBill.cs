using HouseFinance.Core.Bills;
using HouseFinance.Models.API;

namespace HouseFinance.Models.Bills
{
    public class GetBillRequest
    {
        public string BillId { get; set; }
    }

    public class GetBillRequestV2
    {
        public int BillId { get; set; }
    }

    public class GetBillResponse : CommunicationResponse
    {
        public BillDetailsResponse Bill { get; set; }
    }

    public class GetBillResponseV2 : CommunicationResponse
    {
        public BillDetails Bill { get; set; }
    }
}