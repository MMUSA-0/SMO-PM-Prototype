// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnsureMinimumElementsAttribute.cs" company="Usama Nada">
//   No Copyright .. Copy, Share, and Evolve.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace Framework.Core.DataAnnotations
{
    #region usings

    #endregion

    /// <summary>
    ///     The ensure minimum elements attribute.
    /// </summary>
    public class EnsureMinimumElementsAttribute : ValidationAttribute
    {
        /// <summary>
        ///     The _min elements.
        /// </summary>
        private readonly int minElements;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnsureMinimumElementsAttribute"/> class.
        /// </summary>
        /// <param name="minElements">
        /// The min elements.
        /// </param>
        public EnsureMinimumElementsAttribute(int minElements)
        {
            this.minElements = minElements;
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
            var list = value as IList;
            return list?.Count >= this.minElements;
        }
    }
}