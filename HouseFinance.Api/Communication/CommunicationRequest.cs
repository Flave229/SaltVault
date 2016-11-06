using System;

namespace HouseFinance.Api.Communication
{
    public interface ICommunicationRequest
    {
        Guid AuthToken { get; set; }
        string RequestType { get; set; }
        string Id { get; set; }
    }

    public class CommunicationRequest : ICommunicationRequest
    {
        public Guid AuthToken { get; set; }
        public string RequestType { get; set; }
        public string Id { get; set; }
    }
}
