// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ArabicTextOnlyAttribute.cs" company="Usama Nada">
//   No Copyright .. Copy, Share, and Evolve.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


using Framework.Resources;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.ComponentModel.DataAnnotations;

namespace Framework.Core.DataAnnotations
{


    ///// <summary>
    ///// The arabic text only attribute.
    ///// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ArabicTextOnlyAttribute : RegularExpressionAttribute, IClientModelValidator
    {
        /// <summary>
        /// The pattern.
        /// </summary>
        private new const string Pattern = @"^[\u0600-\u06FF\u003A\0-9s]{0,4000}$";

        /// <summary>
        /// Initializes a new instance of the <see cref="ArabicTextOnlyAttribute"/> class.
        /// </summary>
        public ArabicTextOnlyAttribute()
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
            context.Attributes.Add("data-val-regex", "الاسم باللغه العربيه لا يقبل سوى أحرف عربية");
            context.Attributes.Add("data-val-regex-pattern", @"^[\u0600-\u06FF\u003A\0-9s]{0,4000}$");
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
            return string.Format(SharedResources.ArabicLettersOnlyErrorMessage, name);
        }
    }


}