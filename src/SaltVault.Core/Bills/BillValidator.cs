using System;
using SaltVault.Core.Bills.Models;
using SaltVault.Core.Validation;

namespace SaltVault.Core.Bills
{
    public static class BillValidator
    {
        private static readonly ValidationService Validation = new ValidationService();

        public static void CheckIfValidBill(AddBillRequest billRequest)
        {
            try
            {
                if (billRequest == null) throw new Exception("The bill object given was null.");
                if (billRequest.PeopleIds.Count == 0) throw new Exception("No people were assigned to the bill.");

                CheckNameValid(billRequest.Name);
                CheckAmountValid(billRequest.TotalAmount);
                CheckDateValid(billRequest.Due);
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
                throw new Exception("The date entered was out of range. Value must lie between " + minAmount.ToString("d") + " and " + maxAmount.ToString("d") + ".");
        }
    }
}
