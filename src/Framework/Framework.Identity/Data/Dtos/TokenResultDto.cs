using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Identity.Data.Dtos
{
    /// <summary>
    /// The token result base dto.
    /// </summary>
    public class TokenResultBaseDto
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TokenResultBaseDto"/> class.
        /// </summary>
        public TokenResultBaseDto()
        {

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="TokenResultBaseDto"/> class.
        /// </summary>
        /// <param name="_Token">The token.</param>
        /// <param name="_ExpiresAt">The expires at.</param>
        public TokenResultBaseDto(string _Token, DateTime _ExpiresAt)
        {
            Token = _Token;
            ExpiresAt = _ExpiresAt.ToUniversalTime();
        }
        /// <summary>
        /// Gets or sets the token.
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// Gets or sets the expires in.
        /// </summary>
        public DateTime ExpiresAt { get; set; }
    };

    /// <summary>
    /// 
    /// </summary>
    public sealed class TokenResultDto : TokenResultBaseDto
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TokenResultDto"/> class.
        /// </summary>
        public TokenResultDto()
        {

        }
        /// <summary>
        /// Gets or sets the refresh token.
        /// </summary>
        public TokenResultBaseDto RefreshToken { get; set; }

        /// <param name="RefreshToken">The refresh token.</param>
        /// <param name="Token">The token.</param>
        /// <param name="ExpiresAt">The expires in.</param>
        public TokenResultDto(TokenResultBaseDto RefreshToken, string Token, DateTime ExpiresAt) : base(Token, ExpiresAt)
        {
            this.RefreshToken = RefreshToken;
        }
    }
}
