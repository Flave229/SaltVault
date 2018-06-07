using System;
using System.Collections.Generic;
using SaltVault.Core.People;
using SaltVault.Core.Users;

namespace SaltVault.IntegrationTests.TestingHelpers
{
    public class RealAccountHelper : IAccountHelper
    {
        private readonly IPeopleRepository _personRepository;
        private readonly List<int> _persistedUserIds;

        public RealAccountHelper()
        {
            _personRepository = new PeopleRepository();
            _persistedUserIds = new List<int>();
        }

        public Guid GenerateValidCredentials(int personId = 0)
        {
            ActiveUser user = _personRepository.AddPerson("Test", "Credentials", "NoUrl");
            _persistedUserIds.Add(user.PersonId);
            return UserCache.StaticGenerateUserSession(user);
        }

        public Guid GenerateValidExpiredCredentials()
        {
            throw new NotImplementedException();
        }

        public void CleanUp()
        {
            foreach (int userId in _persistedUserIds)
            {
                _personRepository.DeletePerson(userId);
            }
        }
    }
}
