using System;
using System.Collections.Generic;
using System.Linq;
using Services.FileIO;
using Services.Models.FinanceModels;
using Services.Models.GlobalModels;
using Services.Models.ShoppingModels;

namespace HouseFinance.Models
{
    public class StatisticViewModel
    {
        public Person Person { get; set; }
        public List<Bill> BillsForPerson { get; set; }
        public List<ShoppingItem> ShoppingItemsForPerson { get; set; }
        public List<ShoppingItem> ShoppingItemsByPerson { get; set; }

        public int AverageDaysToPayBill { get; set; }
        public double AveragePercentageShareOfBill { get; set; }
        public int AmountOfBillsPaid { get; set; }

        public StatisticViewModel(Guid personId)
        {
            ConstructPersonInformation(personId);
            ConstructBillInformation();
            ConstructShoppingInformation();

            CalculateBillAverages();
        }

        private void ConstructPersonInformation(Guid personId)
        {
            try
            {
                Person = PersonFileHelper.GetPerson(personId);
            }
            catch (Exception exception)
            {
                throw new Exception("Could not retrieve the person with ID {" + Person.Id + "}."
                    + exception.Message, exception);
            }
        }

        private void ConstructBillInformation()
        {
            try
            {
                var billFileHelper = new GenericFileHelper(FilePath.Bills);
                var allBills = billFileHelper.GetAll<Bill>();

                BillsForPerson = allBills.Where(x => x.People.Contains(Person.Id)).ToList();
            }
            catch (Exception exception)
            {
                throw new Exception("Could not construct the Bill data for the person " + Person.FirstName + " " + Person.LastName + " {" + Person.Id + "}."
                    + exception.Message, exception);
            }
        }

        private void ConstructShoppingInformation()
        {
            try
            {
                var fileHelper = new GenericFileHelper(FilePath.Shopping);
                var allShoppingItems = fileHelper.GetAll<ShoppingItem>().ToList();
                ShoppingItemsForPerson = allShoppingItems.Where(x => x.ItemFor.Contains(Person.Id)).ToList();

                ShoppingItemsByPerson = allShoppingItems.Where(x => x.AddedBy.Equals(Person.Id)).ToList();
            }
            catch (Exception exception)
            {
                throw new Exception("Could not construct the Shopping data for the person " + Person.FirstName + " " + Person.LastName + " {" + Person.Id + "}." 
                    + exception.Message, exception);
            }
        }

        private void CalculateBillAverages()
        {
            var sumOfDaysToPayBill = 0;
            var sumOfPercentageShare = 0.0;
            var billsPaidFor = 0;
            var allPayments = PaymentFileHelper.GetPayments();

            foreach (var bill in BillsForPerson)
            {
                var latestPayment = new DateTime();
                var percentageOfBill = 0.0;
                var billPaidFor = false;

                foreach (var paymentId in bill.AmountPaid)
                {
                    foreach (var payment in allPayments)
                    {
                        if (!payment.Id.Equals(paymentId)) continue;
                        if (!payment.PersonId.Equals(Person.Id)) continue;

                        billPaidFor = true;

                        percentageOfBill += ((double)payment.Amount / (double)bill.AmountOwed);

                        if (payment.Created > latestPayment)
                        {
                            latestPayment = payment.Created;
                        }
                    }
                }

                sumOfPercentageShare += percentageOfBill;

                if (billPaidFor)
                    billsPaidFor += 1;

                if (latestPayment != new DateTime())
                {
                    var datediff = bill.Due - latestPayment;

                    sumOfDaysToPayBill += datediff.Days;
                }
                else if (bill.Due <= DateTime.Now)
                {
                    var datediff = bill.Due - DateTime.Now;

                    sumOfDaysToPayBill += datediff.Days;
                }
            }

            if (BillsForPerson.Count != 0)
            {
                AverageDaysToPayBill = sumOfDaysToPayBill / BillsForPerson.Count;
                AveragePercentageShareOfBill = (sumOfPercentageShare / BillsForPerson.Count) * 100;
            }
            else
            {
                AverageDaysToPayBill = 0;
                AveragePercentageShareOfBill = 0;
            }

            AmountOfBillsPaid = billsPaidFor;
        }
    }
}