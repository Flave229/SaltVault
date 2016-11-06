using HouseFinance.Api.Communication;
using System;
using System.Web.Mvc;

namespace HouseFinance.Controllers
{
    public class APIController : Controller
    {
        [Route("Api/{authToken}/{requestType}/{id?}")]
        public string ApiRequest(string authToken, string requestType, string id = "")
        {
            return Communication.Call(new CommunicationRequest
            {
                AuthToken = new Guid(authToken),
                RequestType = requestType,
                Id = id
            });
        }
    }
}