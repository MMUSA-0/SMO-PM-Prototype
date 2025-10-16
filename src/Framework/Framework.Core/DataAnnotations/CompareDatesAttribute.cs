// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompareDatesAttribute.cs" company="Usama Nada">
//   No Copyright .. Copy, Share, and Evolve.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Framework.Core.DataAnnotations
{
    #region usings

    using System;

    #endregion

    public enum CompareOperator
    {
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual
    }
    /// <summary>
    ///     The compare dates attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class CompareDatesAttribute : ValidationAttribute, IClientModelValidator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CompareDatesAttribute"/> class.
        /// </summary>
        /// <param name="from">
        /// The from.
        /// </param>
        public CompareDatesAttribute()
        {
            //this.Start = from;
        }

        /// <summary>
        ///     Gets the start.
        /// </summary>
        public string Start { get; set; }

        public string CompareToPropertyName { get; set; }

        public string CompareToProperty { get; set; }

        public CompareOperator OperatorName { get; set; } = CompareOperator.GreaterThanOrEqual;
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
            #region Old
            //if (value == null)
            //{
            //    return ValidationResult.Success;
            //}

            //var dateTo = (IComparable)value;

            //var propertyInfo = validationContext.ObjectType.GetProperty(this.Start);
            //if (propertyInfo == null)
            //{
            //    return ValidationResult.Success;
            //}

            //var dateFrom = (IComparable)propertyInfo.GetValue(validationContext.ObjectInstance, null);


            //if ((operatorname == CompareOperator.GreaterThan && dateTo.CompareTo(dateFrom) <= 0) ||
            //    (operatorname == CompareOperator.GreaterThanOrEqual && dateTo.CompareTo(dateFrom) < 0) ||
            //    (operatorname == CompareOperator.LessThan && dateTo.CompareTo(dateFrom) >= 0) ||
            //    (operatorname == CompareOperator.LessThanOrEqual && dateTo.CompareTo(dateFrom) > 0))
            //{
            //    var errorMessage = FormatErrorMessage(validationContext.DisplayName);
            //    return new ValidationResult(errorMessage);
            //}
            //return null;
            #endregion
            string operstring = (OperatorName == CompareOperator.GreaterThan ?
           "greater than " : (OperatorName == CompareOperator.GreaterThanOrEqual ?
           "greater than or equal to " :
           (OperatorName == CompareOperator.LessThan ? "less than " :
           (OperatorName == CompareOperator.LessThanOrEqual ? "less than or equal to " : ""))));

            var basePropertyInfo = validationContext.ObjectType.GetProperty(this.CompareToProperty);
            var valOther = (IComparable)basePropertyInfo.GetValue(validationContext.ObjectInstance, null);
            var valThis = (IComparable)value;

            if (valOther != null && valThis != null)
            {
                if ((OperatorName == CompareOperator.GreaterThan && valThis.CompareTo(valOther) <= 0) ||
                    (OperatorName == CompareOperator.GreaterThanOrEqual && valThis.CompareTo(valOther) < 0) ||
                    (OperatorName == CompareOperator.LessThan && valThis.CompareTo(valOther) >= 0) ||
                    (OperatorName == CompareOperator.LessThanOrEqual && valThis.CompareTo(valOther) > 0))
                    return new ValidationResult(base.ErrorMessage);
            }

            return null;
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            MergeAttribute(context.Attributes, "data-val", "true");
            var errorMessage = FormatErrorMessage(context.ModelMetadata.GetDisplayName());
            MergeAttribute(context.Attributes, "data-val-comparedates", errorMessage);
            var x = context.ModelMetadata;
            MergeAttribute(context.Attributes, "data-val-comparedates-comparetopropertyname", CompareToPropertyName);
            MergeAttribute(context.Attributes, "data-val-comparedates-operatorname", OperatorName.ToString());

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