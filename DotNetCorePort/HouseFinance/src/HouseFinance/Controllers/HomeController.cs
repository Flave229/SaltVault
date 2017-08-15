using HouseFinance.Core.Bills;
using Microsoft.AspNetCore.Mvc;

namespace HouseFinance.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var repo = new BillRepository();
            var test = repo.GetAllBasicBillDetails();
            return View("Index");
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
