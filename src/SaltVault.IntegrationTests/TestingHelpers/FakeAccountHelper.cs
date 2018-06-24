using System;
using System.Collections.Generic;
using SaltVault.Core.Users;

namespace SaltVault.IntegrationTests.TestingHelpers
{
    public class FakeAccountHelper : IAccountHelper
    {
        private List<Guid> _cachedSessionIds;

        public FakeAccountHelper()
        {
            _cachedSessionIds = new List<Guid>();
        }

        public Guid GenerateValidCredentials(int personId = 5)
        {
            Guid sessionId = UserCache.StaticGenerateUserSession(new ActiveUser
            {
                PersonId = personId,
                HouseId = 3
            });
            _cachedSessionIds.Add(sessionId);
            return sessionId;
        }

        public Guid GenerateValidExpiredCredentials()
        {
            Guid sessionId = UserCache.StaticGenerateUserSession(new ActiveUser
            {
                PersonId = 5,
                HouseId = 3,
                Created = DateTime.Now.AddDays(-8)
            });
            _cachedSessionIds.Add(sessionId);
            return sessionId;
        }

        public void CleanUp()
        {
            foreach (var cachedSessionId in _cachedSessionIds)
            {
                UserCache.StaticDeleteSession(cachedSessionId);
            }
        }
    }
}