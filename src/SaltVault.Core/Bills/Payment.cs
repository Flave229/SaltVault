using System;
using SaltVault.Core.People;

namespace SaltVault.Core.Bills
{
    public class Payment
    {
        public int Id { get; set; }
        public DateTime DatePaid { get; set; }
        public decimal Amount { get; set; }
        public Person Person { get; set; }
        public int PersonId { get; set; }
        public string PersonName { get; set; }
    }
}