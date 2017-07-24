using System;
using HouseFinance.Core.FileManagement;
using HouseFinance.Core.Shopping;
using HouseFinance.Models;
using Microsoft.AspNetCore.Mvc;

namespace HouseFinance.Controllers
{
    public class ShoppingController : Controller
    {
        public ActionResult Index()
        {
            var shoppingList = ShoppingListBuilder.BuildShoppingList();

            return View(shoppingList);
        }

        public IActionResult AddItem()
        {
            return View(new ShoppingItemFormModel());
        }

        [HttpPost]
        public ActionResult AddItem(ShoppingItemFormModel itemForm)
        {
            foreach (var person in itemForm.SelectedPeople)
            {
                if (person.Selected)
                    itemForm.Item.ItemFor.Add(person.Person.Id);
            }

            ShoppingValidator.CheckIfValidItem(itemForm.Item);
            new GenericFileHelper(FilePath.Shopping).AddOrUpdate<ShoppingItem>(itemForm.Item);

            return RedirectToActionPermanent("Index", "Shopping");
        }

        public IActionResult CompleteItem(Guid itemId)
        {
            var fileHelper = new GenericFileHelper(FilePath.Shopping);
            var shoppingItem = fileHelper.Get<ShoppingItem>(itemId);

            shoppingItem.Purchased = true;

            fileHelper.AddOrUpdate<ShoppingItem>(shoppingItem);

            return RedirectToActionPermanent("Index", "Shopping");
        }

        public IActionResult DeleteItem(Guid itemId)
        {
            var fileHelper = new GenericFileHelper(FilePath.Shopping);
            fileHelper.Delete<ShoppingItem>(itemId);

            return RedirectToActionPermanent("Index", "Shopping");
        }
    }
}