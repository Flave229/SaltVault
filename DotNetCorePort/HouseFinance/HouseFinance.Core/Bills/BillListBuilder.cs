using System.Collections.Generic;
using HouseFinance.Core.FileManagement;
using HouseFinance.Core.People;
using System.Linq;
using HouseFinance.Core.Bills.Payments;

namespace HouseFinance.Core.Bills
{
    public class BillListBuilder
    {
        public static List<BillOverview> BuildBillList()
        {
            var billOverviews = new List<BillOverview>();
            var bills = new GenericFileHelper(FilePath.Bills).GetAll<Bill>();
            var personFileHelper = new GenericFileHelper(FilePath.People);

            foreach (var bill in bills)
            {
                var people = new List<PersonBillDetails>();

                foreach (var person in bill.People)
                {
                    people.Add(new PersonBillDetails
                    {
                        ImageLink = personFileHelper.Get<Person>(person).Image,
                        Paid = bill.AmountPaid.Any(payment => new GenericFileHelper(FilePath.Payments).Get<Payment>(payment).PersonId.Equals(person))
                    });
                }

                BillHelper.CheckRecurring(bill);

                billOverviews.Add(new BillOverview
                {
                    Id = bill.Id,
                    Name = bill.Name,
                    TotalAmount = bill.AmountOwed,
                    AmountDue = BillHelper.GetHowMuchToPay(bill),
                    Overdue = BillHelper.CheckIfBillOverdue(bill),
                    Paid = BillHelper.CheckIfBillPaid(bill),
                    DateDue = bill.Due.ToString("yyyy-MM-dd"),
                    FullDateDue = bill.Due,
                    People = people
                });
            }

            return billOverviews.OrderByDescending(x => x.DateDue).ToList();
        }
    }
}
