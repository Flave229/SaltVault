using System;
using System.Linq;
using System.Web.Mvc;
using HouseFinance.Models;
using Services.FileIO;
using Services.FormHelpers;

namespace HouseFinance.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            var test = new StatisticViewModel(PersonFileHelper.GetPerson("David").Id);

            var bills = BillFileHelper.GetBills();

            for (var i = 0; i < bills.Count; i++)
            {
                BillHelper.CheckRecurring(bills[i]);

                if (BillHelper.CheckIfBillPaid(bills[i]) && (bills[i].Due < DateTime.Now.AddMonths(-6)))
                {
                    bills.Remove(bills[i]);
                    i--;
                }
            }

            var orderedbills = bills.OrderByDescending(x => x.Due).ToList();

            if (TempData.ContainsKey("Exception"))
            {
                ViewBag.ExceptionMessage = TempData["Exception"];
            }

            return View(orderedbills);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return RedirectToActionPermanent("Index", "Home");
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return RedirectToActionPermanent("Index", "Home");
        }
    }
}