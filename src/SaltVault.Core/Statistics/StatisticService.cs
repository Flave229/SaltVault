using System;
using System.Collections.Generic;
using System.Linq;
using SaltVault.Core.Bills;
using SaltVault.Core.People;
using SaltVault.Core.Shopping;
using SaltVault.Core.Shopping.Models;

namespace SaltVault.Core.Statistics
{
    public class StatisticService
    {
        private readonly IBillRepository _billRepository;
        private readonly IShoppingRepository _shoppingRepository;
        private readonly IPeopleRepository _peopleRepository;

        public StatisticService(IBillRepository billRepository, IShoppingRepository shoppingRepository, IPeopleRepository peopleRepository)
        {
            _billRepository = billRepository;
            _shoppingRepository = shoppingRepository;
            _peopleRepository = peopleRepository;
        }

        public StatisticsOverview GetAllStatistics()
        {
            return null;
            //var pagination = new Pagination
            //{
            //    Page = 0,
            //    ResultsPerPage = int.MaxValue
            //};
            //var bills = _billRepository.GetAllBasicBillDetails(pagination);
            //var shoppingItems = _shoppingRepository.GetAllItems(pagination);
            //var people = _peopleRepository.GetAllPeople();
            //var statisticsOverview = new StatisticsOverview();

            //foreach (var person in people)
            //{
            //    var billsForPerson = bills.Where(x => x.People.Any(y => y.Id == person.Id)).ToList();
            //    var shoppingItemsForPerson = shoppingItems.Where(x => x.AddedFor.Any(y => y.Id == person.Id)).ToList();
            //    var shoppingItemsByPerson = shoppingItems.Where(x => x.AddedBy.Id == person.Id).ToList();
            //    var statistics = new Statistics
            //    {
            //        Person = person,
            //        BillsForPerson = billsForPerson,
            //        ShoppingItemsForPerson = shoppingItemsForPerson,
            //        ShoppingItemsByPerson = shoppingItemsByPerson
            //    };

            //    var sumOfDaysToPayBill = 0;
            //    var sumOfPercentageShare = 0.0;
            //    var billsPaidFor = 0;

            //    foreach (var bill in billsForPerson)
            //    {
            //        var latestPayment = new DateTime();
            //        var percentageOfBill = 0.0;
            //        var billPaidFor = false;

            //        foreach (var payment in bill.Payments)
            //        {
            //            if (payment.PersonId != person.Id)
            //                continue;

            //            billPaidFor = true;

            //            percentageOfBill += (double)(payment.Amount / bill.TotalAmount);

            //            if (payment.DatePaid > latestPayment)
            //            {
            //                latestPayment = payment.DatePaid;
            //            }
            //        }

            //        sumOfPercentageShare += percentageOfBill;

            //        if (billPaidFor)
            //            billsPaidFor += 1;

            //        if (latestPayment != new DateTime())
            //        {
            //            var datediff = bill.FullDateDue - latestPayment;

            //            sumOfDaysToPayBill += datediff.Days;
            //        }
            //        else if (bill.FullDateDue <= DateTime.Now)
            //        {
            //            var datediff = bill.FullDateDue - DateTime.Now;

            //            sumOfDaysToPayBill += datediff.Days;
            //        }
            //    }

            //    if (billsForPerson.Count != 0)
            //    {
            //        statistics.AverageDaysToPayBill = sumOfDaysToPayBill / billsForPerson.Count;
            //        statistics.AveragePercentageShareOfBill = (sumOfPercentageShare / billsForPerson.Count) * 100;
            //    }
            //    else
            //    {
            //        statistics.AverageDaysToPayBill = 0;
            //        statistics.AveragePercentageShareOfBill = 0;
            //    }

            //    statistics.AmountOfBillsPaid = billsPaidFor;

            //    statisticsOverview.Statistics.Add(statistics);
            //}

            //return statisticsOverview;
        }
    }

    public class StatisticsOverview
    {
        public List<Statistics> Statistics { get; set; }

        public StatisticsOverview()
        {
            Statistics = new List<Statistics>();
        }
    }

    public class Statistics
    {
        public Person Person { get; set; }
        public List<Bill> BillsForPerson { get; set; }
        public List<Item> ShoppingItemsForPerson { get; set; }
        public List<Item> ShoppingItemsByPerson { get; set; }

        public int AverageDaysToPayBill { get; set; }
        public double AveragePercentageShareOfBill { get; set; }
        public int AmountOfBillsPaid { get; set; }
    }
}
