using System;
using System.Collections.Generic;
using HouseFinance.Core.Authentication;
using HouseFinance.Core.Bills;
using HouseFinance.Core.Bills.Payments;
using HouseFinance.Core.FileManagement;
using HouseFinance.Core.Shopping;
using HouseFinance.Models.API;
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

        [HttpPost]
        [Route("Api/Bills/Add")]
        public CommunicationResponse AddBill([FromBody]Bill billRequest)
        {
            var response = new CommunicationResponse();
            if (Authenticate(Request.Headers["Authorization"]) == false)
            {
                response.AddError("The API Key was invalid");
                return response;
            }

            try
            {
                BillValidator.CheckIfValidBill(billRequest);
                var genericFileHelper = new GenericFileHelper(FilePath.Bills);
                genericFileHelper.AddOrUpdate<Bill>(billRequest);

                response.Notifications = new List<string>
                {
                    $"The bill '{billRequest.Name}' has been added"
                };
            }
            catch (Exception exception)
            {
                response.AddError($"An unexpected exception occured: {exception}");
            }

            return response;
        }

        [HttpPost]
        [Route("Api/Bills/AddPayment")]
        public CommunicationResponse AddPayment([FromBody]Payment paymentRequest)
        {
            var response = new CommunicationResponse();
            if (Authenticate(Request.Headers["Authorization"]) == false)
            {
                response.AddError("The API Key was invalid");
                return response;
            }

            try
            {
                PaymentValidator.CheckIfValidPayment(paymentRequest);
                var genericFileHelper = new GenericFileHelper(FilePath.Payments);
                genericFileHelper.AddOrUpdate<Payment>(paymentRequest);

                response.Notifications = new List<string>
                {
                    "The payment has been added"
                };
            }
            catch (Exception exception)
            {
                response.AddError($"An unexpected exception occured: {exception}");
            }

            return response;
        }

        [HttpGet]
        [Route("Api/Shopping")]
        public GetShoppingResponse GetShoppingItems()
        {
            var response = new GetShoppingResponse();
            if (Authenticate(Request.Headers["Authorization"]) == false)
            {
                response.AddError("The API Key was invalid");
                return response;
            }

            try
            {
                response.Items = ShoppingListBuilder.BuildShoppingList();
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

    public class GetShoppingDetailsResponse : CommunicationResponse
    {
    }
}