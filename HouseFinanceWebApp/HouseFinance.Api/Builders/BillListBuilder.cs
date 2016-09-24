using System.Linq;
using HouseFinance.Api.Models;
using Services.FileIO;
using Services.FormHelpers;
using Services.Models.FinanceModels;
using Services.Models.GlobalModels;

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
                var billDetails = new BillDetails();

                foreach (var person in bill.People)
                {
                    var imageLink = new Images
                    {
                        Link = personFileHelper.Get<Person>(person).Image
                    };

                    billDetails.PeopleImages.Add(imageLink);
                }

                response.BillList.Add(new BillDetails
                {
                    Name = bill.Name,
                    AmountDue = BillHelper.GetHowMuchToPay(bill),
                    DateDue = bill.Due
                });
            }

            response.BillList = response.BillList.OrderByDescending(x => x.DateDue).ToList();

            return response;
        }
    }
}
