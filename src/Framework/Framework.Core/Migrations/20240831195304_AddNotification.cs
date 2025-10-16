using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Framework.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddNotification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "common",
                table: "NotificationType",
                columns: new[] { "Id", "CreatedBy", "CreatedOn", "NameAr", "NameEn", "UpdatedBy", "UpdatedOn" },
                values: new object[,]
                {
                    { 1, "Admin", new DateTime(2024, 8, 12, 12, 1, 1, 0, DateTimeKind.Utc), "بريد إلكتروني", "Email", null, null },
                    { 2, "Admin", new DateTime(2024, 8, 12, 12, 1, 1, 0, DateTimeKind.Utc), "رسالة قصيرة", "SMS", null, null }
                });

            migrationBuilder.InsertData(
                schema: "common",
                table: "NotificationTemplate",
                columns: new[] { "Id", "BodyAr", "BodyEn", "CreatedBy", "CreatedOn", "IsActive", "Name", "NotificationTypeId", "SubjectAr", "SubjectEn", "UpdatedBy", "UpdatedOn" },
                values: new object[,]
                {
                    { 1, "لقد تم تقديم طلبك بنجاح، رقم الطلب: {RequestNumber}", "Your request has been submitted successfully, Order number: {RequestNumber}", "Admin", new DateTime(2024, 8, 12, 12, 1, 1, 0, DateTimeKind.Utc), false, "RequestSubmitted", 1, "طلب التحقق الأمني", "Security verification request", null, null },
                    { 2, "تم استلام مهمة جديدة في صندوق الوارد الخاص بك، لمزيد من المعلومات <a href='{RequestLink}' target='_blank'>انقر هنا</a>", "You have new request in your inbox, for more information's <a href='{RequestLink}' target='_blank'>Click here</a>", "Admin", new DateTime(2024, 8, 12, 12, 1, 1, 0, DateTimeKind.Utc), false, "NewTaskAssigned", 1, "New task assigned", "تم استلام مهمة جديدة", null, null },
                    { 3, "تمت الموافقة على طلب التحقق الأمني ​​({RequestNumber}) لمزيد من المعلومات <a href='{RequestLink}' target='_blank'>انقر هنا</a>", "The Security verification request ({RequestNumber}) has been approved for more information <a href='{RequestLink}' target='_blank'>Click here</a>", "Admin", new DateTime(2024, 8, 12, 12, 1, 1, 0, DateTimeKind.Utc), false, "ApprovalEmail", 1, "طلب التحقق الأمني", "Security verification request", null, null },
                    { 4, "تم رفض طلب التحقق الأمني ​​({RequestNumber}) لمزيد من المعلومات <a href='{RequestLink}' target='_blank'>انقر هنا</a>", "The Security verification request ({RequestNumber}) has been rejected for more information <a href='{RequestLink}' target='_blank'>Click here</a>", "Admin", new DateTime(2024, 8, 12, 12, 1, 1, 0, DateTimeKind.Utc), false, "RejectionEmail", 1, "طلب التحقق الأمني", "Security verification request", null, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "common",
                table: "NotificationTemplate",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                schema: "common",
                table: "NotificationTemplate",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                schema: "common",
                table: "NotificationTemplate",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                schema: "common",
                table: "NotificationTemplate",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                schema: "common",
                table: "NotificationType",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                schema: "common",
                table: "NotificationType",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
