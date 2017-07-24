using System;
using HouseFinance.Core.Bills;
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

            new GenericFileHelper(FilePath.Bills).AddOrUpdate<Bill>(addBillModel.Bill);

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
    }
}