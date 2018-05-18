using System;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using SaltVault.Core;
using SaltVault.Core.Authentication;
using SaltVault.Core.Bills;
using SaltVault.Core.Bills.Models;
using SaltVault.Core.Bills.Payments;
using SaltVault.Core.People;
using SaltVault.Core.Services.Discord;
using SaltVault.Core.Services.Google;
using SaltVault.Core.Shopping;
using SaltVault.Core.ToDo;
using SaltVault.Core.ToDo.Models;
using SaltVault.Core.Users;
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
        private readonly IUserService _userService;
        private readonly IGoogleTokenAuthentication _googleTokenAuthentication;
        private readonly IBillRepository _billRepository;
        private readonly IShoppingRepository _shoppingRepository;
        private readonly IPeopleRepository _peopleRepository;
        private readonly IAuthentication _apiAuthentication;
        private readonly IToDoRepository _toDoRepository;

        public ApiController(IBillRepository billRepository, IShoppingRepository shoppingRepository, IPeopleRepository peopleRepository, IToDoRepository toDoRepository, 
                                IAuthentication apiAuthentication, IDiscordService discordService, IUserService userService, IGoogleTokenAuthentication googleTokenAuthentication)
        {
            _discordService = discordService;
            _userService = userService;
            _googleTokenAuthentication = googleTokenAuthentication;
            _billRepository = billRepository;
            _shoppingRepository = shoppingRepository;
            _peopleRepository = peopleRepository;
            _toDoRepository = toDoRepository;
            _apiAuthentication = apiAuthentication;
        }

        [HttpGet]
        [Route("Api/v2/Bills")]
        public GetBillListResponse GetBillList(int? id, int? page, int? resultsPerPage)
        {
            var response = new GetBillListResponse();
            if (_userService.AuthenticateSession(Request.Headers["Authorization"].ToString()) == false)
            {
                // Fallback 
                response.Notifications.Add("Using the old API Key Authorization. This is soon to be removed and needs to be migrated immediately.");
                if (Authenticate(Request.Headers["Authorization"]) == false)
                {
                    response.AddError("The authorization credentails were invalid");
                    return response;
                }
            }

            try
            {
                if (id == null)
                {
                    Pagination pagination = new Pagination
                    {
                        Page = page ?? 0,
                        ResultsPerPage = resultsPerPage ?? int.MaxValue
                    };
                    response.Bills = _billRepository.GetAllBasicBillDetails(pagination);
                }
                else
                    response.Bills.Add(_billRepository.GetBasicBillDetails((int)id));
            }
            catch (Exception exception)
            {
                response.AddError($"An unexpected exception occured: {exception}", id);
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
                AddBill bill = new AddBill
                {
                    Name = billRequest.Name,
                    Due = billRequest.Due,
                    PeopleIds = billRequest.PeopleIds,
                    RecurringType = billRequest.RecurringType,
                    TotalAmount = billRequest.TotalAmount
                };
                BillValidator.CheckIfValidBill(bill);
                response.Id = _billRepository.AddBill(bill);

                _discordService.AddBillNotification(billRequest.Name, billRequest.Due, billRequest.TotalAmount);
                response.Notifications = new List<string>
                {
                    $"The bill '{billRequest.Name}' has been added"
                };
            }
            catch (Exception exception)
            {
                response.AddError($"An unexpected exception occured: {exception}", billRequest);
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
                response.AddError($"An unexpected exception occured: {exception}", billRequest);
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
                response.AddError($"An unexpected exception occured: {exception}", deleteBillRequest);
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
                response.AddError($"An unexpected exception occured: {exception}", paymentRequest);
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
                response.AddError($"An unexpected exception occured: {exception}", paymentRequest);
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
                response.AddError($"An unexpected exception occured: {exception}", paymentRequest);
            }

            return response;
        }

        [HttpGet]
        [Route("Api/v2/Shopping")]
        public GetShoppingResponse GetShoppingItemsV2(int? page, int? resultsPerPage)
        {
            var response = new GetShoppingResponse();
            if (Authenticate(Request.Headers["Authorization"]) == false)
            {
                response.AddError("The API Key was invalid");
                return response;
            }

            try
            {
                Pagination pagination = new Pagination
                {
                    Page = page ?? 0,
                    ResultsPerPage = resultsPerPage ?? int.MaxValue
                };
                response.ShoppingList = _shoppingRepository.GetAllItems(pagination);
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
                response.AddError($"An unexpected exception occured: {exception}", shoppingRequest);
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
                response.AddError($"An unexpected exception occured: {exception}", shoppingRequest);
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
                response.AddError($"An unexpected exception occured: {exception}", deleteShoppingItemRequest);
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
        public LoginResponse AuthenticateAndLogIn([FromBody]LogInRequest request)
        {
            var response = new LoginResponse();
            if (Authenticate(Request.Headers["Authorization"]) == false)
            {
                response.AddError("The API Key was invalid");
                return response;
            }
            try
            {
                GoogleTokenInformation tokenInformation = _googleTokenAuthentication.VerifyToken(request.Token);
                
                if (tokenInformation.Valid == false)
                {
                    response.AddError($"Server failed to verify Google credentials. Please try again.");
                    return response;
                }

                UserSession sessionInformation = _userService.LogInUser(tokenInformation);
                response.NewUser = sessionInformation.NewUser;
                response.SessionId = sessionInformation.SessionId;
            }
            catch (Exception exception)
            {
                response.AddError($"An unexpected exception occured: {exception}");
            }
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
                response.AddError($"An unexpected exception occured: {exception}", toDoTaskRequest);
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
                _toDoRepository.UpdateToDoTask(toDoRequest);

                response.Notifications = new List<string>
                {
                    $"The Task '{toDoRequest.Title}' has been updated"
                };
            }
            catch (Exception exception)
            {
                response.AddError($"An unexpected exception occured: {exception}", toDoRequest);
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
                var rowUpdated = _toDoRepository.DeleteToDoTask(deleteToDoRequest.Id);

                if (rowUpdated == false)
                {
                    response.AddError($"Cannot delete the To Do Task (ID: {deleteToDoRequest.Id}) because it does not exist");
                    return response;
                }

                response.Notifications = new List<string>
                {
                    $"The To Do Task (ID: {deleteToDoRequest.Id}) has been deleted"
                };
            }
            catch (Exception exception)
            {
                response.AddError($"An unexpected exception occured: {exception}", deleteToDoRequest);
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