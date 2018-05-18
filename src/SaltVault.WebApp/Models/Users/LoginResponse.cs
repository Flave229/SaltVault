using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SaltVault.WebApp.Models.Users
{
    public class LoginResponse : CommunicationResponse
    {
        public Guid SessionId { get; set; }
        public bool NewUser { get; set; }
    }
}
