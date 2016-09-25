using System.Linq;
using HouseFinance.Api.Models;
using Services.FileIO;
using Services.FormHelpers;
using Services.Models.FinanceModels;
using Services.Models.GlobalModels;
using System.Collections.Generic;

namespace HouseFinance.Api.Builders
{
    public static class BillListBuilder
    {
        public static BillListResponse BuildBillList()
        {
            var response = new BillListResponse();
            var bills = new GenericFileHelper(FilePath.Bills).GetAll<Bill>();
            var personFileHelper = new GenericFileHelper(FilePath.People);

            foreach(var bill in bills)
            {
                var images = new List<string>();

                foreach (var person in bill.People)
                {
                    images.Add(personFileHelper.Get<Person>(person).Image);
                }

                response.BillList.Add(new BillDetails
                {
                    Name = bill.Name,
                    AmountDue = BillHelper.GetHowMuchToPay(bill),
                    Overdue = BillHelper.CheckIfBillOverdue(bill),
                    Paid = BillHelper.CheckIfBillPaid(bill),
                    DateDue = bill.Due,
                    PeopleImages = images
                });
            }

            response.BillList = response.BillList.OrderByDescending(x => x.DateDue).ToList();

            return response;
        }
    }
}
