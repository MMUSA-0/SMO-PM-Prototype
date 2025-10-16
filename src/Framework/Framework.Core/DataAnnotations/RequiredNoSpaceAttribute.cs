// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RequiredNoSpaceAttribute.cs" company="Usama Nada">
//   No Copyright .. Copy, Share, and Evolve.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Framework.Core.DataAnnotations
{
    /// <summary>
    /// The required no space attribute.
    /// </summary>
    public class RequiredNoSpaceAttribute : ValidationAttribute, IClientModelValidator
    {
        public void AddValidation(ClientModelValidationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            MergeAttribute(context.Attributes, "data-val", "true");
            MergeAttribute(context.Attributes, "data-val-required", this.FormatErrorMessage(context.ModelMetadata.DisplayName));
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

            if (string.IsNullOrEmpty(value.ToString()))
            {
                return false;
            }

            if (value.GetType() != typeof(string))
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
}