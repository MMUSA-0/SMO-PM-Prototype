using System;
using Framework.Core;
using Framework.Identity.Data.Entities;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Framework.Identity.Data.Extensions
{
    public static class IdentityDbContextModelBuilderExtensions
    {
        public static void ConfigureIdentity(
            [NotNull] this ModelBuilder builder,
            Action<IdentityModelBuilderConfigurationOptions> optionsAction = null)
        {
            Check.NotNull(builder, nameof(builder));

            var options = new IdentityModelBuilderConfigurationOptions();

            optionsAction?.Invoke(options);

            builder.Entity<ApplicationUser>(b =>
            {
                b.ToTable("Users", options.Schema);

                // Primary key
                b.HasKey(u => u.Id);

                // A concurrency token for use with the optimistic concurrency checking
                b.Property(u => u.ConcurrencyStamp).IsConcurrencyToken();

                // Limit the size of columns to use efficient database types
                b.Property(u => u.FullNameEn).IsRequired().HasMaxLength(256);
                b.Property(u => u.FullNameAr).IsRequired().HasMaxLength(256);
                b.Property(u => u.UserName).IsRequired().HasMaxLength(256);
                b.Property(u => u.NormalizedUserName).IsRequired().HasMaxLength(256);
                b.Property(u => u.Email).IsRequired().HasMaxLength(256);
                b.Property(u => u.NormalizedEmail).IsRequired().HasMaxLength(256);
                b.Property(u => u.EmailConfirmed).HasDefaultValue(false);
                b.Property(u => u.PhoneNumber).HasMaxLength(50);
                b.Property(u => u.PhoneNumberConfirmed).HasDefaultValue(false);

                b.Property(u => u.PasswordHash).IsRequired().HasMaxLength(256);
                b.Property(u => u.SecurityStamp).IsRequired().HasMaxLength(256);
                b.Property(u => u.TwoFactorEnabled).IsRequired().HasDefaultValue(false);
                b.Property(u => u.LockoutEnabled).HasDefaultValue(false);
                b.Property(u => u.AccessFailedCount).IsRequired().HasDefaultValue(false);
                b.Property(u => u.IdentityNo).HasMaxLength(50);
                b.Property(u => u.ConcurrencyStamp).HasMaxLength(256);

                b.Property(u => u.TitleEn).IsRequired().HasMaxLength(25);
                b.Property(u => u.TitleAr).IsRequired().HasMaxLength(25);
                b.Property(u => u.IdentityNo).HasMaxLength(50);
                
                //b.Property(u => u.JobTitle).HasMaxLength(256);
                //b.Property(u => u.WorkNumber).HasMaxLength(256);

                b.Property(u => u.CreatedBy).IsRequired().HasMaxLength(256);
                b.Property(u => u.CreatedOn).IsRequired().HasDefaultValueSql("GetDate()");
                b.Property(u => u.UpdatedBy).HasMaxLength(256);


                // Indexes for "normalized" username and email, to allow efficient lookups
                b.HasIndex(u => u.NormalizedUserName).IsUnique();
                b.HasIndex(u => u.NormalizedEmail).IsUnique();
                b.HasIndex(u => u.UserName);
                b.HasIndex(u => u.Email);




                // The relationships between User and other entity types
                // Note that these relationships are configured with no navigation properties

                // Each User can have many UserClaims
                b.HasMany<IdentityUserClaim<Guid>>().WithOne().HasForeignKey(uc => uc.UserId).IsRequired();

                // Each User can have many UserLogins
                b.HasMany<IdentityUserLogin<Guid>>().WithOne().HasForeignKey(ul => ul.UserId).IsRequired();

                // Each User can have many UserTokens
                b.HasMany<IdentityUserToken<Guid>>().WithOne().HasForeignKey(ut => ut.UserId).IsRequired();

                // Each User can have many entries in the UserRole join table
                b.HasMany<ApplicationUserRoles>().WithOne().HasForeignKey(ur => ur.UserId).IsRequired();

            });

            builder.Entity<ApplicationRole>(b =>
            {
                b.ToTable("Roles", options.Schema);
                b.Property(r => r.Name).HasMaxLength(256).IsRequired();
                b.Property(r => r.NormalizedName).IsRequired().HasMaxLength(256);
                b.Property(r => r.RoleGroup).HasMaxLength(256);
                b.Property(r => r.ConcurrencyStamp).IsConcurrencyToken().HasMaxLength(256);

                b.Property(r => r.DisplayNameAr).HasMaxLength(256);
                b.Property(r => r.DisplayNameEn).HasMaxLength(256);
                b.Property(r => r.DescriptionAr).HasMaxLength(256).HasColumnName(nameof(ApplicationRole.DescriptionAr));
                b.Property(r => r.DescriptionEn).HasMaxLength(256).HasColumnName(nameof(ApplicationRole.DescriptionEn));
                b.Property(r => r.CreatedBy).IsRequired().HasMaxLength(256);
                b.Property(u => u.CreatedOn).IsRequired().HasDefaultValueSql("GetDate()");
                b.Property(r => r.UpdatedBy).HasMaxLength(256);

            });

            builder.Entity<ApplicationUserRoles>(b =>
            {
                b.ToTable("UserRoles", options.Schema);
                b.HasKey(u => u.Id);
                b.Property(r => r.CreatedBy).IsRequired().HasMaxLength(256);
                b.Property(u => u.CreatedOn).IsRequired().HasDefaultValueSql("GetDate()");
                b.Property(r => r.UpdatedBy).HasMaxLength(256);
                b.HasIndex(ur => new { ur.Id });

                b.HasOne(d => d.User)
                    .WithMany(d => d.UserRoles)
                    .HasForeignKey(ur => ur.UserId).IsRequired();
                b.HasOne(d => d.Role)
                    .WithMany(d => d.UserRoles)
                    .HasForeignKey(ur => ur.RoleId).IsRequired();
                
                b.HasIndex(ur => new { ur.UserId, ur.RoleId }).IsUnique();
            });

            builder.Entity<IdentityUserClaim<Guid>>(b =>
            {
                b.ToTable("UserClaims", options.Schema);
                b.Property(uc => uc.ClaimType).HasMaxLength(256).IsRequired();
                b.Property(uc => uc.ClaimValue).HasMaxLength(256);

                b.HasIndex(uc => uc.UserId);
            });

            builder.Entity<IdentityUserLogin<Guid>>(b =>
            {
                b.ToTable("UserLogins", options.Schema);

                b.HasKey(x => new { x.UserId, x.LoginProvider });
                b.Property(ul => ul.LoginProvider).HasMaxLength(256).IsRequired();
                b.Property(ul => ul.ProviderKey).HasMaxLength(256).IsRequired();
                b.Property(ul => ul.ProviderDisplayName).HasMaxLength(256);
                b.HasIndex(l => new { l.LoginProvider, l.ProviderKey });
            });

            builder.Entity<IdentityUserToken<Guid>>(b =>
            {
                b.ToTable("UserTokens", options.Schema);

                b.Property(r => r.Value).HasMaxLength(500);
                //b.HasKey(l => new { l.UserId, l.LoginProvider, l.Name });
                //b.Property(ul => ul.LoginProvider).HasMaxLength(256).IsRequired();
                //b.Property(ul => ul.Name).HasMaxLength(256).IsRequired();
            });

            builder.Entity<IdentityRoleClaim<Guid>>(b =>
            {
                b.ToTable("RoleClaims", options.Schema);
                b.Property(uc => uc.ClaimType).HasMaxLength(256).IsRequired();
                b.Property(uc => uc.ClaimValue).HasMaxLength(256);
                b.HasIndex(uc => uc.RoleId);
            });           
                        

            builder.Entity<UserOtp>(b =>
            {
                b.ToTable("UserOtp", options.Schema);
                b.HasKey(u => u.Id);

                b.Property(r => r.Email).HasMaxLength(256);
                b.Property(r => r.Mobile).HasMaxLength(256);
                b.Property(r => r.Otp).HasMaxLength(256);

                b.Property(r => r.CreatedBy).IsRequired().HasMaxLength(256);
                b.Property(u => u.CreatedOn).IsRequired().HasDefaultValueSql("GetDate()");
                b.Property(r => r.UpdatedBy).HasMaxLength(256);

                b.HasIndex(ur => new { ur.Id });
            });
        }
    }
}
