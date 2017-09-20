using System.Collections.Generic;
using SaltVault.Core.Bills;
using SaltVault.Core.People;

namespace SaltVault.WebApp.Models
{
    public class PaymentFormHelper
    {
        public Bill Bill { get; set; }
        public Payment Payment { get; set; }
        public List<Person> People { get; set; }
    }
}