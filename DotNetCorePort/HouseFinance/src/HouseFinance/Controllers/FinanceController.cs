using System;
<<<<<<< HEAD
using Discord.WebSocket;
=======
>>>>>>> parent of 1f4bf10... Starting working in VS2017 and also updated all the third party libraries (Including some .NET Core stuff). There is a new class for Discord intergration that is yet to be added
using HouseFinance.Core.Bills;
using HouseFinance.Core.FileManagement;
using HouseFinance.Core.People;
using HouseFinance.Core.Services.Discord;
using HouseFinance.Models;
using Microsoft.AspNetCore.Mvc;

namespace HouseFinance.Controllers
{
    public class FinanceController : Controller
    {
<<<<<<< HEAD
        private readonly DiscordService _discordService;

        public FinanceController()
        {
            _discordService = new DiscordService(new DiscordSocketClient());
        }

=======
>>>>>>> parent of 1f4bf10... Starting working in VS2017 and also updated all the third party libraries (Including some .NET Core stuff). There is a new class for Discord intergration that is yet to be added
        public IActionResult AddBill()
        {
            var billModel = new AddBillModel();
            var people = new GenericFileHelper(FilePath.People).GetAll<Person>();

            foreach (var person in people)
            {
                billModel.SelectedPeople.Add(new AddBillPerson
                {
                    Person = person,
                    Selected = true
                });
            }

            return View(billModel);
        }

        [HttpPost]
        public IActionResult AddBill(AddBillModel addBillModel)
        {
            foreach (var person in addBillModel.SelectedPeople)
            {
                if (person.Selected)
                {
                    addBillModel.Bill.People.Add(person.Person.Id);
                }
            }

            BillValidator.CheckIfValidBill(addBillModel.Bill);

<<<<<<< HEAD
            _discordService.AddBillNotification(addBillModel.Bill.Name, addBillModel.Bill.Due, addBillModel.Bill.AmountOwed);
=======
            new GenericFileHelper(FilePath.Bills).AddOrUpdate<Bill>(addBillModel.Bill);
>>>>>>> parent of 1f4bf10... Starting working in VS2017 and also updated all the third party libraries (Including some .NET Core stuff). There is a new class for Discord intergration that is yet to be added

            return RedirectToActionPermanent("Index", "Home");
        }

        public IActionResult Error()
        {
            return View();
        }

        public IActionResult BillDetails(Guid billId)
        {
            var billModel = BillDetailsBuilder.BuildBillDetails(billId);

            return View(billModel);
        }

        public IActionResult AddPayment(Guid billId)
        {
            var payment = new PaymentFormHelper
            {
                BillId = billId
            };

            return View("AddPayment", payment);
        }

        [HttpPost]
        public ActionResult AddPayment(PaymentFormHelper formPayment)
        {
            var fileHelper = new GenericFileHelper(FilePath.Bills);

            try
            {
                var realBill = fileHelper.Get<Bill>(formPayment.BillId);

                BillHelper.AddOrUpdatePayment(ref realBill, formPayment.Payment);

                fileHelper.AddOrUpdate<Bill>(realBill);
            }
            catch (Exception ex)
            {
                TempData["Exception"] = ex.Message;

                return RedirectToActionPermanent("AddPayment", new { billId = formPayment.BillId });
            }

            return RedirectToActionPermanent("BillDetails", new { billId = formPayment.BillId, name = fileHelper.Get<Bill>(formPayment.BillId).Name });
        }

        public IActionResult DeleteBill(Guid billId)
        {
            new GenericFileHelper(FilePath.Bills).Delete<Bill>(billId);

            return RedirectToActionPermanent("Index", "Home");
        }

        public IActionResult AddPerson()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddPerson(AddPersonModel personModel)
        {
            var person = new Person
            {
                Id = Guid.NewGuid(),
                FirstName = personModel.FirstName,
                LastName = personModel.LastName,
                Image = personModel.ImageUrl
            };

            var fileHelper = new GenericFileHelper(FilePath.People);
            fileHelper.AddOrUpdate<Person>(person);

            return RedirectToActionPermanent("Index", "Home");
        }
    }
}