using FluentValidation;
using Framework.Resources;

namespace Framework.Web.Validators
{
    /// <summary>
    /// Validator extensions
    /// </summary>
    public static class ValidatorExtensions
    {
        /// <summary>
        /// Set credit card validator
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="ruleBuilder">RuleBuilder</param>
        /// <returns>Result</returns>
        public static IRuleBuilderOptions<T, string> IsCreditCard<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new CreditCardPropertyValidator<T>());
        }

        /// <summary>
        /// Set decimal validator
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="ruleBuilder">RuleBuilder</param>
        /// <param name="maxValue">Maximum value</param>
        /// <returns>Result</returns>
        public static IRuleBuilderOptions<T, decimal> IsDecimal<T>(this IRuleBuilder<T, decimal> ruleBuilder, decimal maxValue)
        {
            return ruleBuilder.SetValidator(new DecimalPropertyValidator<T>(maxValue));
        }

        public static IRuleBuilder<T, string> IsPassword<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            var regExp = "^";
            //Passwords must be at least X characters and contain the following: upper case (A-Z), lower case (a-z), number (0-9) and special character (e.g. !@#$%^&*-)
            regExp += "(?=.*?[A-Z])";
            regExp += "(?=.*?[a-z])";
            regExp += "(?=.*?[0-9])";
            regExp += "(?=.*?[#?!@$%^&*-])";
            regExp += $".{{8,}}$";


            var options = ruleBuilder
                .NotEmpty().WithMessage(SharedResources.RequiredFieldMessage)
                .Matches(regExp).WithMessage(SharedResources.PasswordHint);

            return options;
        }
    }
}
