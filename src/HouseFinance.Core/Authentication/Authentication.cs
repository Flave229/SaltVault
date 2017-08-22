using System;
using System.Collections.Generic;

namespace HouseFinance.Core.Authentication
{
    public class Authentication
    {
        private static readonly List<Guid> AuthenticatedUsers = new List<Guid>
        {
            Guid.Parse("D2DB7539-634F-47C4-818D-59AD03C592E3")
        };

        public static bool CheckKey(string authKey)
        {
            Guid authKeyGuid;

            return Guid.TryParse(authKey, out authKeyGuid) && AuthenticatedUsers.Contains(authKeyGuid);
        }
    }
}
