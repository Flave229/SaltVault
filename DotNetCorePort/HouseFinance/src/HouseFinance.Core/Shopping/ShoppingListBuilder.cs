using System.Collections.Generic;
using HouseFinance.Core.FileManagement;
using HouseFinance.Core.People;
using System.Linq;

namespace HouseFinance.Core.Shopping
{
    public static class ShoppingListBuilder
    {
        public static ShoppingListResponse BuildShoppingList()
        {
            var response = new ShoppingListResponse();
            var items = new GenericFileHelper(FilePath.Shopping).GetAll<ShoppingItem>();
            var personFileHelper = new GenericFileHelper(FilePath.People);

            foreach (var item in items)
            {
                var images = new List<string>();

                foreach (var person in item.ItemFor)
                {
                    images.Add(personFileHelper.Get<Person>(person).Image);
                }

                response.ShoppingList.Add(new Item
                {
                    Id = item.Id,
                    Name = item.Name,
                    AddedByImage = personFileHelper.Get<Person>(item.AddedBy).Image,
                    AddedForImages = images,
                    FullAddedOn = item.Added,
                    AddedOn = item.Added.ToString("yyyy-MM-dd"),
                    Purchased = item.Purchased
                });
            }

            response.ShoppingList = response.ShoppingList.OrderBy(x => x.Purchased).ThenByDescending(x => x.AddedOn).ToList();

            return response;
        }
    }
}
