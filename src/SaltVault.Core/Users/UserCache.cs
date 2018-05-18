using System;
using System.Collections.Generic;
using System.Linq;
using SaltVault.Core.Exception;

namespace SaltVault.Core.Users
{
    public class UserCache
    {
        private readonly Dictionary<Guid, ActiveUser> _activeUserSessions;

        public UserCache()
        {
            _activeUserSessions = new Dictionary<Guid, ActiveUser>();
        }

        public Guid GenerateUserSession(ActiveUser user)
        {
            Guid sessionKey = _activeUserSessions.FirstOrDefault(userSession => userSession.Value.PersonId == user.PersonId).Key;
            if (sessionKey.Equals(default(Guid)))
                sessionKey = Guid.NewGuid();
            
            _activeUserSessions[sessionKey] = user;
            return sessionKey;
        }

        public ActiveUser GetUserDataForSession(Guid sessionId)
        {
            ActiveUser user = _activeUserSessions[sessionId];

            if (user.GetCreationTime().AddDays(7) < DateTime.Now)
                throw new ErrorCodeException("User Session has expired", ErrorCode.USER_SESSION_EXPIRED);

            return user;
        }
    }
}
