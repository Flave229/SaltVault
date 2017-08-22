using System.Collections.Generic;
using HouseFinance.Core.People;
using HouseFinance.Models.API;

namespace HouseFinance.Models.Users
{
    public class GetUsersResponse : CommunicationResponse
    {
        public List<Person> People { get; set; }
    }
}
