namespace Framework.Identity.Data.Dtos
{
    public class UserVM
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Gets or sets the user name.
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// Gets or sets the full name en.
        /// </summary>
        public string FullNameEn { get; set; }
        public string FullNameAr { get; set; }
        /// <summary>
        /// Gets or sets the identity no.
        /// </summary>
        public string IdentityNo { get; set; }
        /// <summary>
        /// Gets or sets the phone number.
        /// </summary>
        public string PhoneNumber { get; set; }
        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// Gets or sets the user roles.
        /// </summary>
        public List<string> UserRoles { get; set; }
        /// <summary>
        /// Gets or sets the token.
        /// </summary>
        public string Token { get; set; }

        public DateTime TokenExpiresOn { get; set; }
        /// <summary>
        /// Gets or sets the refresh token.
        /// </summary>
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiresOn { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether is external.
        /// </summary>
        public bool IsExternal { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether first is login.
        /// </summary>
        public bool IsFirstLogin { get; set; }
        /// <summary>
        /// Gets or sets the verification try count.
        /// </summary>
        public int? VerificationTryCount { get; set; }
        /// <summary>
        /// Gets or sets the mobile verification code.
        /// </summary>
        public string MobileVerificationCode { get; set; }
        /// <summary>
        /// Gets or sets the trans id.
        /// </summary>
        public string TransId { get; set; }
        /// <summary>
        /// Gets or sets the roles codes.
        /// </summary>
        public List<int>? RolesCodes { get; set; }
        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        public string? Status { get; set; }
    }
}
