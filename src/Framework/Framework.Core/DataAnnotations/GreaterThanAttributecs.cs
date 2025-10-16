// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GreaterThanAttributecs.cs" company="Usama Nada">
//   No Copyright .. Copy, Share, and Evolve.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

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
    ///     The date greater than today.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class GreaterThanAttribute : ValidationAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GreaterThanAttribute"/> class.
        /// </summary>
        /// <param name="minPropertyName">
        /// The other property.
        /// </param>
        public GreaterThanAttribute(string minPropertyName)
        {
            this.MinPropertyName = minPropertyName;
        }

        /// <summary>
        /// Gets or sets a value indicating whether allow equal values.
        /// </summary>
        public bool AllowEqualValues { get; set; }

        /// <summary>
        ///     Gets or sets the other property.
        /// </summary>
        public string MinPropertyName { get; set; }

        /// <summary>
        /// Gets or sets the min display name.
        /// </summary>
        private string MinDisplayName { get; set; }

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
            return string.Format(
                CultureInfo.CurrentCulture,
                this.ErrorMessageString,
                currentPropDisplayName,
                this.MinDisplayName);
        }

        /// <summary>
        /// The get second comparable.
        /// </summary>
        /// <param name="validationContext">
        /// The validation context.
        /// </param>
        /// <returns>
        /// The <see cref="IComparable"/>.
        /// </returns>
        public IComparable GetSecondComparable(ValidationContext validationContext)
        {
            var propertyInfo = validationContext.ObjectType.GetProperty(this.MinPropertyName);
            if (propertyInfo != null)
            {
                var secondValue = propertyInfo.GetValue(validationContext.ObjectInstance, null);
                return secondValue as IComparable;
            }

            return null;
        }

        /// <summary>
        /// The is valid.
        /// </summary>
        /// <param name="firstValue">
        /// The first value.
        /// </param>
        /// <param name="validationContext">
        /// The validation context.
        /// </param>
        /// <returns>
        /// The <see cref="ValidationResult"/>.
        /// </returns>
        protected override ValidationResult IsValid(object firstValue, ValidationContext validationContext)
        {
            var propertyInfo = validationContext.ObjectType.GetProperty(this.MinPropertyName);
            if (propertyInfo == null)
            {
                return new ValidationResult($"unknown property {this.MinPropertyName}");
            }

            var firstComparable = firstValue as IComparable;
            var secondComparable = this.GetSecondComparable(validationContext);

            if (firstComparable == null || secondComparable == null)
            {
                return ValidationResult.Success;
            }

            if (this.AllowEqualValues && firstComparable.CompareTo(secondComparable) == 0)
            {
                return ValidationResult.Success;
            }

            if (firstComparable.CompareTo(secondComparable) < 0)
            {
                return new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName));
            }

            return ValidationResult.Success;
        }
    }

    /// <summary>
    /// The greater than attribute adapter.
    /// </summary>
    public class GreaterThanAttributeAdapter : AttributeAdapterBase<GreaterThanAttribute>
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
        public GreaterThanAttributeAdapter(GreaterThanAttribute attribute, IStringLocalizer stringLocalizer)
            : base(attribute, stringLocalizer)
        {
            this.CurrentAttribute = attribute;
        }

        /// <summary>
        /// Gets or sets the current attribute.
        /// </summary>
        public GreaterThanAttribute CurrentAttribute { get; set; }

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
            MergeAttribute(context.Attributes, "data-val-greaterthan", this.GetErrorMessage(context));
            MergeAttribute(context.Attributes, "data-propertytested", this.CurrentAttribute.MinPropertyName);
            MergeAttribute(
                context.Attributes,
                "data-allowequalvalues",
                this.CurrentAttribute.AllowEqualValues.ToString());
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