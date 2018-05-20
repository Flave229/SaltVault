using System;
using System.Collections.Generic;
using System.Linq;
using SaltVault.Core.Exception;

namespace SaltVault.Core.Users
{
    public class UserCache
    {
        private static readonly Dictionary<Guid, ActiveUser> _activeUserSessions = new Dictionary<Guid, ActiveUser>();

        public UserCache()
        {
        }

        public static Guid StaticGenerateUserSession(ActiveUser user)
        {
            Guid sessionKey = _activeUserSessions.FirstOrDefault(userSession => userSession.Value.PersonId == user.PersonId).Key;
            if (sessionKey.Equals(default(Guid)))
                sessionKey = Guid.NewGuid();
            
            _activeUserSessions[sessionKey] = user;
            return sessionKey;
        }

        public Guid GenerateUserSession(ActiveUser user)
        {
            return StaticGenerateUserSession(user);
        }

        public ActiveUser GetUserDataForSession(Guid sessionId)
        {
            ActiveUser user = _activeUserSessions[sessionId];

            if (user.GetCreationTime().AddDays(7) < DateTime.Now)
                throw new ErrorCodeException("User Session has expired", ErrorCode.USER_SESSION_EXPIRED);

            return user;
        }

        public bool CheckSessionExists(Guid sessionId)
        {
            bool existingSession = _activeUserSessions.ContainsKey(sessionId);
            if (existingSession == false)
                return false;

            if (_activeUserSessions[sessionId].GetCreationTime().AddDays(7) < DateTime.Now)
                return false;

            return true;
        }
    }
}
