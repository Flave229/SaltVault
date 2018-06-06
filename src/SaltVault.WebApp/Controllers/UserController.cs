using System;
using Microsoft.AspNetCore.Mvc;
using SaltVault.Core.Exception;
using SaltVault.Core.People;
using SaltVault.Core.Services.Google;
using SaltVault.Core.Users;
using SaltVault.WebApp.Models.Users;

namespace SaltVault.WebApp.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IGoogleTokenAuthentication _googleTokenAuthentication;
        private readonly IPeopleRepository _peopleRepository;

        public UserController(IPeopleRepository peopleRepository, IUserService userService, IGoogleTokenAuthentication googleTokenAuthentication)
        {
            _userService = userService;
            _googleTokenAuthentication = googleTokenAuthentication;
            _peopleRepository = peopleRepository;
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
                    response.AddError("You must belong to a household to get users", ErrorCode.USER_NOT_IN_HOUSEHOLD);
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
    }
}