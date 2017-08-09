using System;
using HouseFinance.Core.Bills;
using HouseFinance.Core.Bills.Payments;
using HouseFinance.Core.FileManagement;
using HouseFinance.Core.People;
using HouseFinance.Models;
using Microsoft.AspNetCore.Mvc;

namespace HouseFinance.Controllers
{
    public class FinanceController : Controller
    {
        public IActionResult AddBill()
        {
            var billModel = new BillModel();
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

        public IActionResult Error()
        {
            return View();
        }

        public IActionResult EditBill(Guid billId)
        {
            var bill = new GenericFileHelper(FilePath.Bills).Get<Bill>(billId);
            var billModel = new BillModel
            {
                Bill = bill
            };

            var people = new GenericFileHelper(FilePath.People).GetAll<Person>();
            foreach (var person in people)
            {
                billModel.SelectedPeople.Add(new AddBillPerson
                {
                    Person = person,
                    Selected = bill.People.Contains(person.Id)
                });
            }

            return View(billModel);
        }

        public IActionResult BillDetails(Guid billId)
        {
            var billModel = BillDetailsBuilder.BuildBillDetails(billId);

            return View(billModel);
        }

        public IActionResult AddPayment(Guid billId)
        {
            var bill = new GenericFileHelper(FilePath.Bills).Get<Bill>(billId);
            var people = new GenericFileHelper(FilePath.People).Get<Person>(bill.People);
            var payment = new PaymentFormHelper
            {
                Bill = bill,
                People = people
            };

            return View(payment);
        }

        public IActionResult EditPayment(Guid billId, Guid paymentId)
        {
            var bill = new GenericFileHelper(FilePath.Bills).Get<Bill>(billId);
            var people = new GenericFileHelper(FilePath.People).Get<Person>(bill.People);
            var payment = new PaymentFormHelper
            {
                Bill = bill,
                People = people,
                Payment = new GenericFileHelper(FilePath.Payments).Get<Payment>(paymentId)
            };

            return View(payment);
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