using System;
using SaltVault.Core.Users;

namespace SaltVault.Tests.TestingHelpers
{
    public class FakeTestingAccountHelper
    {
        public Guid GenerateValidFakeCredentials()
        {
            return UserCache.StaticGenerateUserSession(new ActiveUser
            {
                PersonId = 5,
                HouseId = 3
            });
        }

        public Guid GenerateValidExpiredFakeCredentials()
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
