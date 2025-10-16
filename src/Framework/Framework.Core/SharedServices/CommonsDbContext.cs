using Framework.Core.Data;
using Framework.Core.SharedServices.Entities;
using Framework.Core.SharedServices.Seed;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json.Serialization;

namespace Framework.Core.SharedServices
{
    /// <summary>
    ///     The commons db entities.
    /// </summary>
    public partial class CommonsDbContext : BaseDbContext<CommonsDbContext>, ICommonsDbContext
    {

        public CommonsDbContext(DbContextOptions<CommonsDbContext> options, IHttpContextAccessor httpContextAccessor)
            : base(options)
        {
            CurrentUserName = httpContextAccessor?.HttpContext?.User?.Identity?.Name;
        }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Attachment>(entity =>
            {
                entity.ToTable("Attachment", "common");
                entity.Property(e => e.ContentType).HasMaxLength(100);
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.DescriptionAr).HasMaxLength(500);
                entity.Property(e => e.DescriptionEn).HasMaxLength(500);
                entity.Property(e => e.Extension).IsRequired().HasMaxLength(500);
                entity.Property(e => e.FileName).IsRequired().HasMaxLength(255);
                entity.Property(e => e.FilePath).HasMaxLength(255);
                entity.Property(e => e.TitleAr).HasMaxLength(255);
                entity.Property(e => e.TitleEn).HasMaxLength(255);
                entity.HasOne(d => d.AttachmentType)
                    .WithMany(p => p.Attachments)
                    .HasForeignKey(d => d.AttachmentTypeId);

            });


            modelBuilder.Entity<AttachmentType>(entity =>
            {
                entity.ToTable("AttachmentType", "common");
                entity.Property(e => e.AllowedFilesExtension).HasMaxLength(200);
                entity.Property(e => e.Code).IsRequired();
                entity.Property(e => e.MaxSizeInMegabytes).HasDefaultValueSql("((1))");
                entity.Property(e => e.NameAr).IsRequired().HasMaxLength(256);
                entity.Property(e => e.NameEn).IsRequired().HasMaxLength(256);
                entity.HasData(new AttachmentType
                {
                    Id = 1,
                    Code = "General",
                    NameEn = "General",
                    NameAr = "عام",
                    AllowedFilesExtension = ".jpg,.jpeg,.png,.tif,.tiff,.gif,.bmp,.mp3,.wav,.txt,.rtf,.pdf,.csv,.xls,.xlsx,.xlt,.xltx,.doc,.docx,.dot,.dotx,.ppt,.ppsx,.pptx,.sldx,.zip,.rar",
                    IsMandatory = false,
                    IsImage = false,
                    MaxSizeInMegabytes = 15,
                    CreatedBy = "System",
                    CreatedOn = new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified)
                },
                new AttachmentType
                {
                    Id = 2,
                    Code = "Recommendation",
                    NameEn = "Recommendation",
                    NameAr = "توصية",
                    AllowedFilesExtension = ".jpg,.jpeg,.png,.tif,.tiff,.gif,.bmp,.mp3,.wav,.txt,.rtf,.pdf,.csv,.xls,.xlsx,.xlt,.xltx,.doc,.docx,.dot,.dotx,.ppt,.ppsx,.pptx,.sldx,.zip,.rar",
                    IsMandatory = false,
                    IsImage = false,
                    MaxSizeInMegabytes = 15,
                    CreatedBy = "System",
                    CreatedOn = new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified)
                },
                new AttachmentType
                {
                    Id = 3,
                    Code = "Decision",
                    NameEn = "Decision",
                    NameAr = "مرفق اتخاذ القرار",
                    AllowedFilesExtension = ".jpg,.jpeg,.png,.tif,.tiff,.gif,.bmp,.mp3,.wav,.txt,.rtf,.pdf,.csv,.xls,.xlsx,.xlt,.xltx,.doc,.docx,.dot,.dotx,.ppt,.ppsx,.pptx,.sldx,.zip,.rar",
                    IsMandatory = false,
                    IsImage = false,
                    MaxSizeInMegabytes = 15,
                    CreatedBy = "System",
                    CreatedOn = new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified)
                });
            });


            modelBuilder.Entity<Log>(entity =>
            {
                entity.ToTable("Log", "common");
                entity.Property(e => e.Id);
                entity.Property(e => e.UserAgent).HasMaxLength(256).IsUnicode(false);
                entity.Property(e => e.Exception).HasMaxLength(6000).IsUnicode(false);
                //entity.Property(e => e.HttpMethod).HasMaxLength(256).IsUnicode(false);
                entity.Property(e => e.Host).HasMaxLength(256);
                entity.Property(e => e.LogLevel).IsRequired(false).HasMaxLength(50).IsUnicode(false);
                entity.Property(e => e.Logger).IsRequired(false).HasMaxLength(256).IsUnicode(false);
                entity.Property(e => e.Message).IsRequired(false).HasMaxLength(4000).IsUnicode(false);
                entity.Property(e => e.Thread).IsRequired(false).HasMaxLength(256).IsUnicode(false);
                entity.Property(e => e.Url).HasMaxLength(500).IsUnicode(false);
                entity.Property(e => e.UserName).HasMaxLength(256).IsUnicode(false);
            });

            //modelBuilder.Entity<NotificationTemplate>(entity =>
            //{
            //    entity.ToTable("NotificationTemplate", "common");
            //    entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            //    entity.Property(e => e.BodyAr).IsRequired();
            //    entity.Property(e => e.BodyEn).IsRequired(false);
            //    entity.Property(e => e.SubjectAr).IsRequired().HasMaxLength(256);
            //    entity.Property(e => e.SubjectEn).IsRequired(false).HasMaxLength(256);

            //    entity.HasOne(d => d.NotificationType)
            //        .WithMany(p => p.NotificationTemplates)
            //        .HasForeignKey(d => d.NotificationTypeId);

            //});


            //modelBuilder.Entity<NotificationType>(entity =>
            //{
            //    entity.ToTable("NotificationType", "common");
            //    entity.Property(e => e.NameAr).IsRequired().HasMaxLength(100);
            //    entity.Property(e => e.NameEn).IsRequired().HasMaxLength(100);
            //});
            modelBuilder.ApplyConfiguration(new NotificationTypeConfiguration());

            modelBuilder.ApplyConfiguration(new NotificationTemplateConfiguration());




            modelBuilder.Entity<SystemSetting>(entity =>
            {
                entity.ToTable("SystemSetting", "common");
                entity.Property(e => e.GroupName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Value).IsRequired().HasMaxLength(255);
                entity.Property(e => e.IsSticky).IsRequired();
                entity.Property(e => e.IsSecure).IsRequired();
                entity.Property(e => e.ValueType).IsRequired().HasMaxLength(30).IsUnicode(false);
            });
            //modelBuilder.Entity<SystemSetting>().HasData(this.GetSystemSettingSeedData());

            base.OnModelCreating(modelBuilder);
        }

        public virtual DbSet<Attachment> Attachments { get; set; }
        //public virtual DbSet<AttachmentContent> AttachmentContents { get; set; }
        public virtual DbSet<AttachmentType> AttachmentTypes { get; set; }
        public virtual DbSet<Log> Logs { get; set; }
        public virtual DbSet<NotificationTemplate> NotificationTemplates { get; set; }
        public virtual DbSet<SystemSetting> SystemSettings { get; set; }
        public virtual DbSet<NotificationType> NotificationTypes { get; set; }

        //private List<SystemSetting> GetSystemSettingSeedData()
        //{
        //    string seedData = File.ReadAllText(@"SystemSettingsSeedData.json");
        //    List<SystemSetting> settings = JsonConvert.DeserializeObject<List<SystemSetting>>(seedData);
        //    return settings;
        //}

    }
}