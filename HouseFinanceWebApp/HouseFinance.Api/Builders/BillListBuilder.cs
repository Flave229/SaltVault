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
                var people = new List<BillPeopleDetails>();

                foreach (var person in bill.People)
                {
                    var personDetails = new BillPeopleDetails
                    {
                        ImageLink = personFileHelper.Get<Person>(person).Image
                    };

                    personDetails.Paid = bill.AmountPaid.Any(payment => new GenericFileHelper(FilePath.People).Get<Payment>(payment).PersonId.Equals(person));
                }

                response.BillList.Add(new BillDetails
                {
                    Name = bill.Name,
                    AmountDue = BillHelper.GetHowMuchToPay(bill),
                    Overdue = BillHelper.CheckIfBillOverdue(bill),
                    Paid = BillHelper.CheckIfBillPaid(bill),
                    DateDue = bill.Due.ToString("yyyy-MM-dd"),
                    FullDateDue = bill.Due,
                    People = people
                });
            }

            response.BillList = response.BillList.OrderByDescending(x => x.DateDue).ToList();

            return response;
        }
    }
}
