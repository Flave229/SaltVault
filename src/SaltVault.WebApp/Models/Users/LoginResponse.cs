using System;

namespace SaltVault.WebApp.Models.Users
{
    public class LoginResponse : CommunicationResponse
    {
        public Guid SessionId { get; set; }
        public bool NewUser { get; set; }
    }
}
