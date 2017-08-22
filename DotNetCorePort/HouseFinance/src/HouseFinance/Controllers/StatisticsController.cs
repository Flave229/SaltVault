using HouseFinance.Core.Bills;
using HouseFinance.Core.Shopping;
using HouseFinance.Core.Statistics;
using Microsoft.AspNetCore.Mvc;

namespace HouseFinance.Controllers
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