using System;

namespace SaltVault.Core.Users
{
    public class ActiveUser
    {
        public DateTime Created { get; set; }
        public int PersonId { get; set; }
        public int HouseId { get; set; }

        public ActiveUser()
        {
            Created = DateTime.Now;
        }
    }
}