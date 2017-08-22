using System.Collections.Generic;
using HouseFinance.Core.People;
using HouseFinance.Core.Shopping;

namespace HouseFinance.Models
{
    public class ShoppingItemFormModel
    {
        public ShoppingItem Item { get; set; }
        public List<PersonForItem> SelectedPeople { get; set; }
    }

    public class PersonForItem
    {
        public Person Person { get; set; }
        public bool Selected { get; set; }
    }
}
