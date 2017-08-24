using System;
using System.Text.RegularExpressions;

namespace SaltVault.Core.Validation
{
    public class ValidationService
    {
        public bool CheckStringOnlyLetters(string input)
        {
            var regex = new Regex(@"^[a-zA-Z0-9\s]*$");

            return regex.IsMatch(input);
        }

        public bool CheckStringWithinLengthRange(int min, int max, string input)
        {
            return input.Length >= min && input.Length <= max;
        }

        public bool CheckDecimalWithinSizeRange(decimal min, decimal max, decimal input)
        {
            return input >= min && input <= max;
        }

        public bool CheckDateWithinRange(DateTime min, DateTime max, DateTime input)
        {
            return input >= min && input <= max;
        }
    }
}
