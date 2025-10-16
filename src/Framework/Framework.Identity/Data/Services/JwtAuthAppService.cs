using Framework.Core.Extensions;
using Framework.Core.Globalization;
using Framework.Identity.Data.Constants;
using Framework.Identity.Data.Dtos;
using Framework.Identity.Data.Entities;
using Framework.Identity.Recources;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Identity.Data.Services
{
    public class JwtAuthAppService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly UserRoleAppService _userRoleAppService;
        private readonly IConfiguration _configuration;
        private readonly UserTokensAppService _userTokensAppService;
        //private static readonly int _sessionDuration = 30;
        public JwtAuthAppService(IConfiguration configuration,
            UserTokensAppService userTokensAppService, UserRoleAppService userRoleAppService,
            UserManager<ApplicationUser> userManager)
        {
            _configuration = configuration;
            _userTokensAppService = userTokensAppService;
            _userRoleAppService = userRoleAppService;
            _userManager = userManager;
        }

        public async  Task<TokenResultDto> RefreshToken(TokenDto tokenDto)
        {
            var user = await _userManager.FindByIdAsync(tokenDto.UserId);
            if (user == null || !user.IsActive)
                throw new UnauthorizedAccessException(IdentityResources.YouAreNotAuthorized);

            var token = await GenerateJWTToken(user.UserName);
            var refreshToken = await GenerateJWTRefreshToken(user.UserName);

            return new TokenResultDto
            {
                Token = token.token,
                RefreshToken =new TokenResultBaseDto {Token= refreshToken.token,ExpiresAt = refreshToken.expiresOn } ,
                ExpiresAt = token.expiresOn
                
            };





            //var result = GetTokenPrincipal(tokenDto.AccessToken);

            ////var principal = result.principal;
            //var tokenValue = result.securityToken as JwtSecurityToken;

            //var usernameFromToken = tokenValue.Claims.FirstOrDefault(x => x.Type == "name")?.Value;

            //string? jtiFromToken = tokenValue.Claims.FirstOrDefault(claim => claim.Type == "jti")?.Value;
            //UserTokensDto savedRefreshToken = await _userTokensAppService.GetUserToken(usernameFromToken, jtiFromToken);
            //if (savedRefreshToken != null)
            //{
            //    if (savedRefreshToken.LoginProvider == jtiFromToken)
            //    {
            //        await _userTokensAppService.DeleteAsync(savedRefreshToken);
            //    }
            //}

            //var newAccessToken = await GenerateJWTToken(usernameFromToken);
            ////var newRefreshToken = GenerateRefreshToken();
            //return new TokenResultDto
            //{
            //    Token = newAccessToken.token,
            //    RefreshToken = newAccessToken.re
            //    ExpiresAt = newAccessToken.expiresOn
            //}; 
        }
        public async Task<(string token, DateTime expiresOn)> GenerateJWTToken(string username)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JWT:Key"]));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            _ = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JWT:Key"]));

            List<Claim> claim = new();
            ClaimDto userClaim = await GetClaimByUsername(username);
            if (userClaim != null && userClaim.UserRole != null && userClaim.UserRole.Count > 0)
            {
                claim.Add(new Claim("uid", userClaim.UserId.ToString()));
                claim.Add(new Claim("username", username));
                claim.Add(new Claim("email", userClaim.Email));                
                claim.Add(new Claim("fullname", userClaim.FullName));
                claim.Add(new Claim("fullnamear", userClaim.FullNameAr));
                
                
                //claim.Add(new Claim("sub", username));
                claim.Add(new Claim("nbf", DateTime.Now.AddHours(1).Ticks.ToString()));
                claim.Add(new Claim("iat", DateTime.Now.Ticks.ToString()));
                string JTI = Guid.NewGuid().ToString();
                claim.Add(new Claim("jti", JTI));
                if (userClaim != null)
                {
                    if (claim.FirstOrDefault(t => t.Type == ClaimTypes.NameIdentifier) != null)
                        claim.RemoveAll(t => t.Type == ClaimTypes.NameIdentifier);

                    claim.Add(new Claim(ClaimTypes.NameIdentifier, userClaim.UserId.ToString()));
                    if (userClaim.UserRole != null && userClaim.UserRole.Count > 0)
                    {
                        foreach (var role in userClaim.UserRole)
                        {
                            if (role != null && !string.IsNullOrEmpty(role.RoleName))
                            {
                                claim.Add(new Claim("role", role.RoleName));
                            }
                        }
                    }
                    await _userTokensAppService.InsertAsync(new UserTokensDto { UserId = userClaim.UserId, Name = username, LoginProvider = JTI });
                }

               DateTime tokenExipre = DateTime.Now.AddMinutes(JWTTokensConstants.TokenExpiry.Minutes);
                var tokenOptions = new JwtSecurityToken(issuer: _configuration["JWT:Issuer"],
                                                        audience: _configuration["JWT:Audience"],
                                                        claim,
                                                        notBefore: DateTime.UtcNow,
                                                        expires: tokenExipre,
                                                        signingCredentials);
                var tokenObj = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

                return (tokenObj, tokenExipre);
            }
            return (string.Empty, DateTime.UtcNow);
        }
        public async Task<(string token, DateTime expiresOn)> GenerateJWTRefreshToken(string username)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JWT:Key"]));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            _ = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JWT:Key"]));

            List<Claim> claim = [];
            ClaimDto userClaim = await GetClaimByUsername(username);
            if (userClaim != null && userClaim.UserRole != null && userClaim.UserRole.Count > 0)
            {
                claim.Add(new Claim("uid", userClaim.UserId.ToString()));
                claim.Add(new Claim("username", username));
                claim.Add(new Claim("email", userClaim.Email));
                claim.Add(new Claim("name", username));
                string JTI = Guid.NewGuid().ToString();
                claim.Add(new Claim("jti", JTI));
                if (userClaim != null)
                {
                    claim.Add(new Claim(ClaimTypes.NameIdentifier, userClaim.UserId.ToString()));
                    if (userClaim.UserRole != null && userClaim.UserRole.Count > 0)
                    {
                        foreach (var role in userClaim.UserRole)
                        {
                            if (role != null && !string.IsNullOrEmpty(role.RoleName))
                            {
                                claim.Add(new Claim("role", role.RoleName));
                            }
                        }
                    }
                    await _userTokensAppService.InsertAsync(new UserTokensDto
                    {
                        UserId = userClaim.UserId,
                        Name = username,
                        LoginProvider = JTI
                    });
                }

                DateTime tokenExipre = DateTime.UtcNow.AddDays(JWTTokensConstants.RefreshTokenExpiry.TotalDays);
                var tokenOptions = new JwtSecurityToken(issuer: _configuration["JWT:ValidIssuer"],
                                                        audience: _configuration["JWT:ValidAudience"],
                                                        claim,
                                                        notBefore: DateTime.UtcNow,
                                                        expires: tokenExipre,
                                                        signingCredentials);
                var tokenObj = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

                return (tokenObj, tokenExipre);
            }
            return (string.Empty, DateTime.UtcNow);
        }

        public async Task<string> GenerateActivationJWTToken(string username)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            List<Claim> claim = new List<Claim>();
            ClaimDto userClaim = await GetClaimByUsername(username);
            if (userClaim != null)
            {
                claim.Add(new Claim("name", username));
                claim.Add(new Claim("username", username));
                claim.Add(new Claim("sub", username));
                claim.Add(new Claim("nbf", DateTime.Now.AddDays(1).Ticks.ToString()));
                claim.Add(new Claim("iat", DateTime.Now.Ticks.ToString()));
                string JTI = Guid.NewGuid().ToString();
                claim.Add(new Claim("jti", JTI));
                if (userClaim != null)
                {
                    claim.Add(new Claim("nameidentifier", userClaim.UserId.ToString()));
                    await _userTokensAppService.InsertAsync(new UserTokensDto { UserId = userClaim.UserId, Name = username, LoginProvider = JTI, Value = "Activation" });
                }

                var tokenOptions = new JwtSecurityToken(_configuration["Jwt:Issuer"], _configuration["Jwt:Audience"], claim, null, DateTime.Now.AddDays(1), signingCredentials);
                var tokenObj = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

                return JTI;
            }
            return string.Empty;
        }

        public async Task<bool> DeleteUserToken(Guid UserId)
        {
            var UserObj = await _userManager.FindByIdAsync(UserId.ToString());
            if (UserObj != null)
            {
                await _userTokensAppService.DeleteAsync(UserId);
                return true;
            }
            return false;
        }

        public async Task<ClaimDto> GetClaimByUsername(string username)
        {
            ClaimDto claimUser = null;
            if (!string.IsNullOrEmpty(username))
            {
                var user = await _userManager.FindByNameAsync(username);
                if (user != null)
                {
                    claimUser = new ClaimDto
                    {
                        UserId = user.Id,
                        Email = user.Email,
                        FullName = user.FullNameEn , //CultureHelper.IsArabic? user.FullNameAr: user.FullNameEn,
                        FullNameAr = user.FullNameAr,
                        UserRole = await _userRoleAppService.GetClaimUserRolesList(user.Id)
                    };
                }
            }
            return claimUser;
        }

        public async Task<ClaimDto> GetClaimByEmail(string email)
        {
            ClaimDto claimUser = null;
            if (!string.IsNullOrEmpty(email))
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user != null)
                {
                    claimUser = new ClaimDto
                    {
                        UserId = user.Id,
                        Email = user.UserName,
                        FullName = user.FullNameEn ,//CultureHelper.IsArabic ? user.FullNameAr : user.FullNameEn,
                        FullNameAr = user.FullNameAr,
                        UserRole = await _userRoleAppService.GetClaimUserRolesList(user.Id)
                    };
                }
            }
            return claimUser;
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private TokenValidationParameters GetTokenValidationParameters()
        {
            var Key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidAudience = _configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Key),
                //ClockSkew = TimeSpan.Zero
            };
            return tokenValidationParameters;
        }
    }
}
