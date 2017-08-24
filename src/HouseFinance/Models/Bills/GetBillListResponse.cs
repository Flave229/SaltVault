using System.Collections.Generic;
using SaltVault.Core.Bills;

namespace SaltVault.WebApp.Models.Bills
{
    public class GetBillListResponse : CommunicationResponse
    {
        public List<Bill> Bills { get; set; }
    }
}