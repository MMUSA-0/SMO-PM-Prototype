// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExceptionExtensions.cs" company="Usama Nada">
//   No Copyright .. Copy, Share, and Evolve.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Framework.Core.Extensions
{
    #region usings

    using System;
    using System.Collections;
    using System.Text;

    #endregion

    /// <summary>
    ///     The exception extensions.
    /// </summary>
    public static class ExceptionExtensions
    {
        /// <summary>
        /// The to formatted string.
        /// </summary>
        /// <param name="exception">
        /// The exception.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string ToFormattedString(this Exception exception)
        {
            ////if (WebHelper.CurrentHttpContext != null)
            ////{
            ////    exception.Data["Page Url"] = WebHelper.CurrentHttpContext.Request.Url();
            ////    exception.Data["User Agent"] = WebHelper.CurrentHttpContext.Request.UserAgent();
            ////    exception.Data["Current User"] = WebHelper.CurrentHttpContext?.User?.Identity?.Name ?? "anonymous";
            ////}
            var exceptionString = new StringBuilder();
            exceptionString.AppendFormat("{0}\n", exception.Message);
            exceptionString.Append(exception);
            exceptionString.Append("\nData:\n");

            foreach (DictionaryEntry dictionaryEntry in exception.Data)
            {
                exceptionString.AppendFormat("\t{0}: {1}\n", dictionaryEntry.Key, dictionaryEntry.Value);
            }

            exceptionString.Append("--------- End of the exception ---------");

            return exceptionString.ToString();
        }
    }
}