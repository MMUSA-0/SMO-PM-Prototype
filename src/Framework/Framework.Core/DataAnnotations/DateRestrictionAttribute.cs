// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DateRestrictionAttribute.cs" company="Usama Nada">
//   No Copyright .. Copy, Share, and Evolve.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Framework.Core.Extensions;
using Framework.Core.Utils;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Localization;
using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Framework.Core.DataAnnotations
{
    #region usings

    #endregion

    /// <summary>
    /// The date restriction type.
    /// </summary>
    public enum DateRestrictionType
    {
        /// <summary>
        /// The future only.
        /// </summary>
        FutureOnly,

        /// <summary>
        /// The future including today.
        /// </summary>
        FutureIncludingToday,

        /// <summary>
        /// The past only.
        /// </summary>
        PastOnly,

        /// <summary>
        /// The past including today.
        /// </summary>
        PastIncludingToday
    }

    /// <summary>
    /// The date restriction attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class DateRestrictionAttribute : ValidationAttribute
    {
        /// <summary>
        /// Gets or sets the date restriction type.
        /// </summary>
        public DateRestrictionType DateRestrictionType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether validate client side only.
        /// </summary>
        public bool ValidateClientSideOnly { get; set; }

        /// <summary>
        /// The format error message.
        /// </summary>
        /// <param name="currentPropDisplayName">
        /// The current prop display name.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string FormatErrorMessage(string currentPropDisplayName)
        {
            return string.Format(CultureInfo.CurrentCulture, this.ErrorMessageString, currentPropDisplayName);
        }

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
            if (this.ValidateClientSideOnly)
            {
                return ValidationResult.Success;
            }

            if (!(value is DateTime))
            {
                return ValidationResult.Success;
            }

            var selectedDate = value.To<DateTime>().Date;
            var today = DateTime.Now.Date;

            switch (this.DateRestrictionType)
            {
                case DateRestrictionType.FutureOnly:
                    if (selectedDate > today)
                    {
                        return ValidationResult.Success;
                    }

                    break;
                case DateRestrictionType.FutureIncludingToday:
                    if (selectedDate >= today)
                    {
                        return ValidationResult.Success;
                    }

                    break;
                case DateRestrictionType.PastOnly:
                    if (selectedDate < today)
                    {
                        return ValidationResult.Success;
                    }

                    break;
                case DateRestrictionType.PastIncludingToday:
                    if (selectedDate <= today)
                    {
                        return ValidationResult.Success;
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(this.DateRestrictionType));
            }

            return new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName));
        }
    }

    /// <summary>
    /// The date restriction attribute adapter.
    /// </summary>
    public class DateRestrictionAttributeAdapter : AttributeAdapterBase<DateRestrictionAttribute>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DateRestrictionAttributeAdapter"/> class.
        /// </summary>
        /// <param name="attribute">
        /// The attribute.
        /// </param>
        /// <param name="stringLocalizer">
        /// The string localizer.
        /// </param>
        public DateRestrictionAttributeAdapter(DateRestrictionAttribute attribute, IStringLocalizer stringLocalizer)
            : base(attribute, stringLocalizer)
        {
            this.CurrentAttribute = attribute;
        }

        /// <summary>
        /// Gets or sets the current attribute.
        /// </summary>
        public DateRestrictionAttribute CurrentAttribute { get; set; }

        /// <summary>
        /// The add validation.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        public override void AddValidation(ClientModelValidationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            MergeAttribute(context.Attributes, "data-val", "true");
            MergeAttribute(context.Attributes, "data-val-daterestriction", this.GetErrorMessage(context));
            MergeAttribute(
                context.Attributes,
                "data-todaydate",
                DateTimeHelper.GetGregoreanDateString(DateTime.Now.Date));
            MergeAttribute(
                context.Attributes,
                "data-daterestrictiontype",
                this.CurrentAttribute.DateRestrictionType.ToString().ToCamelCase());
        }

        /// <summary>
        /// The get error message.
        /// </summary>
        /// <param name="validationContext">
        /// The validation context.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        public override string GetErrorMessage(ModelValidationContextBase validationContext)
        {
            if (validationContext == null)
            {
                throw new ArgumentNullException(nameof(validationContext));
            }

            return this.GetErrorMessage(
                validationContext.ModelMetadata,
                validationContext.ModelMetadata.GetDisplayName());
        }
    }
}