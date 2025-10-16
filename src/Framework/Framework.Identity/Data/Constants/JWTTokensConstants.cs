// Ignore Spelling: Auth

namespace Framework.Identity.Data.Constants
{
    /// <summary>
    /// The JWT tokens constants.
    /// </summary>
    public static class JWTTokensConstants
    {
        /// <summary>
        /// Gets the token expiry.
        /// </summary>
        public static TimeSpan TokenExpiry { get => TimeSpan.FromMinutes(30); }//TimeSpan.FromHours(2); }
        /// <summary>
        /// Gets the refresh token expiry.
        /// </summary>
        /// <value>
        /// The refresh token expiry.
        /// </value>
        public static TimeSpan RefreshTokenExpiry { get => TimeSpan.FromDays(10); }
    }
}
