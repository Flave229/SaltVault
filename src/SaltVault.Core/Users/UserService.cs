﻿using System;
using System.Collections.Generic;
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
        void UpdateHouseholdForUser(string sessionId, int houseId);
        void DeleteSession(string sessionId);
        Person GetPersonFromSession(string sessionId);
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
            string sanitisedToken = authHeader.Replace("Token ", "");
            return _userCache.CheckSessionExists(new Guid(sanitisedToken));
        }

        public ActiveUser GetUserInformationFromAuthHeader(string authHeader)
        {
            string sanitisedToken = authHeader.Replace("Token ", "");
            return _userCache.GetUserDataForSession(new Guid(sanitisedToken));
        }

        public void UpdateHouseholdForUser(string sessionId, int houseId)
        {
            string sanitisedToken = sessionId.Replace("Token ", "");
            _userCache.UpdateHouseIdForUser(new Guid(sanitisedToken), houseId);
        }

        public void DeleteSession(string sessionId)
        {
            string sanitisedToken = sessionId.Replace("Token ", "");
            _userCache.DeleteSession(new Guid(sanitisedToken));
        }

        public Person GetPersonFromSession(string sessionId)
        {
            ActiveUser user = _userCache.GetUserDataForSession(new Guid(sessionId));
            return _peopleRepository.GetPeople(new List<int> {user.PersonId})[0];
        }
    }

    public class UserSession
    {
        public Guid SessionId { get; set; }
        public bool NewUser { get; set; }
    }
}