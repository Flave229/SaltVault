using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SaltVault.Core.Bills;
using SaltVault.Core.Exception;
using SaltVault.Core.Household;
using SaltVault.Core.Household.Model;
using SaltVault.Core.People;
using SaltVault.Core.Shopping;
using SaltVault.Core.ToDo;
using SaltVault.Core.Users;
using SaltVault.WebApp.Models;
using SaltVault.WebApp.Models.Household;
using SaltVault.WebApp.Models.Users;

namespace SaltVault.WebApp.Controllers
{
    public class HouseholdController : Controller
    {
        private readonly IUserService _userService;
        private readonly IInviteLinkService _inviteLinkService;
        private readonly IBillRepository _billRepository;
        private readonly IShoppingRepository _shoppingRepository;
        private readonly IPeopleRepository _peopleRepository;
        private readonly IToDoRepository _toDoRepository;
        private readonly IHouseholdRepository _houseRepository;

        public HouseholdController(IBillRepository billRepository, IShoppingRepository shoppingRepository, IPeopleRepository peopleRepository, IToDoRepository toDoRepository,
            IHouseholdRepository householdRepository, IUserService userService, IInviteLinkService inviteLinkService)
        {
            _userService = userService;
            _inviteLinkService = inviteLinkService;
            _billRepository = billRepository;
            _shoppingRepository = shoppingRepository;
            _peopleRepository = peopleRepository;
            _toDoRepository = toDoRepository;
            _houseRepository = householdRepository;
        }

        [HttpGet]
        [Route("Api/v2/Household")]
        public GetHouseholdResponse GetHousehold()
        {
            var response = new GetHouseholdResponse();

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
                    response.AddError("You must belong to a household", ErrorCode.USER_NOT_IN_HOUSEHOLD);
                    return response;
                }

                response.House = _houseRepository.GetHouseholdForUser(user.PersonId);
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
        [Route("Api/v2/Household")]
        public AddHouseholdResponse AddHousehold([FromBody]AddHouseholdRequest request)
        {
            var response = new AddHouseholdResponse();

            try
            {
                if (_userService.AuthenticateSession(Request.Headers["Authorization"].ToString()) == false)
                {
                    response.AddError("The authorization credentails were invalid", ErrorCode.SESSION_INVALID);
                    return response;
                }

                string sessionId = Request.Headers["Authorization"].ToString();
                ActiveUser user = _userService.GetUserInformationFromAuthHeader(sessionId);
                
                response.Id = _houseRepository.AddHousehold(request.Name, user.PersonId);
                _userService.UpdateHouseholdForUser(sessionId, response.Id);
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

        [HttpPatch]
        [Route("Api/v2/Household")]
        public CommunicationResponse UpdateHousehold([FromBody]UpdateHouseholdRequest request)
        {
            var response = new AddHouseholdResponse();

            try
            {
                if (_userService.AuthenticateSession(Request.Headers["Authorization"].ToString()) == false)
                {
                    response.AddError("The authorization credentails were invalid", ErrorCode.SESSION_INVALID);
                    return response;
                }
                
                UpdateHousehold updateHousehold = new UpdateHousehold
                {
                    Id = request.Id,
                    Name = request.Name
                };
                bool rowUpdated = _houseRepository.UpdateHousehold(updateHousehold);

                if (rowUpdated == false)
                {
                    response.AddError("The given House Id does not correspond to an existing Household");
                    return response;
                }
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

        [HttpGet]
        [Route("Api/v2/Household/InviteLink")]
        public CreateHouseholdInviteLinkResponse CreateInviteLink()
        {
            var response = new CreateHouseholdInviteLinkResponse();

            try
            {
                if (_userService.AuthenticateSession(Request.Headers["Authorization"].ToString()) == false)
                {
                    response.AddError("The authorization credentails were invalid", ErrorCode.SESSION_INVALID);
                    return response;
                }

                string sessionId = Request.Headers["Authorization"].ToString();
                ActiveUser user = _userService.GetUserInformationFromAuthHeader(sessionId);

                response.InviteLink = _inviteLinkService.GenerateInviteLinkForHousehold(user);
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
        [Route("Api/v2/Household/InviteLink")]
        public JoinHouseholdResponse JoinHouseholdWithInviteLink([FromBody]JoinHouseholdRequest request)
        {
            var response = new JoinHouseholdResponse();

            try
            {
                if (_userService.AuthenticateSession(Request.Headers["Authorization"].ToString()) == false)
                {
                    response.AddError("The authorization credentails were invalid", ErrorCode.SESSION_INVALID);
                    return response;
                }

                string sessionId = Request.Headers["Authorization"].ToString();
                ActiveUser user = _userService.GetUserInformationFromAuthHeader(sessionId);

                response.Id = _inviteLinkService.GetHouseholdForInviteLink(request.InviteLink);
                _houseRepository.AddPersonToHousehold(response.Id, user.PersonId);
                _userService.UpdateHouseholdForUser(sessionId, response.Id);
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

                string sessionId = Request.Headers["Authorization"].ToString();
                ActiveUser user = _userService.GetUserInformationFromAuthHeader(sessionId);
                if (user.HouseId == 0)
                {
                    response.AddError("You must belong to a household", ErrorCode.USER_NOT_IN_HOUSEHOLD);
                    return response;
                }

                _billRepository.DeleteHouseholdBills(user.HouseId);
                _shoppingRepository.DeleteHouseholdShoppingItems(user.HouseId);
                _toDoRepository.DeleteHouseholdToDoTask(user.HouseId);

                if (deleteHouseholdRequest.KeepHousehold == false)
                {
                    _houseRepository.DeleteHousehold(user.HouseId);
                    _userService.DeleteSession(sessionId);
                    _userService.UpdateHouseholdForUser(sessionId, -1);
                }
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
    }
}
