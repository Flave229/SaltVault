using System;
using System.Collections.Generic;

namespace HouseFinance.Api.Communication
{
    public static class AuthenticatedUsers
    {
        private static List<Guid> _authenticatedUsers = new List<Guid>
        {
            Guid.Parse("D2DB7539-634F-47C4-818D-59AD03C592E3")
        };

        public static bool CheckAuthentication(Guid authKey)
        {
            return _authenticatedUsers.Contains(authKey);
        }

        public static bool CheckAuthentication(string authKey)
        {
            Guid authKeyGuid;

            if (Guid.TryParse(authKey, out authKeyGuid))
            {
                return CheckAuthentication(authKeyGuid);
            }

            return false;
        }
    }
}
