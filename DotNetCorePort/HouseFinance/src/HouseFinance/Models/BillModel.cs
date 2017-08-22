using System.Collections.Generic;
using HouseFinance.Core.Bills;
using HouseFinance.Core.People;

namespace HouseFinance.Models
{
    public class BillModel
    {
        public BillDetails Bill { get; set; }
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