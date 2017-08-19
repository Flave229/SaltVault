using System;

namespace HouseFinance.Models.Bills
{
    public class DeleteBillRequest
    {
        public Guid BillId { get; set; }
    }

    public class DeleteBillRequestV2
    {
        public int BillId { get; set; }
    }
}