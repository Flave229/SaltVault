using System.Collections.Generic;
using SaltVault.Core.People;

namespace SaltVault.WebApp.Models.Users
{
    public class GetUsersResponse : CommunicationResponse
    {
        public List<Person> People { get; set; }

        public GetUsersResponse()
        {
            People = new List<Person>();
        }
    }
}
