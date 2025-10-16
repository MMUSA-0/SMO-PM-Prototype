using Framework.Core.Data;
using Framework.Identity.Data.Entities;
using Framework.Identity.Data.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Framework.Identity.Data
{
    public class AppIdentityDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid, IdentityUserClaim<Guid>,
        ApplicationUserRoles, IdentityUserLogin<Guid>, IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>, IBaseDbContext
    {
        public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options, IHttpContextAccessor httpContextAccessor)
            : base(options)
        {
            //CurrentUserName = httpContextAccessor?.HttpContext?.User?.Identity?.Name;
            CurrentUserName = ((System.Security.Claims.ClaimsIdentity)httpContextAccessor?.HttpContext?.User?.Identity)?.FindFirst("name")?.Value;

        }

        public string CurrentUserName { get; set; }
        public static string Schema { get; set; } = "identity";


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ConfigureIdentity(options =>
            {
                options.Schema = Schema;
            });

            // Removed seed data configurations to prevent pending model changes warning
            // Data seeding is now handled by DatabaseInitializer
            // builder.ApplyConfiguration(new RoleConfiguration());
            // builder.ApplyConfiguration(new AdminConfiguration());
            // builder.ApplyConfiguration(new UsersWithRolesConfig());
        }

        public override int SaveChanges()
        {
            try
            {
                ChangeTracker.SetShadowProperties(CurrentUserName);
                ChangeTracker.Validate();

                return base.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new Exception(ex.Message, ex);
            }
            finally
            {
                ChangeTracker.AutoDetectChangesEnabled = true;
            }
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                ChangeTracker.SetShadowProperties(CurrentUserName);
                ChangeTracker.Validate();

                return await base.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            finally
            {
                ChangeTracker.AutoDetectChangesEnabled = true;
            }
        }

        public Task<int> SaveChangesWithAuditAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
