using System.Collections.Generic;
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

        public IActionResult Error()
        {
            return View();
        }
    }
}