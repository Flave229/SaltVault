using System;
using Services.Models.FinanceModels;
using Services.Validation;

namespace Services.FormHelpers
{
    public static class PaymentValidator
    {
        private static readonly ValidationService Validation = new ValidationService();

        public static void CheckIfValidPayment(Payment payment)
        {
            try
            {
                if (payment == null) throw new Exception("The payment object given was null.");
                if (!Validation.CheckGuidValid(payment.PersonId)) throw new Exception("The person object given was invalid.");

                if (!Validation.CheckGuidValid(payment.Id)) throw new Exception("The Id for the payment object was invalid.");

                CheckAmountValid(payment.Amount);
                CheckDateValid(payment.Created);
            }
            catch (Exception ex)
            {
                throw new Exception("The payment object cannot be validated: " + ex.Message, ex);
            }
        }

        public static void CheckAmountValid(decimal amount)
        {
            var minAmount = 0.01m;
            var maxAmount = 1000000;

            if (!Validation.CheckDecimalWithinSizeRange(minAmount, maxAmount, amount))
                throw new Exception("The amount entered for the payment was out of range. Value must lie between " + minAmount + " and " + maxAmount + ".");
        }

        public static void CheckDateValid(DateTime date)
        {
            var minAmount = new DateTime(1970, 1, 1);
            var maxAmount = DateTime.Now;

            if (!Validation.CheckDateWithinRange(minAmount, maxAmount, date))
                throw new Exception("The date entered for the payment was out of range. Value must lie between " + minAmount.ToShortDateString() + " and " + maxAmount.ToShortDateString() + ".");
        }
    }
}
