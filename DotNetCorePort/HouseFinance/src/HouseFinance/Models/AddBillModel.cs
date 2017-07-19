using System.Collections.Generic;
using HouseFinance.Core.Bills;
using HouseFinance.Core.People;

namespace HouseFinance.Models
{
    public class AddBillModel
    {
        public Bill Bill { get; set; }
        public List<AddBillPerson> SelectedPeople { get; set; }

        public AddBillModel()
        {
            SelectedPeople = new List<AddBillPerson>();
        }
    }

    public class AddBillPerson
    {
        public Person Person { get; set; }
        public bool Selected { get; set; }
    }
}