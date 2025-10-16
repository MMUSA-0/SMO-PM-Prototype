// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MustBeTrueAttribute.cs" company="Usama Nada">
//   No Copyright .. Copy, Share, and Evolve.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Framework.Core.DataAnnotations
{
    #region usings

    #endregion

    /// <summary>
    ///     The must be true attribute.
    /// </summary>
    public class MustBeTrueAttribute : ValidationAttribute, IClientModelValidator
    {
        public void AddValidation(ClientModelValidationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            MergeAttribute(context.Attributes, "data-val", "true");
            MergeAttribute(context.Attributes, "data-val-mustbetrue", this.FormatErrorMessage(context.ModelMetadata.DisplayName));
        }

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
            return string.Format(CultureInfo.CurrentCulture, this.ErrorMessageString, name);
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
        /// <exception cref="InvalidOperationException">
        /// </exception>
        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return false;
            }

            if (value.GetType() != typeof(bool))
            {
                throw new InvalidOperationException("can only be used on boolean properties.");
            }

            return (bool)value;
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


    /// <summary>
    /// The must be true attribute adapter.
    /// </summary>
    //public class MustBeTrueAttributeAdapter : AttributeAdapterBase<MustBeTrueAttribute>
    //{
    //    /// <summary>
    //    /// Initializes a new instance of the <see cref="MustBeTrueAttributeAdapter"/> class.
    //    /// </summary>
    //    /// <param name="attribute">
    //    /// The attribute.
    //    /// </param>
    //    /// <param name="stringLocalizer">
    //    /// The string localizer.
    //    /// </param>
    //    public MustBeTrueAttributeAdapter(MustBeTrueAttribute attribute, IStringLocalizer stringLocalizer)
    //        : base(attribute, stringLocalizer)
    //    {
    //    }

    //    /// <summary>
    //    /// The add validation.
    //    /// </summary>
    //    /// <param name="context">
    //    /// The context.
    //    /// </param>
    //    /// <exception cref="ArgumentNullException">
    //    /// </exception>
    //    public override void AddValidation(ClientModelValidationContext context)
    //    {
    //        if (context == null)
    //        {
    //            throw new ArgumentNullException(nameof(context));
    //        }

    //        MergeAttribute(context.Attributes, "data-val", "true");
    //        MergeAttribute(context.Attributes, "data-val-mustbetrue", this.GetErrorMessage(context));
    //    }

    //    /// <summary>
    //    /// The get error message.
    //    /// </summary>
    //    /// <param name="validationContext">
    //    /// The validation context.
    //    /// </param>
    //    /// <returns>
    //    /// The <see cref="string"/>.
    //    /// </returns>
    //    /// <exception cref="ArgumentNullException">
    //    /// </exception>
    //    public override string GetErrorMessage(ModelValidationContextBase validationContext)
    //    {
    //        if (validationContext == null)
    //        {
    //            throw new ArgumentNullException(nameof(validationContext));
    //        }

    //        return this.GetErrorMessage(
    //            validationContext.ModelMetadata,
    //            validationContext.ModelMetadata.GetDisplayName());
    //    }
    //}

    public class MustBeTrueAttributeAdapter : AttributeAdapterBase<MustBeTrueAttribute>
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
        public MustBeTrueAttributeAdapter(MustBeTrueAttribute attribute, IStringLocalizer stringLocalizer)
            : base(attribute, stringLocalizer)
        {
            this.CurrentAttribute = attribute;
        }

        /// <summary>
        /// Gets or sets the current attribute.
        /// </summary>
        public MustBeTrueAttribute CurrentAttribute { get; set; }

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
            MergeAttribute(context.Attributes, "data-val-mustbetrue", this.GetErrorMessage(context));
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