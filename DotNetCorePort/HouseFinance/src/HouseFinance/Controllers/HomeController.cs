using HouseFinance.Core.Bills;
using Microsoft.AspNetCore.Mvc;

namespace HouseFinance.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var billModel = BillListBuilder.BuildBillList();

            return View("Index", billModel);
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
