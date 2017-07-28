using System.Collections.Generic;
using HouseFinance.Core.Bills;

namespace HouseFinance.Models.API
{
    public class GetBillListResponse : CommunicationResponse
    {
        public List<BillOverview> Bills { get; set; }
    }
}