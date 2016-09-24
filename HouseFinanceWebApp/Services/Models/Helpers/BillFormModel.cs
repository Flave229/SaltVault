using System.Collections.Generic;
using Services.FileIO;
using Services.Models.FinanceModels;
using Services.Models.GlobalModels;

namespace Services.Models.Helpers
{
    public class BillFormModel
    {
        public Bill Bill { get; set; }
        public List<PersonForBill> SelectedPeople { get; set; }

        public BillFormModel()
        {
            Bill = new Bill();
            SelectedPeople = new List<PersonForBill>();

            var people = new GenericFileHelper(FilePath.People).GetAll<Person>();

            foreach (var person in people)
            {
                SelectedPeople.Add(new PersonForBill(person));
            }
        }
    }

    public class PersonForBill
    {
        public Person Person { get; set; }
        public bool Selected { get; set; }

        public PersonForBill()
        {
            Person = new Person();
            Selected = true;
        }

        public PersonForBill(Person person)
        {
            Person = person;
            Selected = true;
        }
    }
}
