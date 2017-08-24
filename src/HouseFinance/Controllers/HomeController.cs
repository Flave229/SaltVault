using Microsoft.AspNetCore.Mvc;

namespace SaltVault.WebApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View("Index");
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
