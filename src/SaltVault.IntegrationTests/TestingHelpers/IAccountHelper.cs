using System;

namespace SaltVault.IntegrationTests.TestingHelpers
{
    public interface IAccountHelper
    {
        Guid GenerateValidCredentials(int personId = 5);
        Guid GenerateValidExpiredCredentials();
        void CleanUp(Guid? sessionId);
    }
}