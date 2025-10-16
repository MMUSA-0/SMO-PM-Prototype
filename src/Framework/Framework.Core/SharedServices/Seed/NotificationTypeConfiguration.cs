using Framework.Core.Notifications;
using Framework.Core.SharedServices.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Framework.Core.SharedServices.Seed
{
    public class NotificationTypeConfiguration : IEntityTypeConfiguration<NotificationType>
    {
        public void Configure(EntityTypeBuilder<NotificationType> builder)
        {
            builder.ToTable("NotificationType", "common");
            builder.Property(e => e.NameAr).IsRequired().HasMaxLength(100);
            builder.Property(e => e.NameEn).IsRequired().HasMaxLength(100);
            builder.HasData(DataSeed());
            //base.Configure(builder);

        }

        private static List<NotificationType> DataSeed()
        {
            List<NotificationType> data = new List<NotificationType>();
            data.Add(new NotificationType
            {
                Id = (int)NotificationTypes.Email,
                NameAr = "بريد إلكتروني",
                NameEn = "Email",
                CreatedBy = "Admin",
                CreatedOn = new DateTime(2024, 8, 12, 12, 1, 1, DateTimeKind.Utc)
            });
            data.Add(new NotificationType
            {
                Id = (int)NotificationTypes.Sms,
                NameAr = "رسالة قصيرة",
                NameEn = "SMS",
                CreatedBy = "Admin",
                CreatedOn = new DateTime(2024, 8, 12, 12, 1, 1, DateTimeKind.Utc)
            });
            return data;
        }



    }
}
