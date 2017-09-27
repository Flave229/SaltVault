using Microsoft.AspNetCore.Mvc;
using SaltVault.Core.Bills;
using SaltVault.Core.People;
using SaltVault.Core.Shopping;
using SaltVault.Core.Statistics;

namespace SaltVault.WebApp.Controllers
{
    public class StatisticsController : Controller
    {
        private readonly StatisticService _statisticService;

        public StatisticsController(IBillRepository billRepository, IShoppingRepository shoppingRepository, IPeopleRepository peopleRepository)
        {
            _statisticService = new StatisticService(billRepository, shoppingRepository, peopleRepository);
        }

        public IActionResult Index()
        {
            var statisticOverview = _statisticService.GetAllStatistics();
            return View(statisticOverview);
        }
    }
}