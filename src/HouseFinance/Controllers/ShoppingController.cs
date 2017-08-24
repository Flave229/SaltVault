using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SaltVault.Core.Bills;
using SaltVault.Core.Shopping;
using SaltVault.WebApp.Models;

namespace SaltVault.WebApp.Controllers
{
    public class ShoppingController : Controller
    {
        private readonly ShoppingRepository _shoppingRepository;
        private readonly BillRepository _billRepository;

        public ShoppingController()
        {
            _billRepository = new BillRepository();
            _shoppingRepository = new ShoppingRepository();
        }

        public ActionResult Index()
        {
            var shoppingList = _shoppingRepository.GetAllItems();

            return View(shoppingList);
        }

        public IActionResult AddItem()
        {
            var people = _billRepository.GetAllPeople();
            var shoppingModel = new ShoppingItemFormModel
            {
                Item = new ShoppingItem(),
                SelectedPeople = people.Select(x => new PersonForItem
                {
                    Person = x,
                    Selected = true
                }).ToList()
            };
            return View(shoppingModel);
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
            _shoppingRepository.AddItem(new AddShoppingItemRequest
            {
                ItemFor = itemForm.SelectedPeople.Where(x => x.Selected).Select(x => x.Person.Id).ToList(),
                Name = itemForm.Item.Name,
                AddedBy = itemForm.Item.AddedBy,
                Added = DateTime.Now
            });

            return RedirectToActionPermanent("Index", "Shopping");
        }

        public IActionResult CompleteItem(int itemId)
        {
            _shoppingRepository.UpdateItem(new UpdateShoppingItemRequestV2
            {
                Id = itemId,
                Purchased = true
            });

            return RedirectToActionPermanent("Index", "Shopping");
        }

        public IActionResult DeleteItem(int itemId)
        {
            _shoppingRepository.DeleteItem(itemId);

            return RedirectToActionPermanent("Index", "Shopping");
        }
    }
}