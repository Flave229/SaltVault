using System;

namespace SaltVault.IntegrationTests.TestingHelpers
{
    public interface IAccountHelper
    {
        Guid GenerateValidCredentials();
        Guid GenerateValidExpiredCredentials();
        void CleanUp(Guid? sessionId);
    }
}