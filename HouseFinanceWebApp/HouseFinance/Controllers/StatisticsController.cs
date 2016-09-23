using System.Collections.Generic;
using System.Web.Mvc;
using HouseFinance.Models;
using Services.FileIO;

namespace HouseFinance.Controllers
{
    public class StatisticsController : Controller
    {
        // GET: Statistics
        public ActionResult Index()
        {
            var statisticsList = new List<StatisticViewModel>();

            var people = PersonFileHelper.GetPeople();

            foreach (var person in people)
            {
                statisticsList.Add(new StatisticViewModel(person.Id));
            }

            return View(statisticsList);
        }
    }
}