using System.Collections.Generic;
using HouseFinance.Core.FileManagement;
using HouseFinance.Core.People;
using HouseFinance.Core.Shopping;

namespace HouseFinance.Models
{
    public class ShoppingItemFormModel
    {
        public ShoppingItemV2 Item { get; set; }
        public List<PersonForItem> SelectedPeople { get; set; }

        public ShoppingItemFormModel()
        {
            Item = new ShoppingItemV2();
            SelectedPeople = new List<PersonForItem>();

            var people = new GenericFileHelper(FilePath.People).GetAll<Person>();

            foreach (var person in people)
            {
                SelectedPeople.Add(new PersonForItem(person));
            }
        }
    }

    public class PersonForItem
    {
        public Person Person { get; set; }
        public bool Selected { get; set; }

        public PersonForItem()
        {
            Person = new Person();
            Selected = false;
        }

        public PersonForItem(Person person)
        {
            Person = person;
            Selected = true;
        }
    }
}
