using System.Collections.Generic;
using SaltVault.Core.People;
using SaltVault.Core.Shopping;

namespace SaltVault.WebApp.Models
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
