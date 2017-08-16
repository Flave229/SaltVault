using System.Collections.Generic;
using HouseFinance.Core.Bills;
using HouseFinance.Models.API;

namespace HouseFinance.Models.Bills
{
    public class GetBillListResponse : CommunicationResponse
    {
        public List<BillOverview> Bills { get; set; }
    }

    public class GetBillListResponseV2 : CommunicationResponse
    {
        public List<BillOverviewV2> Bills { get; set; }
    }
}