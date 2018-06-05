using System;
using SaltVault.Core.Users;

namespace SaltVault.IntegrationTests.TestingHelpers
{
    public class FakeAccountHelper : IAccountHelper
    {
        public Guid GenerateValidCredentials()
        {
            return UserCache.StaticGenerateUserSession(new ActiveUser
            {
                PersonId = 5,
                HouseId = 3
            });
        }

        public Guid GenerateValidExpiredCredentials()
        {
            return UserCache.StaticGenerateUserSession(new ActiveUser
            {
                PersonId = 5,
                HouseId = 3,
                Created = DateTime.Now.AddDays(-8)
            });
        }

        public void CleanUp(Guid? sessionId)
        {
            if (sessionId != null)
                UserCache.TerminateSession((Guid)sessionId);
        }
    }
}