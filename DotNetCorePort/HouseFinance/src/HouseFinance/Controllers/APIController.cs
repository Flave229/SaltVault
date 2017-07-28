using System;
using System.Collections.Generic;
using HouseFinance.Core.Authentication;
using HouseFinance.Core.Bills;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace HouseFinance.Controllers
{
    public class ApiController : Controller
    {
        [HttpGet]
        [Route("Api/Bills")]
        public GetBillListResponse GetBillList()
        {
            var response = new GetBillListResponse();
            if (Authenticate(Request.Headers["Authorization"]) == false)
            {
                response.AddError("The API Key was invalid");
                return response;
            }

            try
            {
                response.Bills = BillListBuilder.BuildBillList();
            }
            catch (Exception exception)
            {
                response.AddError($"An unexpected exception occured: {exception}");
            }

            return response;
        }

        [HttpPost]
        [Route("Api/Bills")]
        public GetBillResponse GetBill([FromBody]GetBillRequest billRequest)
        {
            var response = new GetBillResponse();
            if (Authenticate(Request.Headers["Authorization"]) == false)
            {
                response.AddError("The API Key was invalid");
                return response;
            }

            try
            {
                response.Bill = BillDetailsBuilder.BuildBillDetails(Guid.Parse(billRequest.BillId));
            }
            catch (Exception exception)
            {
                response.AddError($"An unexpected exception occured: {exception}");
            }

            return response;
        }

        private bool Authenticate(StringValues authorizationHeader)
        {
            var apiKey = authorizationHeader.ToString().Replace("Token ", "");
            return Authentication.CheckKey(apiKey);
        }
    }

    public class GetBillResponse : CommunicationResponse
    {
        public BillDetailsResponse Bill { get; set; }
    }

    public class GetBillRequest
    {
        public string BillId { get; set; }
    }

    public class GetBillListResponse : CommunicationResponse
    {
        public List<BillOverview> Bills { get; set; }
    }

    public class CommunicationResponse
    {
        public Error Error { get; set; }
        public bool HasError { get; set; }
        public List<string> Notifications { get; set; }

        public void AddError(string message)
        {
            Error = new Error { Message = message };
            HasError = true;
        }
    }

    public class Error
    {
        public string Message { get; set; }
    }
}