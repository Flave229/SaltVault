using System;
using System.Collections.Generic;

namespace SaltVault.Core.Authentication
{
    public interface IAuthentication
    {
        bool CheckKey(string authKey);
    }

    public class ApiAuthentication : IAuthentication
    {
        private static readonly List<Guid> AuthenticatedUsers = new List<Guid>
        {
            Guid.Parse("D2DB7539-634F-47C4-818D-59AD03C592E3")
        };

        public bool CheckKey(string authKey)
        {
            return Guid.TryParse(authKey, out var authKeyGuid) && AuthenticatedUsers.Contains(authKeyGuid);
        }
    }
}
