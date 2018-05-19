using System;
using System.IO;
using SaltVault.Core.People;
using SaltVault.Core.Services.Google;

namespace SaltVault.Core.Users
{
    public interface IUserService
    {
        UserSession LogInUser(GoogleTokenInformation tokenInformation);
        bool AuthenticateSession(string requestHeader);
        bool AuthenticateClientId(string clientId);
    }

    public class UserService : IUserService
    {
        private readonly UserCache _userCache;
        private readonly IPeopleRepository _peopleRepository;
        private string _appClientId;

        public UserService(UserCache userCache, IPeopleRepository peopleRepository)
        {
            _userCache = userCache;
            _peopleRepository = peopleRepository;
            _appClientId = File.ReadAllText("./Data/Config/ClientIds.config");
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

        public bool AuthenticateSession(string requestHeader)
        {
            try
            {
                string[] sanitisedTokens = requestHeader.Replace("ClientID ", "").Replace("Token ", "").Split(',');
                return _appClientId.Contains(sanitisedTokens[0]) && _userCache.CheckSessionExists(new Guid(sanitisedTokens[1]));
            }
            catch (System.Exception exception)
            {
                return false;
            }
        }

        public bool AuthenticateClientId(string clientId)
        {
            try
            {
                string sanitisedToken = clientId.Replace("ClientID ", "");
                return _appClientId.Contains(sanitisedToken);
            }
            catch (System.Exception exception)
            {
                return false;
            }
        }
    }

    public class UserSession
    {
        public Guid SessionId { get; set; }
        public bool NewUser { get; set; }
    }
}