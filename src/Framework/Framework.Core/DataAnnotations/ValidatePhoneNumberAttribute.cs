// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidatePhoneNumberAttribute.cs" company="Usama Nada">
//   No Copyright .. Copy, Share, and Evolve.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Framework.Core.DataAnnotations
{
    #region usings

    #endregion

    /// <summary>
    ///     The phone number attribute.
    /// </summary>
    public class ValidatePhoneNumberAttribute : ValidationAttribute, IClientModelValidator
    {
        /// <summary>
        ///     The default error message.
        /// </summary>
        /// <summary>
        ///     Gets or sets the country code.
        /// </summary>
        public string CountryCode { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the number type.
        /// </summary>
        public NumberType NumberType { get; set; } = NumberType.MOBILE;

        public void AddValidation(ClientModelValidationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            MergeAttribute(context.Attributes, "data-val", "true");
            MergeAttribute(context.Attributes, "data-val-checkisvalidnumber", this.FormatErrorMessage(context.ModelMetadata.DisplayName));
            MergeAttribute(
                context.Attributes,
                "data-val-checkisvalidnumber-countrycode",
                this.CountryCode);
            MergeAttribute(
                context.Attributes,
                "data-val-checkisvalidnumber-numbertype",
                this.NumberType.ToString());
        }

        // public override bool
        /// <summary>
        /// The is valid.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="validationContext">
        /// The validation context.
        /// </param>
        /// <returns>
        /// The <see cref="ValidationResult"/>.
        /// </returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }

            var numberType = this.NumberType; // .ToString().ToEnum<Utils.NumberType>();

            var isValid = Core.Utils.PhoneNumbers.IsValidNumber(value as string, numberType, this.CountryCode);

            if (isValid)
            {
                validationContext.ObjectType.GetProperty(validationContext.MemberName).SetValue(
                    validationContext.ObjectInstance,
                    Core.Utils.PhoneNumbers.FormatPhoneNumber(value.ToString(), this.CountryCode),
                    null);
                return ValidationResult.Success;
            }

            return new ValidationResult(this.ErrorMessage);
        }
        private bool MergeAttribute(
          IDictionary<string, string> attributes,
          string key,
          string value)
        {
            if (attributes.ContainsKey(key))
            {
                return false;
            }
            attributes.Add(key, value);
            return true;
        }
    }
}