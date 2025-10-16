// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisableScriptsAttribute.cs" company="Usama Nada">
//   No Copyright .. Copy, Share, and Evolve.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Framework.Resources;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Localization;
using System;
using System.ComponentModel.DataAnnotations;

namespace Framework.Core.DataAnnotations
{
    #region usings

    //using Microsoft.AspNetCore.Mvc.DataAnnotations.Internal;

    #endregion

    /// <summary>
    ///     The disable scripts attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class DisableScriptsAttribute : RegularExpressionAttribute
    {
        /// <summary>
        ///     The pattern.
        /// </summary>
        private new const string Pattern = @"^[^<>{}]+$";

        /// <summary>
        ///     Initializes a new instance of the <see cref="DisableScriptsAttribute" /> class.
        /// </summary>
        public DisableScriptsAttribute()
            : base(Pattern)
        {
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
            return string.Format(SharedResources.ScriptsNotAllowedErrorMessage, name);
        }
    }

    //public class DisableScriptsAttribute : ValidationAttribute
    //{
    //    private const string Pattern = @"^[^<>{}]+$";

    //    public override bool IsValid(object value)
    //    {
    //        Match match = Regex.Match(value.ToString(), Pattern, RegexOptions.IgnoreCase);
    //        return match.Success;
    //    }

    //    public override string FormatErrorMessage(string name)
    //    {
    //        return string.Format(CommonMessages.ScriptsNotAllowedErrorMessage, name);
    //    }

    //}

    /// <summary>
    /// The disable scripts attribute adaptor.
    /// </summary>
    //public class DisableScriptsAttributeAdaptor : RegularExpressionAttributeAdapter
    //{
    //    /// <summary>
    //    /// Initializes a new instance of the <see cref="DisableScriptsAttributeAdaptor"/> class.
    //    /// </summary>
    //    /// <param name="attribute">
    //    /// The attribute.
    //    /// </param>
    //    /// <param name="stringLocalizer">
    //    /// The string localizer.
    //    /// </param>
    //    public DisableScriptsAttributeAdaptor(RegularExpressionAttribute attribute, IStringLocalizer stringLocalizer)
    //        : base(attribute, stringLocalizer)
    //    {
    //    }
    //}

    public class DisableScriptsAttributeAdaptor : AttributeAdapterBase<DisableScriptsAttribute>
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
        public DisableScriptsAttributeAdaptor(DisableScriptsAttribute attribute, IStringLocalizer stringLocalizer)
            : base(attribute, stringLocalizer)
        {
            this.CurrentAttribute = attribute;
        }

        /// <summary>
        /// Gets or sets the current attribute.
        /// </summary>
        public DisableScriptsAttribute CurrentAttribute { get; set; }

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