using Framework.Core.Notifications;
using Framework.Core.SharedServices.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Framework.Core.SharedServices.Seed
{
    public class NotificationTemplateConfiguration : IEntityTypeConfiguration<NotificationTemplate>
    {
        public void Configure(EntityTypeBuilder<NotificationTemplate> builder)
        {
            builder.ToTable("NotificationTemplate", "common");
            builder.Property(e => e.Name).IsRequired().HasMaxLength(100);
            builder.Property(e => e.BodyAr).IsRequired();
            builder.Property(e => e.BodyEn).IsRequired(false);
            builder.Property(e => e.SubjectAr).IsRequired().HasMaxLength(256);
            builder.Property(e => e.SubjectEn).IsRequired(false).HasMaxLength(256);

            builder.HasOne(d => d.NotificationType)
                .WithMany(p => p.NotificationTemplates)
                .HasForeignKey(d => d.NotificationTypeId);
            builder.HasData(DataSeed());
        }

       
        private static List<NotificationTemplate> DataSeed()
        {
            List<NotificationTemplate> data = new List<NotificationTemplate>();
            data.Add(new NotificationTemplate
            {
                Id = (int)NotificationTemplatesEnum.RequestSubmitted,
                Name = NotificationTemplatesEnum.RequestSubmitted.ToString(),
                NotificationTypeId = (int)NotificationTypes.Email,
                CreatedBy = "Admin",
                CreatedOn = new DateTime(2024, 8, 12, 12, 1, 1, DateTimeKind.Utc),
                SubjectAr = "طلب التحقق الأمني",
                SubjectEn = "Security verification request",
                BodyAr = "لقد تم تقديم طلبك بنجاح، رقم الطلب: {RequestNumber}",
                BodyEn = "Your request has been submitted successfully, Order number: {RequestNumber}",
            });

            data.Add(new NotificationTemplate
            {
                Id = (int)NotificationTemplatesEnum.NewTaskAssigned,
                Name = NotificationTemplatesEnum.NewTaskAssigned.ToString(),
                NotificationTypeId = (int)NotificationTypes.Email,
                CreatedBy = "Admin",
                CreatedOn = new DateTime(2024, 8, 12, 12, 1, 1, DateTimeKind.Utc),
                SubjectEn = "New task assigned",
                SubjectAr = "تم استلام مهمة جديدة",
                BodyAr = "تم استلام مهمة جديدة في صندوق الوارد الخاص بك، لمزيد من المعلومات <a href='{RequestLink}' target='_blank'>انقر هنا</a>",
                BodyEn = "You have new request in your inbox, for more information's <a href='{RequestLink}' target='_blank'>Click here</a>",
            });

            data.Add(new NotificationTemplate
            {
                Id = (int)NotificationTemplatesEnum.ApprovalEmail,
                Name = NotificationTemplatesEnum.ApprovalEmail.ToString(),
                NotificationTypeId = (int)NotificationTypes.Email,
                CreatedBy = "Admin",
                CreatedOn = new DateTime(2024, 8, 12, 12, 1, 1, DateTimeKind.Utc),
                SubjectAr = "طلب التحقق الأمني",
                SubjectEn = "Security verification request",
                BodyAr = "تمت الموافقة على طلب التحقق الأمني ​​({RequestNumber}) لمزيد من المعلومات <a href='{RequestLink}' target='_blank'>انقر هنا</a>",
                BodyEn = "The Security verification request ({RequestNumber}) has been approved for more information <a href='{RequestLink}' target='_blank'>Click here</a>",
            });

            data.Add(new NotificationTemplate
            {
                Id = (int)NotificationTemplatesEnum.RejectionEmail,
                Name = NotificationTemplatesEnum.RejectionEmail.ToString(),
                NotificationTypeId = (int)NotificationTypes.Email,
                CreatedBy = "Admin",
                CreatedOn = new DateTime(2024, 8, 12, 12, 1, 1, DateTimeKind.Utc),
                SubjectAr = "طلب التحقق الأمني",
                SubjectEn = "Security verification request",
                BodyAr = "تم رفض طلب التحقق الأمني ​​({RequestNumber}) لمزيد من المعلومات <a href='{RequestLink}' target='_blank'>انقر هنا</a>",
                BodyEn = "The Security verification request ({RequestNumber}) has been rejected for more information <a href='{RequestLink}' target='_blank'>Click here</a>",
            });
            return data;
        }




    }
}
