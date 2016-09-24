using System;
using System.Linq;
using System.Web.Mvc;
using Services.FileIO;
using Services.Models.Helpers;
using Services.Models.ShoppingModels;

namespace HouseFinance.Controllers
{
    public class ShoppingController : Controller
    {
        // GET: Shopping
        public ActionResult Index()
        {
            var shoppingList = new GenericFileHelper(FilePath.Shopping).GetAll<ShoppingItem>();

            var orderedItems = shoppingList.OrderBy(x => x.Purchased).ThenByDescending(x => x.Added).ToList();

            return View(orderedItems);
        }
        
        // GET: Finance/AddItem
        public ActionResult AddItem()
        {
            if (TempData.ContainsKey("Exception"))
            {
                ViewBag.ExceptionMessage = TempData["Exception"];
            }

            return View(new ShoppingItemFormModel());
        }

        // POST: Finance/AddItem
        [HttpPost]
        public ActionResult AddItem(ShoppingItemFormModel itemForm)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ExceptionMessage = "The Data entered was Invalid.";

                return View(itemForm);
            }

            try
            {
                foreach (var person in itemForm.SelectedPeople)
                {
                    if (person.Selected)
                    {
                        itemForm.Item.ItemFor.Add(person.Person.Id);
                    }
                }
                
                new GenericFileHelper(FilePath.Shopping).AddOrUpdate<ShoppingItem>(itemForm.Item);
            }
            catch (Exception exception)
            {
                TempData["Exception"] = exception.Message;

                return RedirectToActionPermanent("AddItem");
            }

            return RedirectToActionPermanent("Index", "Shopping");
        }

        public ActionResult CompleteItem(Guid itemId)
        {
            try
            {
                var fileHelper = new GenericFileHelper(FilePath.Shopping);
                var shoppingItem = fileHelper.Get<ShoppingItem>(itemId);

                shoppingItem.Purchased = true;

                fileHelper.AddOrUpdate<ShoppingItem>(shoppingItem);
            }
            catch (Exception exception)
            {
                TempData["Exception"] = exception.Message;

                return RedirectToActionPermanent("Index", "Shopping");
            }

            return RedirectToActionPermanent("Index", "Shopping");
        }

        public ActionResult DeleteItem(Guid itemId)
        {
            try
            {
                var fileHelper = new GenericFileHelper(FilePath.Shopping);
                fileHelper.Delete<ShoppingItem>(itemId);
            }
            catch (Exception exception)
            {
                TempData["Exception"] = exception.Message;

                return RedirectToActionPermanent("Index", "Shopping");
            }

            return RedirectToActionPermanent("Index", "Shopping");
        }
    }
}