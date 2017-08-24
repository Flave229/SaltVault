using System.Collections.Generic;
using SaltVault.Core.Bills;
using SaltVault.Core.People;

namespace SaltVault.WebApp.Models
{
    public class BillModel
    {
        public Bill Bill { get; set; }
        public List<PersonModel> SelectedPeople { get; set; }

        public BillModel()
        {
            SelectedPeople = new List<PersonModel>();
        }
    }

    public class PersonModel
    {
        public Person Person { get; set; }
        public bool Selected { get; set; }
    }
}