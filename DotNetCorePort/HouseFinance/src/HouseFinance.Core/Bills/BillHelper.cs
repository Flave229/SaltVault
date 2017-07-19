using System;
using System.Collections.Generic;
using System.Linq;
using HouseFinance.Core.Bills.Payments;
using HouseFinance.Core.FileManagement;

namespace HouseFinance.Core.Bills
{
    public class BillHelper
    {
        public static void AddOrUpdatePayment(ref Bill bill, Payment payment)
        {
            try
            {
                BillValidator.CheckIfValidBill(bill);
                PaymentValidator.CheckIfValidPayment(payment);

                if (bill.AmountPaid.Any(existingPayment => existingPayment.Equals(payment.Id)))
                {
                    UpdatePayment(ref bill, payment);
                }
                else
                {
                    AddPayment(ref bill, payment);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An Error occured Adding/Updating a Payment. " + ex.Message, ex);
            }
        }

        public static decimal GetTotalAmountPaid(Bill bill)
        {
            var payments = new GenericFileHelper(FilePath.Payments).GetAll<Payment>();
            var paymentsForBill = bill.AmountPaid.Select(paymentId => payments.FirstOrDefault(payment => payment.Id.Equals(paymentId))).ToList();
            return paymentsForBill.Sum(payment => payment.Amount);
        }

        public static decimal GetHowMuchToPay(Bill bill)
        {
            return bill.AmountOwed - GetTotalAmountPaid(bill);
        }

        public static bool CheckIfBillOverdue(Bill bill)
        {
            return bill.Due < DateTime.Now && !CheckIfBillPaid(bill);
        }

        public static bool CheckIfBillPaid(Bill bill)
        {
            return GetTotalAmountPaid(bill) >= bill.AmountOwed;
        }

        public static bool CheckIfBillPaid(Bill bill, decimal additionalPayment)
        {
            return GetTotalAmountPaid(bill) + additionalPayment >= bill.AmountOwed;
        }

        public static void CheckRecurring(Bill bill)
        {
            if (bill.RecurringType == RecurringType.Monthly)
                CheckMonthlyBillForUpdate(bill);
        }

        private static void AddPayment(ref Bill bill, Payment payment)
        {
            if (CheckIfBillPaid(bill))
                throw new Exception("The bill is already paid, cannot add new payment");

            if (CheckIfBillPaid(bill, payment.Amount))
                payment.Amount = GetHowMuchToPay(bill);

            new GenericFileHelper(FilePath.Payments).AddOrUpdate<Payment>(payment);
            bill.AmountPaid.Add(payment.Id);
        }

        private static void UpdatePayment(ref Bill bill, Payment payment)
        {
            for (var i = 0; i < bill.AmountPaid.Count; i++)
            {
                if (bill.AmountPaid.ElementAt(i) != payment.Id) continue;

                var fileHelper = new GenericFileHelper(FilePath.Payments);
                var updatedPayment = fileHelper.Get<Payment>(payment.Id);

                bill.AmountPaid[i] = payment.Id;

                if (CheckIfBillPaid(bill))
                {
                    updatedPayment.Amount += GetHowMuchToPay(bill);
                }

                fileHelper.AddOrUpdate<Payment>(payment);

                break;
            }
        }

        private static void CheckMonthlyBillForUpdate(Bill bill)
        {
            if (bill.Due <= DateTime.Now)
                CreateNewMonthlyBill(bill);
        }

        private static void CreateNewMonthlyBill(Bill bill)
        {
            bill.RecurringType = RecurringType.None;

            var nextMonthBill = new Bill()
            {
                AmountOwed = bill.AmountOwed,
                AmountPaid = new List<Guid>(),
                Due = bill.Due.AddMonths(1),
                Id = Guid.NewGuid(),
                Name = bill.Name,
                People = bill.People,
                RecurringType = RecurringType.Monthly
            };

            var fileHelper = new GenericFileHelper(FilePath.Bills);

            fileHelper.AddOrUpdate<Bill>(nextMonthBill);
            fileHelper.AddOrUpdate<Bill>(bill);
        }
    }
}
