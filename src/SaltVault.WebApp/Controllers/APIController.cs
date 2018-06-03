using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SaltVault.Core;
using SaltVault.Core.Bills;
using SaltVault.Core.Bills.Models;
using SaltVault.Core.Bills.Payments;
using SaltVault.Core.Exception;
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
        private readonly IToDoRepository _toDoRepository;

        public ApiController(IBillRepository billRepository, IShoppingRepository shoppingRepository, IPeopleRepository peopleRepository, IToDoRepository toDoRepository, 
                                IDiscordService discordService, IUserService userService, IGoogleTokenAuthentication googleTokenAuthentication)
        {
            _discordService = discordService;
            _userService = userService;
            _googleTokenAuthentication = googleTokenAuthentication;
            _billRepository = billRepository;
            _shoppingRepository = shoppingRepository;
            _peopleRepository = peopleRepository;
            _toDoRepository = toDoRepository;
        }

        [HttpGet]
        [Route("Api/v2/Bills")]
        public GetBillListResponse GetBillList(int? id, int? page, int? resultsPerPage)
        {
            var response = new GetBillListResponse();
            try
            {
                if (_userService.AuthenticateSession(Request.Headers["Authorization"].ToString()) == false)
                {
                    response.AddError("The authorization credentails were invalid", Request.Headers["Authorization"].ToString(), ErrorCode.SESSION_INVALID);
                    return response;
                }

                ActiveUser user = _userService.GetUserInformationFromAuthHeader(Request.Headers["Authorization"].ToString());
                if (user.HouseId == 0)
                {
                    response.Notifications.Add("You must belong to a household to retrieve bills");
                }
                else if (id == null)
                {
                    Pagination pagination = new Pagination
                    {
                        Page = page ?? 0,
                        ResultsPerPage = resultsPerPage ?? int.MaxValue
                    };
                    response.Bills = _billRepository.GetAllBasicBillDetails(pagination, user.HouseId);
                }
                else
                    response.Bills.Add(_billRepository.GetBasicBillDetails((int)id, user.HouseId));
            }
            catch (ErrorCodeException exception)
            {
                response.AddError($"An unexpected exception occured: {exception}", exception.Code);
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

            try
            {
                if (_userService.AuthenticateSession(Request.Headers["Authorization"].ToString()) == false)
                {
                    response.AddError("The authorization credentails were invalid", ErrorCode.SESSION_INVALID);
                    return response;
                }

                ActiveUser user = _userService.GetUserInformationFromAuthHeader(Request.Headers["Authorization"].ToString());
                if (user.HouseId == 0)
                {
                    response.Notifications.Add("You must belong to a household to add bills");
                    return response;
                }

                AddBill bill = new AddBill
                {
                    Name = billRequest.Name,
                    Due = billRequest.Due,
                    PeopleIds = billRequest.PeopleIds,
                    RecurringType = billRequest.RecurringType,
                    TotalAmount = billRequest.TotalAmount,
                    HouseId = user.HouseId
                };
                BillValidator.CheckIfValidBill(bill);
                response.Id = _billRepository.AddBill(bill);

                if (user.HouseId == 1)
                    _discordService.AddBillNotification(billRequest.Name, billRequest.Due, billRequest.TotalAmount);

                response.Notifications = new List<string>
                {
                    $"The bill '{billRequest.Name}' has been added"
                };
            }
            catch (ErrorCodeException exception)
            {
                response.AddError($"An unexpected exception occured: {exception}", billRequest, exception.Code);
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

            try
            {
                if (_userService.AuthenticateSession(Request.Headers["Authorization"].ToString()) == false)
                {
                    response.AddError("The authorization credentails were invalid", ErrorCode.SESSION_INVALID);
                    return response;
                }

                ActiveUser user = _userService.GetUserInformationFromAuthHeader(Request.Headers["Authorization"].ToString());
                if (user.HouseId == 0)
                {
                    response.Notifications.Add("You must belong to a household to add bills");
                    return response;
                }
                UpdateBill bill = new UpdateBill
                {
                    Name = billRequest.Name,
                    Id = billRequest.Id,
                    Due = billRequest.Due,
                    PeopleIds = billRequest.PeopleIds,
                    TotalAmount = billRequest.TotalAmount,
                    RecurringType = billRequest.RecurringType
                };
                var rowUpdated = _billRepository.UpdateBill(bill);

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
            catch (ErrorCodeException exception)
            {
                response.AddError($"An unexpected exception occured: {exception}", billRequest, exception.Code);
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

            try
            {
                if (_userService.AuthenticateSession(Request.Headers["Authorization"].ToString()) == false)
                {
                    response.AddError("The authorization credentails were invalid", ErrorCode.SESSION_INVALID);
                    return response;
                }

                ActiveUser user = _userService.GetUserInformationFromAuthHeader(Request.Headers["Authorization"].ToString());
                if (user.HouseId == 0)
                {
                    response.Notifications.Add("You must belong to a household to add bills");
                    return response;
                }
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
            catch (ErrorCodeException exception)
            {
                response.AddError($"An unexpected exception occured: {exception}", deleteBillRequest, exception.Code);
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

            try
            {
                if (_userService.AuthenticateSession(Request.Headers["Authorization"].ToString()) == false)
                {
                    response.AddError("The authorization credentails were invalid", ErrorCode.SESSION_INVALID);
                    return response;
                }

                var payment = new Payment
                {
                    Amount = paymentRequest.Amount,
                    DatePaid = paymentRequest.Created,
                    PersonId = paymentRequest.PersonId
                };
                PaymentValidator.CheckIfValidPayment(payment, paymentRequest.BillId);
                _billRepository.AddPayment(payment, paymentRequest.BillId);

                response.Notifications = new List<string>
                {
                    "The payment has been added"
                };
            }
            catch (ErrorCodeException exception)
            {
                response.AddError($"An unexpected exception occured: {exception}", paymentRequest, exception.Code);
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

            try
            {
                if (_userService.AuthenticateSession(Request.Headers["Authorization"].ToString()) == false)
                {
                    response.AddError("The authorization credentails were invalid", ErrorCode.SESSION_INVALID);
                    return response;
                }

                PaymentValidator.CheckIfValidPayment(paymentRequest);
                _billRepository.UpdatePayment(paymentRequest);

                response.Notifications = new List<string>
                {
                    "The payment has been updated"
                };
            }
            catch (ErrorCodeException exception)
            {
                response.AddError($"An unexpected exception occured: {exception}", paymentRequest, exception.Code);
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

            try
            {
                if (_userService.AuthenticateSession(Request.Headers["Authorization"].ToString()) == false)
                {
                    response.AddError("The authorization credentails were invalid", ErrorCode.SESSION_INVALID);
                    return response;
                }

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
            catch (ErrorCodeException exception)
            {
                response.AddError($"An unexpected exception occured: {exception}", paymentRequest, exception.Code);
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

            try
            {
                if (_userService.AuthenticateSession(Request.Headers["Authorization"].ToString()) == false)
                {
                    response.AddError("The authorization credentails were invalid", ErrorCode.SESSION_INVALID);
                    return response;
                }

                ActiveUser user = _userService.GetUserInformationFromAuthHeader(Request.Headers["Authorization"].ToString());
                if (user.HouseId == 0)
                {
                    response.Notifications.Add("You must belong to a household to add shopping items");
                    return response;
                }
                Pagination pagination = new Pagination
                {
                    Page = page ?? 0,
                    ResultsPerPage = resultsPerPage ?? int.MaxValue
                };
                response.ShoppingList = _shoppingRepository.GetAllItems(pagination, user.HouseId);
            }
            catch (ErrorCodeException exception)
            {
                response.AddError($"An unexpected exception occured: {exception}", exception.Code);
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

            try
            {
                if (_userService.AuthenticateSession(Request.Headers["Authorization"].ToString()) == false)
                {
                    response.AddError("The authorization credentails were invalid", ErrorCode.SESSION_INVALID);
                    return response;
                }

                ActiveUser user = _userService.GetUserInformationFromAuthHeader(Request.Headers["Authorization"].ToString());
                if (user.HouseId == 0)
                {
                    response.Notifications.Add("You must belong to a household to add Shopping Items");
                    return response;
                }
                ShoppingItem item = new ShoppingItem
                {
                    ItemFor = shoppingRequest.ItemFor,
                    Added = DateTime.Now,
                    AddedBy = user.PersonId,
                    Name = shoppingRequest.Name,
                    Purchased = false
                };
                ShoppingValidator.CheckIfValidItem(item);
                _shoppingRepository.AddItem(item, user.HouseId);

                response.Notifications = new List<string>
                {
                    $"The shopping item '{shoppingRequest.Name}' has been added"
                };
            }
            catch (ErrorCodeException exception)
            {
                response.AddError($"An unexpected exception occured: {exception}", shoppingRequest, exception.Code);
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

            try
            {
                if (_userService.AuthenticateSession(Request.Headers["Authorization"].ToString()) == false)
                {
                    response.AddError("The authorization credentails were invalid", ErrorCode.SESSION_INVALID);
                    return response;
                }

                ShoppingValidator.CheckIfValidItem(shoppingRequest);
                _shoppingRepository.UpdateItem(shoppingRequest);

                response.Notifications = new List<string>
                {
                    $"The shopping item '{shoppingRequest.Name}' has been updated"
                };
            }
            catch (ErrorCodeException exception)
            {
                response.AddError($"An unexpected exception occured: {exception}", shoppingRequest, exception.Code);
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

            try
            {
                if (_userService.AuthenticateSession(Request.Headers["Authorization"].ToString()) == false)
                {
                    response.AddError("The authorization credentails were invalid", ErrorCode.SESSION_INVALID);
                    return response;
                }

                _shoppingRepository.DeleteItem(deleteShoppingItemRequest.ShoppingItemId);

                response.Notifications = new List<string>
                {
                    "The shopping item has been deleted"
                };
            }
            catch (ErrorCodeException exception)
            {
                response.AddError($"An unexpected exception occured: {exception}", deleteShoppingItemRequest, exception.Code);
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

            try
            {
                if (_userService.AuthenticateSession(Request.Headers["Authorization"].ToString()) == false)
                {
                    response.AddError("The authorization credentails were invalid", ErrorCode.SESSION_INVALID);
                    return response;
                }

                ActiveUser user = _userService.GetUserInformationFromAuthHeader(Request.Headers["Authorization"].ToString());
                if (user.HouseId == 0)
                {
                    response.Notifications.Add("You must belong to a household to get users for a household");
                    return response;
                }
                response.People = _peopleRepository.GetAllPeople(user.HouseId);
            }
            catch (ErrorCodeException exception)
            {
                response.AddError($"An unexpected exception occured: {exception}", exception.Code);
            }
            catch (Exception exception)
            {
                response.AddError($"An unexpected exception occured: {exception}");
            }

            return response;
        }

        [HttpDelete]
        [Route("Api/v2/Household")]
        public CommunicationResponse DeleteHousehold([FromBody]DeleteHouseholdRequest deleteHouseholdRequest)
        {
            var response = new CommunicationResponse();

            try
            {
                if (_userService.AuthenticateSession(Request.Headers["Authorization"].ToString()) == false)
                {
                    response.AddError("The authorization credentails were invalid", ErrorCode.SESSION_INVALID);
                    return response;
                }

                ActiveUser user = _userService.GetUserInformationFromAuthHeader(Request.Headers["Authorization"].ToString());
                if (user.HouseId == 0)
                {
                    response.Notifications.Add("You must belong to a household");
                    return response;
                }

                _billRepository.DeleteHouseholdBills(user.HouseId);
                _shoppingRepository.DeleteHouseholdShoppingItems(user.HouseId);

                response.Notifications = new List<string>
                {
                    $"The data for the household (ID: {user.HouseId}) have been deleted"
                };
            }
            catch (ErrorCodeException exception)
            {
                response.AddError($"An unexpected exception occured: {exception}", deleteHouseholdRequest, exception.Code);
            }
            catch (Exception exception)
            {
                response.AddError($"An unexpected exception occured: {exception}", deleteHouseholdRequest);
            }

            return response;
        }

        [HttpPost]
        [Route("Api/v2/LogIn")]
        public LoginResponse AuthenticateAndLogIn([FromBody]LogInRequest request)
        {
            var response = new LoginResponse();

            try
            {
                GoogleTokenInformation tokenInformation = _googleTokenAuthentication.VerifyToken(request.Token);

                if (tokenInformation.Valid == false)
                {
                    response.AddError($"Server failed to verify Google credentials. Please try again.", request);
                    return response;
                }

                UserSession sessionInformation = _userService.LogInUser(tokenInformation);
                response.NewUser = sessionInformation.NewUser;
                response.SessionId = sessionInformation.SessionId;
            }
            catch (ErrorCodeException exception)
            {
                response.AddError($"An unexpected exception occured: {exception}", request, exception.Code);
            }
            catch (Exception exception)
            {
                response.AddError($"An unexpected exception occured: {exception}", request);
            }
            return response;
        }

        [HttpGet]
        [Route("Api/v2/ToDo")]
        public GetToDoResponse GetToDoList()
        {
            var response = new GetToDoResponse();

            try
            {
                if (_userService.AuthenticateSession(Request.Headers["Authorization"].ToString()) == false)
                {
                    response.AddError("The authorization credentails were invalid", ErrorCode.SESSION_INVALID);
                    return response;
                }

                ActiveUser user = _userService.GetUserInformationFromAuthHeader(Request.Headers["Authorization"].ToString());
                if (user.HouseId == 0)
                {
                    response.Notifications.Add("You must belong to a household to get To Do Tasks");
                    return response;
                }
                response.ToDoTasks = _toDoRepository.GetToDoList(user.HouseId);
            }
            catch (ErrorCodeException exception)
            {
                response.AddError($"An unexpected exception occured: {exception}", exception.Code);
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

            try
            {
                if (_userService.AuthenticateSession(Request.Headers["Authorization"].ToString()) == false)
                {
                    response.AddError("The authorization credentails were invalid", ErrorCode.SESSION_INVALID);
                    return response;
                }

                ActiveUser user = _userService.GetUserInformationFromAuthHeader(Request.Headers["Authorization"].ToString());
                if (user.HouseId == 0)
                {
                    response.Notifications.Add("You must belong to a household to add To Do Tasks");
                    return response;
                }
                response.Id = _toDoRepository.AddToDoTask(toDoTaskRequest, user.HouseId);

                response.Notifications = new List<string>
                {
                    $"The task '{toDoTaskRequest.Title}' has been added"
                };
            }
            catch (ErrorCodeException exception)
            {
                response.AddError($"An unexpected exception occured: {exception}", toDoTaskRequest, exception.Code);
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

            try
            {
                if (_userService.AuthenticateSession(Request.Headers["Authorization"].ToString()) == false)
                {
                    response.AddError("The authorization credentails were invalid", ErrorCode.SESSION_INVALID);
                    return response;
                }

                _toDoRepository.UpdateToDoTask(toDoRequest);

                response.Notifications = new List<string>
                {
                    $"The Task '{toDoRequest.Title}' has been updated"
                };
            }
            catch (ErrorCodeException exception)
            {
                response.AddError($"An unexpected exception occured: {exception}", toDoRequest, exception.Code);
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

            try
            {
                if (_userService.AuthenticateSession(Request.Headers["Authorization"].ToString()) == false)
                {
                    response.AddError("The authorization credentails were invalid", ErrorCode.SESSION_INVALID);
                    return response;
                }

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
            catch (ErrorCodeException exception)
            {
                response.AddError($"An unexpected exception occured: {exception}", deleteToDoRequest, exception.Code);
            }
            catch (Exception exception)
            {
                response.AddError($"An unexpected exception occured: {exception}", deleteToDoRequest);
            }

            return response;
        }
    }

    public class UpdatePaymentRequest
    {
        public Guid PaymentId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Created { get; set; }
    }
}