using System.Collections.Generic;
using HouseFinance.Core.Bills;
using HouseFinance.Core.People;

namespace HouseFinance.Models
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