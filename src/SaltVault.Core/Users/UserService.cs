using System;
using System.IO;
using SaltVault.Core.People;
using SaltVault.Core.Services.Google;

namespace SaltVault.Core.Users
{
    public interface IUserService
    {
        UserSession LogInUser(GoogleTokenInformation tokenInformation);
        bool AuthenticateSession(string authHeader);
        ActiveUser GetUserInformationFromAuthHeader(string authHeader);
    }

    public class UserService : IUserService
    {
        private readonly UserCache _userCache;
        private readonly IPeopleRepository _peopleRepository;

        public UserService(UserCache userCache, IPeopleRepository peopleRepository)
        {
            _userCache = userCache;
            _peopleRepository = peopleRepository;
        }

        public UserSession LogInUser(GoogleTokenInformation tokenInformation)
        {
            bool newUser = false;
            ActiveUser user = _peopleRepository.GetPersonFromGoogleClientId(tokenInformation.sub);
            if (user == null)
            {
                user = _peopleRepository.AddPersonUsingGoogleInformation(tokenInformation);
                newUser = true;
            }

            Guid sessionId = _userCache.GenerateUserSession(user);
            return new UserSession
            {
                SessionId = sessionId,
                NewUser = newUser
            };
        }

        public bool AuthenticateSession(string authHeader)
        {
            try
            {
                string sanitisedToken = authHeader.Replace("Token ", "");
                return _userCache.CheckSessionExists(new Guid(sanitisedToken));
            }
            catch (System.Exception exception)
            {
                return false;
            }
        }


        public ActiveUser GetUserInformationFromAuthHeader(string authHeader)
        {
            string[] sanitisedTokens = authHeader.Replace("Token ", "").Split(',');
            return _userCache.GetUserDataForSession(new Guid(sanitisedTokens[1]));
        }
    }

    public class UserSession
    {
        public Guid SessionId { get; set; }
        public bool NewUser { get; set; }
    }
}