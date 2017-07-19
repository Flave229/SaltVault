using System;
using HouseFinance.Core.FileManagement;

namespace HouseFinance.Core.People
{
    public class Person : IPersistedData
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Image { get; set; }

        public Person()
        {
            Id = Guid.NewGuid();
        }
    }
}
