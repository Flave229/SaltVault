using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SaltVault.Core.Bills;
using SaltVault.WebApp.Models;

namespace SaltVault.WebApp.Controllers
{
    public class FinanceController : Controller
    {
        private readonly BillRepository _billRepository;

        public FinanceController()
        {
            _billRepository = new BillRepository();
        }

        public IActionResult AddBill()
        {
            var billModel = new BillModel();
            var people = _billRepository.GetAllPeople();

            foreach (var person in people)
            {
                billModel.SelectedPeople.Add(new PersonModel
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

        public IActionResult EditBill(int billId)
        {
            var bill = _billRepository.GetBasicBillDetails(billId);
            var billModel = new BillModel
            {
                Bill = bill
            };

            var people = _billRepository.GetAllPeople();
            foreach (var person in people)
            {
                billModel.SelectedPeople.Add(new PersonModel
                {
                    Person = person,
                    Selected = bill.People.Any(x => x.Id == person.Id)
                });
            }

            return View(billModel);
        }

        public IActionResult BillDetails(int billId)
        {
            var billDetails = _billRepository.GetBasicBillDetails(billId);

            return View(billDetails);
        }

        public IActionResult AddPayment(int billId)
        {
            var bill = _billRepository.GetBasicBillDetails(billId);
            var people = _billRepository.GetPeople(bill.People.Select(x => x.Id).ToList());
            var payment = new PaymentFormHelper
            {
                Bill = bill,
                People = people
            };

            return View(payment);
        }

        public IActionResult EditPayment(int billId, int paymentId)
        {
            var bill = _billRepository.GetBasicBillDetails(billId);
            var people = _billRepository.GetPeople(bill.People.Select(x => x.Id).ToList());
            var payment = new PaymentFormHelper
            {
                Bill = bill,
                People = people,
                Payment = _billRepository.GetPayment(paymentId)
            };

            return View(payment);
        }
    }
}