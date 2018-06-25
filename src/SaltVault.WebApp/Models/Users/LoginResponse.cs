using System;
using SaltVault.Core.People;

namespace SaltVault.WebApp.Models.Users
{
    public class LoginResponse : CommunicationResponse
    {
        public Guid SessionId { get; set; }
        public Person User { get; set; }
        public bool NewUser { get; set; }
    }
}
