using FluentValidation;
using FluentValidation.Validators;
using System;

namespace Framework.Web.Validators
{
    /// <summary>
    /// Decimal validator
    /// </summary>
    public class DecimalPropertyValidator<T> : PropertyValidator<T, decimal>
    {
        private readonly decimal _maxValue;

        public override string Name => "DecimalPropertyValidator";

        public DecimalPropertyValidator(decimal maxValue)
        {
            _maxValue = maxValue;
        }

        public override bool IsValid(ValidationContext<T> context, decimal value)
        {
            return Math.Round(value, 3) < _maxValue;
        }

        protected override string GetDefaultMessageTemplate(string errorCode)
        {
            return "Decimal value is out of range";
        }
    }
}