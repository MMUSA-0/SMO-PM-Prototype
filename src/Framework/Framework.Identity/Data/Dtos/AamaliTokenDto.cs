namespace Framework.Identity.Data.Dtos
{
    /// <summary>
    /// The token dto.
    /// </summary>
    public class AamaliTokenDto
    {
        /// <summary>
        /// Gets or sets the access token.
        /// </summary>
        public string AccessToken { get; set; }
        /// <summary>
        /// Gets or sets the expires on.
        /// </summary>
        /// <value>
        /// The expires on.
        /// </value>
        public DateTime ExpiresOn { get; set; }
    }
}
