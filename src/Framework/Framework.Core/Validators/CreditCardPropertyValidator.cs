using FluentValidation;
using FluentValidation.Validators;
using System.Linq;

namespace Framework.Web.Validators
{
    /// <summary>
    /// Credit card validator
    /// </summary>
    public class CreditCardPropertyValidator<T> : PropertyValidator<T, string>
    {
        public override string Name => "CreditCardPropertyValidator";

        public CreditCardPropertyValidator()
        {
        }

        public override bool IsValid(ValidationContext<T> context, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            var ccValue = value.Replace(" ", "").Replace("-", "");

            var checksum = 0;
            var evenDigit = false;

            // http://www.beachnet.com/~hstiles/cardtype.html
            foreach (var digit in ccValue.Reverse())
            {
                if (!char.IsDigit(digit))
                    return false;

                var digitValue = (digit - '0') * (evenDigit ? 2 : 1);
                evenDigit = !evenDigit;

                while (digitValue > 0)
                {
                    checksum += digitValue % 10;
                    digitValue /= 10;
                }
            }

            return (checksum % 10) == 0;
        }

        protected override string GetDefaultMessageTemplate(string errorCode)
        {
            return "Credit card number is not valid";
        }
    }
}
