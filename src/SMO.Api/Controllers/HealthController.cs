using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SMO.Domain.Interfaces;
using System;
using System.Threading.Tasks;

namespace SMO.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly IAppDbContext _context;
        private readonly ILogger<HealthController> _logger;

        public HealthController(IAppDbContext context, ILogger<HealthController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Basic health check endpoint (no authentication required)
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetHealth()
        {
            try
            {
                // Check database connectivity
                var canConnect = await (_context as DbContext).Database.CanConnectAsync();
                
                if (canConnect)
                {
                    return Ok(new
                    {
                        status = "healthy",
                        timestamp = DateTime.UtcNow,
                        service = "SMO Vision Center API",
                        version = "1.0.0",
                        database = "connected"
                    });
                }
                else
                {
                    return StatusCode(503, new
                    {
                        status = "unhealthy",
                        timestamp = DateTime.UtcNow,
                        service = "SMO Vision Center API",
                        version = "1.0.0",
                        database = "disconnected",
                        error = "Database connection failed"
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Health check failed");
                return StatusCode(503, new
                {
                    status = "unhealthy",
                    timestamp = DateTime.UtcNow,
                    service = "SMO Vision Center API",
                    version = "1.0.0",
                    error = "Health check failed"
                });
            }
        }

        /// <summary>
        /// Detailed health check (requires authentication)
        /// </summary>
        [HttpGet("detailed")]
        [Authorize]
        public async Task<IActionResult> GetDetailedHealth()
        {
            try
            {
                var dbContext = _context as DbContext;
                var canConnect = await dbContext.Database.CanConnectAsync();
                
                // Get database statistics
                var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();
                var appliedMigrations = await dbContext.Database.GetAppliedMigrationsAsync();
                
                return Ok(new
                {
                    status = canConnect ? "healthy" : "unhealthy",
                    timestamp = DateTime.UtcNow,
                    service = "SMO Vision Center API",
                    version = "1.0.0",
                    environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
                    database = new
                    {
                        connected = canConnect,
                        provider = dbContext.Database.ProviderName,
                        pendingMigrations = pendingMigrations.Count(),
                        appliedMigrations = appliedMigrations.Count()
                    },
                    uptime = DateTime.UtcNow - System.Diagnostics.Process.GetCurrentProcess().StartTime.ToUniversalTime()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Detailed health check failed");
                return StatusCode(503, new
                {
                    status = "unhealthy",
                    timestamp = DateTime.UtcNow,
                    service = "SMO Vision Center API",
                    version = "1.0.0",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Simple ping endpoint for quick connectivity check
        /// </summary>
        [HttpGet("ping")]
        [AllowAnonymous]
        public IActionResult Ping()
        {
            return Ok(new { message = "pong", timestamp = DateTime.UtcNow });
        }
    }
}