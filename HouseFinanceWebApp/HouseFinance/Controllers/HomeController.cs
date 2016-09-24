using System;
using System.Linq;
using System.Web.Mvc;
using Services.FileIO;
using Services.FormHelpers;
using Services.Models.FinanceModels;

namespace HouseFinance.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            var billFileHelper = new BillFileHelper();
            var bills = billFileHelper.GetAll().Cast<Bill>().ToList();

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