using System;
using System.Collections.Generic;
using System.Net.Http;
using HouseFinance.Core.Authentication;
using HouseFinance.Core.Bills;
using HouseFinance.Core.Bills.Payments;
using HouseFinance.Core.Services.Discord;
using HouseFinance.Core.Shopping;
using HouseFinance.Models.API;
using HouseFinance.Models.Bills;
using HouseFinance.Models.Shopping;
using HouseFinance.Models.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace HouseFinance.Controllers
{
    public class ApiController : Controller
    {
        private readonly DiscordService _discordService;
        private readonly BillRepository _billRepository;
        private readonly ShoppingRepository _shoppingRepository;

        public ApiController()
        {
            _discordService = new DiscordService(new HttpClient());
            _billRepository = new BillRepository();
            _shoppingRepository = new ShoppingRepository();
        }

        [HttpGet]
        [Route("Api/v2/Bills")]
        public GetBillListResponse GetBillListV2()
        {
            var response = new GetBillListResponse();
            if (Authenticate(Request.Headers["Authorization"]) == false)
            {
                response.AddError("The API Key was invalid");
                return response;
            }

            try
            {
                response.Bills = _billRepository.GetAllBasicBillDetails();
            }
            catch (Exception exception)
            {
                response.AddError($"An unexpected exception occured: {exception}");
            }

            return response;
        }

        [HttpPost]
        [Route("Api/v2/Bills")]
        public GetBillResponse GetBillV2([FromBody]GetBillRequest billRequest)
        {
            var response = new GetBillResponse();
            if (Authenticate(Request.Headers["Authorization"]) == false)
            {
                response.AddError("The API Key was invalid");
                return response;
            }

            try
            {
                response.Bill = _billRepository.GetBasicBillDetails(billRequest.BillId);
            }
            catch (Exception exception)
            {
                response.AddError($"An unexpected exception occured: {exception}");
            }

            return response;
        }

        [HttpPost]
        [Route("Api/v2/Bills/Add")]
        public AddBillResponse AddBillV2([FromBody]AddBillRequest billRequest)
        {
            var response = new AddBillResponse();
            if (Authenticate(Request.Headers["Authorization"]) == false)
            {
                response.AddError("The API Key was invalid");
                return response;
            }

            try
            {
                BillValidator.CheckIfValidBill(billRequest);
                response.Id = _billRepository.AddBill(billRequest);

                _discordService.AddBillNotification(billRequest.Name, billRequest.Due, billRequest.TotalAmount);
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

        [HttpPatch]
        [Route("Api/v2/Bills/Update")]
        public CommunicationResponse UpdateBillV2([FromBody]UpdateBillRequestV2 billRequest)
        {
            var response = new AddBillResponse();
            if (Authenticate(Request.Headers["Authorization"]) == false)
            {
                response.AddError("The API Key was invalid");
                return response;
            }

            try
            {
                var rowUpdated = _billRepository.UpdateBill(billRequest);

                if (rowUpdated == false)
                {
                    response.AddError("The given Bill Id does not correspond to an existing Bill");
                    return response;
                }

                response.Notifications = new List<string>
                {
                    $"The bill '{billRequest.Name}' has been updated"
                };
            }
            catch (Exception exception)
            {
                response.AddError($"An unexpected exception occured: {exception}");
            }

            return response;
        }

        [HttpDelete]
        [Route("Api/v2/Bills/Delete")]
        public CommunicationResponse DeleteBillV2([FromBody]DeleteBillRequestV2 deleteBillRequest)
        {
            var response = new CommunicationResponse();
            if (Authenticate(Request.Headers["Authorization"]) == false)
            {
                response.AddError("The API Key was invalid");
                return response;
            }

            try
            {
                var rowUpdated = _billRepository.DeleteBill(deleteBillRequest.BillId);

                if (rowUpdated == false)
                {
                    response.AddError($"Cannot delete the bill (ID: {deleteBillRequest.BillId}) because it does not exist");
                    return response;
                }

                response.Notifications = new List<string>
                {
                    $"The bill (ID: {deleteBillRequest.BillId}) has been deleted"
                };
            }
            catch (Exception exception)
            {
                response.AddError($"An unexpected exception occured: {exception}");
            }

            return response;
        }

        [HttpPost]
        [Route("Api/v2/Bills/Payments")]
        public CommunicationResponse AddPaymentV2([FromBody]AddPaymentRequestV2 paymentRequest)
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
                _billRepository.AddPayment(paymentRequest);

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

        [HttpPatch]
        [Route("Api/v2/Bills/Payments")]
        public CommunicationResponse UpdatePaymentV2([FromBody]UpdatePaymentRequestV2 paymentRequest)
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
                _billRepository.UpdatePayment(paymentRequest);

                response.Notifications = new List<string>
                {
                    "The payment has been updated"
                };
            }
            catch (Exception exception)
            {
                response.AddError($"An unexpected exception occured: {exception}");
            }

            return response;
        }

        [HttpDelete]
        [Route("Api/v2/Bills/Payments")]
        public CommunicationResponse DeletePaymentV2([FromBody]DeletePaymentRequestV2 paymentRequest)
        {
            var response = new CommunicationResponse();
            if (Authenticate(Request.Headers["Authorization"]) == false)
            {
                response.AddError("The API Key was invalid");
                return response;
            }

            try
            {
                var rowUpdated = _billRepository.DeletePayment(paymentRequest.PaymentId);

                if (rowUpdated == false)
                {
                    response.AddError($"Cannot delete the payment (ID: {paymentRequest.PaymentId}) because it does not exist");
                    return response;
                }

                response.Notifications = new List<string>
                {
                    $"The payment (ID: {paymentRequest.PaymentId}) has been deleted"
                };
            }
            catch (Exception exception)
            {
                response.AddError($"An unexpected exception occured: {exception}");
            }

            return response;
        }

        [HttpGet]
        [Route("Api/v2/Shopping")]
        public GetShoppingResponse GetShoppingItemsV2()
        {
            var response = new GetShoppingResponse();
            if (Authenticate(Request.Headers["Authorization"]) == false)
            {
                response.AddError("The API Key was invalid");
                return response;
            }

            try
            {
                response.Items = _shoppingRepository.GetAllItems();
            }
            catch (Exception exception)
            {
                response.AddError($"An unexpected exception occured: {exception}");
            }

            return response;
        }

        [HttpPost]
        [Route("Api/v2/Shopping")]
        public CommunicationResponse AddShoppingItemV2([FromBody]AddShoppingItemRequest shoppingRequest)
        {
            var response = new CommunicationResponse();
            if (Authenticate(Request.Headers["Authorization"]) == false)
            {
                response.AddError("The API Key was invalid");
                return response;
            }

            try
            {
                ShoppingValidator.CheckIfValidItem(shoppingRequest);
                _shoppingRepository.AddItem(shoppingRequest);

                response.Notifications = new List<string>
                {
                    $"The shopping item '{shoppingRequest.Name}' has been added"
                };
            }
            catch (Exception exception)
            {
                response.AddError($"An unexpected exception occured: {exception}");
            }

            return response;
        }

        [HttpPatch]
        [Route("Api/v2/Shopping")]
        public CommunicationResponse UpdateShoppingItemV2([FromBody]UpdateShoppingItemRequestV2 shoppingRequest)
        {
            var response = new CommunicationResponse();
            if (Authenticate(Request.Headers["Authorization"]) == false)
            {
                response.AddError("The API Key was invalid");
                return response;
            }

            try
            {
                ShoppingValidator.CheckIfValidItem(shoppingRequest);
                _shoppingRepository.UpdateItem(shoppingRequest);

                response.Notifications = new List<string>
                {
                    $"The shopping item '{shoppingRequest.Name}' has been updated"
                };
            }
            catch (Exception exception)
            {
                response.AddError($"An unexpected exception occured: {exception}");
            }

            return response;
        }

        [HttpDelete]
        [Route("Api/v2/Shopping")]
        public CommunicationResponse DeleteShoppingItemV2([FromBody]DeleteShoppingItemRequest deleteShoppingItemRequest)
        {
            var response = new CommunicationResponse();
            if (Authenticate(Request.Headers["Authorization"]) == false)
            {
                response.AddError("The API Key was invalid");
                return response;
            }

            try
            {
                _shoppingRepository.DeleteItem(deleteShoppingItemRequest.ShoppingItemId);

                response.Notifications = new List<string>
                {
                    "The shopping item has been deleted"
                };
            }
            catch (Exception exception)
            {
                response.AddError($"An unexpected exception occured: {exception}");
            }

            return response;
        }

        [HttpGet]
        [Route("Api/v2/Users")]
        public GetUsersResponse GetUsers()
        {
            var response = new GetUsersResponse();
            if (Authenticate(Request.Headers["Authorization"]) == false)
            {
                response.AddError("The API Key was invalid");
                return response;
            }

            try
            {
                response.People = _billRepository.GetAllPeople();
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

    public class UpdatePaymentRequest
    {
        public Guid PaymentId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Created { get; set; }
    }
}