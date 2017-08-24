using Microsoft.AspNetCore.Mvc;
using SaltVault.Core.Bills;
using SaltVault.Core.Shopping;
using SaltVault.Core.Statistics;

namespace SaltVault.WebApp.Controllers
{
    public class StatisticsController : Controller
    {
        private readonly StatisticService _statisticService;

        public StatisticsController()
        {
            _statisticService = new StatisticService(new BillRepository(), new ShoppingRepository());
        }

        public IActionResult Index()
        {
            var statisticOverview = _statisticService.GetAllStatistics();
            return View(statisticOverview);
        }
    }
}