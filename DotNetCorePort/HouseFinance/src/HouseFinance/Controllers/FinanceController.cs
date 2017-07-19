using HouseFinance.Models;
using Microsoft.AspNetCore.Mvc;

namespace HouseFinance.Controllers
{
    public class FinanceController : Controller
    {
        public IActionResult AddBill()
        {
            return View(new AddBillModel());
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
