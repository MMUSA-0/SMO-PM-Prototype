// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IsDateAfterAttribute.cs" company="Usama Nada">
//   No Copyright .. Copy, Share, and Evolve.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Localization;
using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;

namespace Framework.Core.DataAnnotations
{
    #region usings

    #endregion

    // <summary>
    // The is date after attribute.
    // The compare dates attribute.
    // </summary>    
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class IsDateAfterAttribute : ValidationAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IsDateAfterAttribute"/> class.
        /// </summary>
        /// <param name="from">
        /// The from.
        /// </param>
        /// <param name="fromDisplayType">
        /// The from Display Type.
        /// </param>
        /// <param name="fromResourceName">
        /// The from Resource Name.
        /// </param>
        public IsDateAfterAttribute(string from, Type fromDisplayType, string fromResourceName)
        {
            this.Start = from;
            this.FromDisplayType = fromDisplayType;
            this.FromResourceName = fromResourceName;
        }

        /// <summary>
        /// Gets or sets a value indicating whether allow equal dates.
        /// </summary>
        public bool AllowEqualDates { get; set; }

        /// <summary>
        ///     Gets the start.
        /// </summary>
        public string Start { get; }

        /// <summary>
        /// Gets or sets the from display name.
        /// </summary>
        private string FromDisplayName { get; set; }

        /// <summary>
        /// Gets the from display type.
        /// </summary>
        private Type FromDisplayType { get; }

        /// <summary>
        /// Gets the from resource name.
        /// </summary>
        private string FromResourceName { get; }

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
            if (string.IsNullOrEmpty(this.FromDisplayName))
            {
                this.GetResourceValue();
            }

            return string.Format(
                CultureInfo.CurrentCulture,
                this.ErrorMessageString,
                currentPropDisplayName,
                this.FromDisplayName);
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
            var propertyInfo = validationContext.ObjectType.GetProperty(this.Start);
            if (propertyInfo == null)
            {
                return new ValidationResult($"unknown property {this.Start}");
            }

            var startDateValue = propertyInfo.GetValue(validationContext.ObjectInstance, null);

            if (string.IsNullOrEmpty(this.FromDisplayName))
            {
                this.GetResourceValue();
            }

            if (!(value is DateTime) || !(startDateValue is DateTime))
            {
                return ValidationResult.Success;
            }

            // Compare values
            if ((DateTime)value >= (DateTime)startDateValue)
            {
                if (this.AllowEqualDates)
                {
                    return ValidationResult.Success;
                }

                if ((DateTime)value > (DateTime)startDateValue)
                {
                    return ValidationResult.Success;
                }
            }

            // var provider = new DataAnnotationsModelMetadataProvider();
            // var otherMetaData = provider.GetMetadataForProperty(() => model, type, this.Start);

            // this.StartDisplayName = otherMetaData.DisplayName;
            return new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName));
        }

        /// <summary>
        /// The get resource value.
        /// </summary>
        private void GetResourceValue()
        {
            if (this.FromDisplayType != null && !string.IsNullOrEmpty(this.FromResourceName))
            {
                var property = this.FromDisplayType.GetProperty(
                    this.FromResourceName,
                    BindingFlags.Public | BindingFlags.Static);

                if (property != null && property.PropertyType != typeof(string))
                {
                    this.FromDisplayName = string.Empty;
                }
                else
                {
                    this.FromDisplayName = property.GetValue(null, null).ToString();
                }
            }
        }
    }

    /// <summary>
    /// The is date after attribute adapter.
    /// </summary>
    public class IsDateAfterAttributeAdapter : AttributeAdapterBase<IsDateAfterAttribute>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IsDateAfterAttributeAdapter"/> class.
        /// </summary>
        /// <param name="attribute">
        /// The attribute.
        /// </param>
        /// <param name="stringLocalizer">
        /// The string localizer.
        /// </param>
        public IsDateAfterAttributeAdapter(IsDateAfterAttribute attribute, IStringLocalizer stringLocalizer)
            : base(attribute, stringLocalizer)
        {
            this.CurrentAttribute = attribute;
        }

        /// <summary>
        /// Gets or sets the current attribute.
        /// </summary>
        public IsDateAfterAttribute CurrentAttribute { get; set; }

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
            MergeAttribute(context.Attributes, "data-val-isdateafter", this.GetErrorMessage(context));
            MergeAttribute(context.Attributes, "data-propertytested", this.CurrentAttribute.Start);
            MergeAttribute(
                context.Attributes,
                "data-allowequalvalues",
                this.CurrentAttribute.AllowEqualDates.ToString());
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