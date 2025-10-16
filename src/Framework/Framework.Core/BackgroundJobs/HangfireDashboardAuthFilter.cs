// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HangfireDashboardAuthFilter.cs" company="Usama Nada">
//   No Copyright .. Copy, Share, and Evolve.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Framework.Core.BackgroundJobs
{
    #region usings

    using Hangfire.Dashboard;

    #endregion

    /// <summary>
    /// The hangfire dashboard auth filter.
    /// </summary>
    public class HangfireDashboardAuthFilter : IDashboardAuthorizationFilter
    {
        /// <summary>
        /// The authorize.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();

            if (httpContext.Connection.RemoteIpAddress?.ToString() == "127.0.0.1" ||
            httpContext.Connection.RemoteIpAddress?.ToString() == "::1")
            {
                return true;
            }

            // Allow all authenticated users to see the Dashboard (potentially dangerous).
            return httpContext.User.Identity.IsAuthenticated;
        }
    }
}