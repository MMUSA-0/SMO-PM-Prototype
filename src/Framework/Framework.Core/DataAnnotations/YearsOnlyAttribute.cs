// --------------------------------------------------------------------------------------------------------------------
// <copyright file="YearsOnlyAttribute.cs" company="Usama Nada">
//   No Copyright .. Copy, Share, and Evolve.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Framework.Core.Extensions;
using Framework.Resources;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Localization;
using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Framework.Core.DataAnnotations
{
    #region usings

    //using Microsoft.AspNetCore.Mvc.DataAnnotations.Internal;

    #endregion

    /// <summary>
    ///     The years only attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class YearsOnlyAttribute : RegularExpressionAttribute
    {
        /// <summary>
        ///     The pattern.
        /// </summary>
        private new const string Pattern = @"^\d{4}$";

        /// <summary>
        ///     Initializes a new instance of the <see cref="YearsOnlyAttribute" /> class.
        /// </summary>
        public YearsOnlyAttribute()
            : base(Pattern)
        {
        }

        /// <summary>
        ///     Gets or sets the culture.
        /// </summary>
        public CalendarType CalendarType { get; set; } = CalendarType.gregorian;

        /// <summary>
        /// The format error message.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string FormatErrorMessage(string name)
        {
            return string.Format(SharedResources.YearOnlyErrorMessage, name);
        }

        /// <summary>
        /// The is valid.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public override bool IsValid(object value)
        {
            if (string.IsNullOrEmpty(value.To<string>()))
            {
                return true;
            }

            try
            {
                var date = DateTime.ParseExact(
                    value.To<string>(),
                    "yyyy",
                    this.CalendarType == CalendarType.gregorian ? new CultureInfo("en-US") : new CultureInfo("ar-SA"));
                return date.Year != DateTime.MinValue.Year;
            }
            catch
            {
                return false;
            }
        }
    }

    /// <summary>
    /// The years only attribute adaptor.
    /// </summary>
    //public class YearsOnlyAttributeAdaptor : RegularExpressionAttributeAdapter
    //{
    //    /// <summary>
    //    /// Initializes a new instance of the <see cref="YearsOnlyAttributeAdaptor"/> class.
    //    /// </summary>
    //    /// <param name="attribute">
    //    /// The attribute.
    //    /// </param>
    //    /// <param name="stringLocalizer">
    //    /// The string localizer.
    //    /// </param>
    //    public YearsOnlyAttributeAdaptor(RegularExpressionAttribute attribute, IStringLocalizer stringLocalizer)
    //        : base(attribute, stringLocalizer)
    //    {
    //    }
    //}

    public class YearsOnlyAttributeAdaptor : AttributeAdapterBase<YearsOnlyAttribute>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GreaterThanAttributeAdapter"/> class.
        /// </summary>
        /// <param name="attribute">
        /// The attribute.
        /// </param>
        /// <param name="stringLocalizer">
        /// The string localizer.
        /// </param>
        public YearsOnlyAttributeAdaptor(YearsOnlyAttribute attribute, IStringLocalizer stringLocalizer)
            : base(attribute, stringLocalizer)
        {
            this.CurrentAttribute = attribute;
        }

        /// <summary>
        /// Gets or sets the current attribute.
        /// </summary>
        public YearsOnlyAttribute CurrentAttribute { get; set; }

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
