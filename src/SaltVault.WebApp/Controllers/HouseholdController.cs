using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SaltVault.Core.Bills;
using SaltVault.Core.Exception;
using SaltVault.Core.People;
using SaltVault.Core.Shopping;
using SaltVault.Core.ToDo;
using SaltVault.Core.Users;
using SaltVault.WebApp.Models;
using SaltVault.WebApp.Models.Users;

namespace SaltVault.WebApp.Controllers
{
    public class HouseholdController : Controller
    {
        private readonly IUserService _userService;
        private readonly IBillRepository _billRepository;
        private readonly IShoppingRepository _shoppingRepository;
        private readonly IPeopleRepository _peopleRepository;
        private readonly IToDoRepository _toDoRepository;

        public HouseholdController(IBillRepository billRepository, IShoppingRepository shoppingRepository, IPeopleRepository peopleRepository, IToDoRepository toDoRepository,
            IUserService userService)
        {
            _userService = userService;
            _billRepository = billRepository;
            _shoppingRepository = shoppingRepository;
            _peopleRepository = peopleRepository;
            _toDoRepository = toDoRepository;
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
                _toDoRepository.DeleteHouseholdToDoTask(user.HouseId);

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
