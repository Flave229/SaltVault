using HouseFinance.Api.Models;
using Services.FileIO;
using Services.FormHelpers;
using Services.Models.FinanceModels;
using Services.Models.GlobalModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HouseFinance.Api.Builders
{
    public static class BillDetailsBuilder
    {
        public static BillDetailsResponse BuildBillDetails(Guid id)
        {
            var bill = new GenericFileHelper(FilePath.Bills).Get<Bill>(id);
            var personFileHelper = new GenericFileHelper(FilePath.People);
            var paymentFileHelper = new GenericFileHelper(FilePath.Payments);

            var payments = new List<BillPayments>();

            foreach (var payment in bill.AmountPaid)
            {
                var paymentFromFile = paymentFileHelper.Get<Payment>(payment);
                var person = personFileHelper.Get<Person>(paymentFromFile.PersonId);

                payments.Add(new BillPayments
                {
                    Id = paymentFromFile.Id,
                    PersonName = person.FirstName + " " + person.LastName,
                    DatePaid = paymentFromFile.Created.ToString("dd/MM/yyyy"),
                    AmountPaid = paymentFromFile.Amount
                });
            }

            return new BillDetailsResponse
            {
                Id = bill.Id,
                Name = bill.Name + " : " + bill.Due.ToString("MMMM yyyy"),
                AmountDue = bill.AmountOwed,
                AmountPaid = BillHelper.GetTotalAmountPaid(bill),
                FullDateDue = bill.Due,
                DateDue = bill.Due.ToString("dd/MM/yyyy"),
                Payments = payments.OrderBy(x => x.DatePaid).ToList()
            };
        }
    }
}
