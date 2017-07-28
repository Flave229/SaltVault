using System;
using System.Collections.Generic;
using System.IO;
using HouseFinance.Core.Authentication;
using HouseFinance.Core.Bills;
using Microsoft.AspNetCore.Mvc;

namespace HouseFinance.Controllers
{
    public class ApiController : Controller
    {
        [Route("Api/Bills")]
        public GetBillsResponse GetBills(CommunicationRequest billsRequest)
        {
            var response = new GetBillsResponse();
            try
            {
                if (Authentication.CheckKey(billsRequest.ApiKey) == false)
                {
                    response.AddError("The API Key was invalid");
                    return response;
                }

                response.Bills = BillListBuilder.BuildBillList();
                return response;
            }
            catch (Exception exception)
            {
                response.AddError($"An unexpected exception occured: {exception}");
                return response;
            }
        }
    }

    public class GetBillsResponse : CommunicationResponse
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

    public class CommunicationRequest
    {
        public Guid ApiKey { get; set; }
    }
}