using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using SaltVault.Core.Authentication;
using SaltVault.Core.Bills;
using SaltVault.Core.Bills.Models;
using SaltVault.Core.Bills.Payments;
using SaltVault.Core.People;
using SaltVault.Core.Services.Discord;
using SaltVault.Core.Shopping;
using SaltVault.Core.ToDo;
using SaltVault.Core.ToDo.Models;
using SaltVault.WebApp.Models;
using SaltVault.WebApp.Models.Bills;
using SaltVault.WebApp.Models.Shopping;
using SaltVault.WebApp.Models.ToDo;
using SaltVault.WebApp.Models.Users;
using AddPaymentRequest = SaltVault.Core.Bills.Models.AddPaymentRequest;
using UpdateBillRequest = SaltVault.Core.Bills.Models.UpdateBillRequest;
using UpdateShoppingItemRequest = SaltVault.Core.Shopping.Models.UpdateShoppingItemRequest;

namespace SaltVault.WebApp.Controllers
{
    public class ApiController : Controller
    {
        private readonly IDiscordService _discordService;
        private readonly IBillRepository _billRepository;
        private readonly IShoppingRepository _shoppingRepository;
        private readonly IPeopleRepository _peopleRepository;
        private readonly IAuthentication _apiAuthentication;
        private readonly IToDoRepository _toDoRepository;

        public ApiController(IBillRepository billRepository, IShoppingRepository shoppingRepository, IPeopleRepository peopleRepository, IToDoRepository toDoRepository, IAuthentication apiAuthentication, IDiscordService discordService)
        {
            _discordService = discordService;
            _billRepository = billRepository;
            _shoppingRepository = shoppingRepository;
            _peopleRepository = peopleRepository;
            _toDoRepository = toDoRepository;
            _apiAuthentication = apiAuthentication;
        }

        [HttpGet]
        [Route("Api/v2/Bills")]
        public GetBillListResponse GetBillList(int? id)
        {
            var response = new GetBillListResponse();
            if (Authenticate(Request.Headers["Authorization"]) == false)
            {
                response.AddError("The API Key was invalid");
                return response;
            }

            try
            {
                if (id == null)
                    response.Bills = _billRepository.GetAllBasicBillDetails();
                else
                    response.Bills.Add(_billRepository.GetBasicBillDetails((int)id));
            }
            catch (Exception exception)
            {
                response.AddError($"An unexpected exception occured: {exception}");
            }

            return response;
        }

        [HttpPost]
        [Route("Api/v2/Bills")]
        public AddBillResponse AddBill([FromBody]AddBillRequest billRequest)
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
        [Route("Api/v2/Bills")]
        public CommunicationResponse UpdateBill([FromBody]UpdateBillRequest billRequest)
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
        [Route("Api/v2/Bills")]
        public CommunicationResponse DeleteBill([FromBody]DeleteBillRequest deleteBillRequest)
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
        public CommunicationResponse AddPayment([FromBody]AddPaymentRequest paymentRequest)
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
        public CommunicationResponse UpdatePayment([FromBody]Core.Bills.Models.UpdatePaymentRequest paymentRequest)
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
        public CommunicationResponse DeletePayment([FromBody]DeletePaymentRequest paymentRequest)
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
        public CommunicationResponse UpdateShoppingItemV2([FromBody]UpdateShoppingItemRequest shoppingRequest)
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
                response.People = _peopleRepository.GetAllPeople();
            }
            catch (Exception exception)
            {
                response.AddError($"An unexpected exception occured: {exception}");
            }

            return response;
        }

        [HttpPost]
        [Route("Api/v2/LogIn")]
        public CommunicationResponse AuthenticateAndLogIn()
        {
            var response = new CommunicationResponse();
            response.AddError($"Endpoint not implemented");
            return response;
        }

        [HttpGet]
        [Route("Api/v2/ToDo")]
        public GetToDoResponse GetToDoList()
        {
            var response = new GetToDoResponse();
            if (Authenticate(Request.Headers["Authorization"]) == false)
            {
                response.AddError("The API Key was invalid");
                return response;
            }

            try
            {
                response.ToDoTasks = _toDoRepository.GetToDoList();
            }
            catch (Exception exception)
            {
                response.AddError($"An unexpected exception occured: {exception}");
            }

            return response;
        }

        [HttpPost]
        [Route("Api/v2/ToDo")]
        public AddToDoResponse AddToDoItem([FromBody]AddToDoTaskRequest toDoTaskRequest)
        {
            var response = new AddToDoResponse();
            if (Authenticate(Request.Headers["Authorization"]) == false)
            {
                response.AddError("The API Key was invalid");
                return response;
            }

            try
            {
                response.Id = _toDoRepository.AddToDoTask(toDoTaskRequest);

                response.Notifications = new List<string>
                {
                    $"The task '{toDoTaskRequest.Title}' has been added"
                };
            }
            catch (Exception exception)
            {
                response.AddError($"An unexpected exception occured: {exception}");
            }

            return response;
        }

        [HttpPatch]
        [Route("Api/v2/ToDo")]
        public CommunicationResponse UpdateToDoItem([FromBody]UpdateToDoRequest toDoRequest)
        {
            var response = new AddBillResponse();
            if (Authenticate(Request.Headers["Authorization"]) == false)
            {
                response.AddError("The API Key was invalid");
                return response;
            }

            try
            {
                var rowUpdated = _toDoRepository.UpdateToDoTask(toDoRequest);

                if (rowUpdated == false)
                {
                    response.AddError("The given To Do Task Id does not correspond to an existing Task");
                    return response;
                }

                response.Notifications = new List<string>
                {
                    $"The Task '{toDoRequest.Title}' has been updated"
                };
            }
            catch (Exception exception)
            {
                response.AddError($"An unexpected exception occured: {exception}");
            }

            return response;
        }

        [HttpDelete]
        [Route("Api/v2/ToDo")]
        public CommunicationResponse DeleteToDoItem([FromBody]DeleteToDoRequest deleteToDoRequest)
        {
            var response = new CommunicationResponse();
            if (Authenticate(Request.Headers["Authorization"]) == false)
            {
                response.AddError("The API Key was invalid");
                return response;
            }

            try
            {
                var rowUpdated = _toDoRepository.DeleteToDoTask(deleteToDoRequest.ToDoId);

                if (rowUpdated == false)
                {
                    response.AddError($"Cannot delete the To Do Task (ID: {deleteToDoRequest.ToDoId}) because it does not exist");
                    return response;
                }

                response.Notifications = new List<string>
                {
                    $"The To Do Task (ID: {deleteToDoRequest.ToDoId}) has been deleted"
                };
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
            return _apiAuthentication.CheckKey(apiKey);
        }
    }

    public class UpdatePaymentRequest
    {
        public Guid PaymentId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Created { get; set; }
    }
}