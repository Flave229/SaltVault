using System;

namespace SaltVault.Core.Users
{
    public class ActiveUser
    {
        private readonly DateTime _created;
        public int PersonId { get; set; }
        public int HouseId { get; set; }

        public ActiveUser()
        {
            _created = DateTime.Now;
        }

        public DateTime GetCreationTime()
        {
            return _created;
        }
    }
}