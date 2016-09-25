using System.Linq;
using HouseFinance.Api.Models;
using Services.FileIO;
using Services.Models.ShoppingModels;
using Services.Models.GlobalModels;
using System.Collections.Generic;

namespace HouseFinance.Api.Builders
{
    public static class ShoppingListBuilder
    {
        public static ShoppingListResponse BuildShoppingList()
        {
            var response = new ShoppingListResponse();
            var items = new GenericFileHelper(FilePath.Shopping).GetAll<ShoppingItem>();
            var personFileHelper = new GenericFileHelper(FilePath.People);

            foreach(var item in items)
            {
                var images = new List<string>();

                foreach (var person in item.ItemFor)
                {
                    images.Add(personFileHelper.Get<Person>(person).Image);
                }

                response.ShoppingList.Add(new Item
                {
                    Name = item.Name,
                    AddedByImage = personFileHelper.Get<Person>(item.AddedBy).Image,
                    AddedForImages = images,
                    AddedOn = item.Added.ToString("yyyy-MM-dd"),
                    Purchased = item.Purchased
                });
            }

            response.ShoppingList = response.ShoppingList.OrderBy(x => x.Purchased).ThenByDescending(x => x.AddedOn).ToList();

            return response;
        }
    }
}
