using System;
using Services.Models.FinanceModels;
using Services.Validation;

namespace Services.FormHelpers
{
    public static class BillValidator
    {
        private static readonly ValidationService Validation = new ValidationService();

        public static void CheckIfValidBill(Bill bill)
        {
            try
            {
                if (bill == null) throw new Exception("The bill object given was null.");
                if (bill.People.Count == 0) throw new Exception("No people were assigned to the bill.");

                foreach (var person in bill.People)
                {
                    if (person == null) throw new Exception("A person assigned to the bill was null.");
                }

                foreach (var payment in bill.AmountPaid)
                {
                    if (payment == null) throw new Exception("A payment assigned to the bill was null.");
                }

                if (!Validation.CheckGuidValid(bill.Id)) throw new Exception("The Id for the payment object was invalid.");

                CheckNameValid(bill.Name);
                CheckAmountValid(bill.AmountOwed);
                CheckDateValid(bill.Due);
            }
            catch (Exception ex)
            {
                throw new Exception("The payment object cannot be validated: " + ex.Message, ex);
            }
        }

        public static void CheckNameValid(string name)
        {
            var minAmount = 3;
            var maxAmount = 50;

            if (!Validation.CheckStringWithinLengthRange(minAmount, maxAmount, name))
                throw new Exception("The name entered for the bill was out of range. Name length must lie between " + minAmount + " and " + maxAmount + " characters.");

            if (!Validation.CheckStringOnlyLetters(name))
                throw new Exception("The name entered for the bill contains invalid characters.");
        }

        public static void CheckAmountValid(decimal amount)
        {
            var minAmount = 0.01m;
            var maxAmount = 1000000;

            if (!Validation.CheckDecimalWithinSizeRange(minAmount, maxAmount, amount))
                throw new Exception("The amount entered was out of range. Value must lie between " + minAmount + " and " + maxAmount + ".");
        }

        public static void CheckDateValid(DateTime date)
        {
            var minAmount = new DateTime(1970, 1, 1);
            var maxAmount = DateTime.MaxValue;

            if (!Validation.CheckDateWithinRange(minAmount, maxAmount, date))
                throw new Exception("The date entered was out of range. Value must lie between " + minAmount.ToShortDateString() + " and " + maxAmount.ToShortDateString() + ".");
        }
    }
}
