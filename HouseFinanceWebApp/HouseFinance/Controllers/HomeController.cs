using System.Web.Mvc;
using HouseFinance.Api.Builders;

namespace HouseFinance.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            var billModel = BillListBuilder.BuildBillList();

            if (TempData.ContainsKey("Exception"))
            {
                ViewBag.ExceptionMessage = TempData["Exception"];
            }

            return View(billModel);
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