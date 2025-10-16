// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DecimalsOnlyAttribute.cs" company="Usama Nada">
//   No Copyright .. Copy, Share, and Evolve.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#region usings

using Framework.Core.Extensions;
using Framework.Resources;
using System;
using System.ComponentModel.DataAnnotations;

namespace Framework.Core.DataAnnotations
{
    #endregion

    /// <summary>
    ///     The numbers only attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class DecimalsOnlyAttribute : RegularExpressionAttribute
    {
        /// <summary>
        ///     The pattern.
        /// </summary>
        private new const string Pattern = @"^[0-9]\d*(\.\d+)?$";

        /// <summary>
        /// Initializes a new instance of the <see cref="DecimalsOnlyAttribute"/> class. 
        ///     Initializes a new instance of the <see cref="NumbersOnlyAttribute"/> class.
        ///     Initializes a new instance of the <see cref="ArabicTextOnlyAttribute"/> class.
        ///     Initializes a new instance of the <see cref="DisableScriptsAttribute"/>
        ///     class.
        /// </summary>
        public DecimalsOnlyAttribute()
            : base(Pattern)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DecimalsOnlyAttribute"/> class.
        /// </summary>
        /// <param name="minimum">
        /// The minimum.
        /// </param>
        /// <param name="maximum">
        /// The maximum.
        /// </param>
        public DecimalsOnlyAttribute(decimal? minimum, decimal? maximum)
            : base(Pattern)
        {
            this.Minimum = minimum;
            this.Maximum = maximum;
        }

        /// <summary>
        /// Gets the maximum.
        /// </summary>
        private decimal? Maximum { get; }

        /// <summary>
        /// Gets the minimum.
        /// </summary>
        private decimal? Minimum { get; }

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
            return string.Format(SharedResources.NumberOnlyErrorMessage, name);
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
            var obj = value.To<decimal>();
            if (this.Maximum != null && this.Minimum != null)
            {
                return obj < this.Maximum && obj > this.Minimum;
            }

            if (this.Minimum != null)
            {
                return obj > this.Minimum;
            }

            if (this.Maximum != null)
            {
                return obj > this.Minimum;
            }

            return base.IsValid(value);
        }
    }


}