using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace SMO.Api.Filters
{
    /// <summary>
    /// Development filter to bypass authentication for testing
    /// WARNING: This should ONLY be used in development environment
    /// </summary>
    public class DevelopmentAuthBypassFilter : IAuthorizationFilter
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<DevelopmentAuthBypassFilter> _logger;

        public DevelopmentAuthBypassFilter(IConfiguration configuration, ILogger<DevelopmentAuthBypassFilter> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // Only bypass in Development environment
            var environment = context.HttpContext.RequestServices.GetService<IHostEnvironment>();
            if (environment?.IsDevelopment() != true)
            {
                return; // Don't bypass in non-development environments
            }

            // Check if bypass is enabled in configuration
            var bypassEnabled = _configuration.GetValue<bool>("DevelopmentSettings:BypassAuthentication");
            if (!bypassEnabled)
            {
                return; // Bypass not enabled
            }

            // Check if the endpoint allows anonymous access
            var endpoint = context.HttpContext.GetEndpoint();
            var allowAnonymous = endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null;
            if (allowAnonymous)
            {
                return; // Already allows anonymous
            }

            _logger.LogWarning("DEVELOPMENT MODE: Authentication bypassed for {Path}", context.HttpContext.Request.Path);

            // Create mock claims for development user
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, _configuration["DevelopmentSettings:MockUserId"] ?? "dev-001"),
                new Claim(ClaimTypes.Name, _configuration["DevelopmentSettings:MockUserName"] ?? "Dev User"),
                new Claim(ClaimTypes.Email, "dev@smo.gov.sa"),
                new Claim("DevelopmentMode", "true")
            };

            // Add mock roles from configuration
            var roles = _configuration.GetSection("DevelopmentSettings:MockUserRoles").Get<string[]>();
            if (roles != null)
            {
                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
            }

            // Create mock identity and principal
            var identity = new ClaimsIdentity(claims, "Development");
            var principal = new ClaimsPrincipal(identity);

            // Set the user on the HttpContext
            context.HttpContext.User = principal;
        }
    }
}