using SaltVault.Core.Bills;

namespace SaltVault.WebApp.Models.Bills
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