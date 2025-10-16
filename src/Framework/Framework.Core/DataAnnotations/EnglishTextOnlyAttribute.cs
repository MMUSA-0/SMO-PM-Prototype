// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnglishTextOnlyAttribute.cs" company="Usama Nada">
//   No Copyright .. Copy, Share, and Evolve.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Framework.Resources;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.ComponentModel.DataAnnotations;

namespace Framework.Core.DataAnnotations
{
    #region usings

    // using Microsoft.AspNetCore.Mvc.DataAnnotations.Internal;

    #endregion

    /// <summary>
    ///     The english text only attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class EnglishTextOnlyAttribute : RegularExpressionAttribute, IClientModelValidator
    {
        /// <summary>
        ///     The pattern.
        /// </summary>
        private new const string Pattern = @"^[A-Za-z0-9\s!@#$%^&*()_+=-`~\\\]\[{}|';:/.,?]*$";

        /// <summary>
        /// Initializes a new instance of the <see cref="EnglishTextOnlyAttribute"/> class.
        /// </summary>
        public EnglishTextOnlyAttribute()
            : base(Pattern)
        {
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            return base.IsValid(value, validationContext);
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            context.Attributes.Add("data-val", "true");
            context.Attributes.Add("data-val-regex", "الاسم باللغه الانجليزيه لا يقبل سوى أحرف إنجليزية");
            context.Attributes.Add("data-val-regex-pattern", @"^[A-Za-z0-9\s!@#$%^&amp;*()_+=-`~\\\]\[{}|';:/.,?]*$");
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
            return string.Format(SharedResources.EnglishLettersOnlyErrorMessage, name);
        }
    }
}