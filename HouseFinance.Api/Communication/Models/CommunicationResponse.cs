using System.Collections.Generic;

namespace HouseFinance.Api.Communication.Models
{
    public class CommunicationResponse
    {
        public Error Error { get; set; }
        public bool HasError { get; set; }
        public List<string> Notifications { get; set; }
    }
}