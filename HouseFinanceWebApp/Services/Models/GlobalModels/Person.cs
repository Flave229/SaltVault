using System;

namespace Services.Models.GlobalModels
{
    public class Person
    {
        public Guid Id          { get; set; }
        public string FirstName { get; set; }
        public string LastName  { get; set; }
        public string Image     { get; set; }

        public Person()
        {
            Id = Guid.NewGuid();
        }
    }
}