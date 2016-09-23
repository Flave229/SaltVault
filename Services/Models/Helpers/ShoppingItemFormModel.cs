using System.Collections.Generic;
using Services.FileIO;
using Services.Models.GlobalModels;
using Services.Models.ShoppingModels;

namespace Services.Models.Helpers
{
    public class ShoppingItemFormModel
    {
        public ShoppingItem Item { get; set; }
        public List<PersonForItem> SelectedPeople { get; set; }

        public ShoppingItemFormModel()
        {
            Item = new ShoppingItem();
            SelectedPeople = new List<PersonForItem>();

            var people = PersonFileHelper.GetPeople();

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