using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SaltVault.Core;
using SaltVault.Core.Bills;
using SaltVault.Core.Bills.Models;
using SaltVault.Core.Bills.Payments;
using SaltVault.Core.Exception;
using SaltVault.Core.Services.Discord;
using SaltVault.Core.Users;
using SaltVault.WebApp.Models;
using SaltVault.WebApp.Models.Bills;

namespace SaltVault.WebApp.Controllers
{
    public class BillController : Controller
    {
        private readonly IDiscordService _discordService;
        private readonly IUserService _userService;
        private readonly IBillRepository _billRepository;
        private readonly IRecurringBillWorker _recurringBillWorker;

        public BillController(IBillRepository billRepository, IDiscordService discordService, IUserService userService, IRecurringBillWorker recurringBillWorker)
        {
            _userService = userService;
            _billRepository = billRepository;
            _discordService = discordService;
            _recurringBillWorker = recurringBillWorker;
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
                    response.AddError("You must belong to a household to get bills", ErrorCode.USER_NOT_IN_HOUSEHOLD);
                    return response;
                }

                _recurringBillWorker.GenerateNextMonthsBills(user.HouseId);

                if (id == null)
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
                    response.AddError("You must belong to a household to add bills", ErrorCode.USER_NOT_IN_HOUSEHOLD);
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
                    response.AddError("You must belong to a household to update bills", ErrorCode.USER_NOT_IN_HOUSEHOLD);
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
                    response.AddError("You must belong to a household to delete bills", ErrorCode.USER_NOT_IN_HOUSEHOLD);
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
    }
}
