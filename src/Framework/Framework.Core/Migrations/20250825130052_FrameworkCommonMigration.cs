using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Framework.Core.Migrations
{
    /// <inheritdoc />
    public partial class FrameworkCommonMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "common",
                table: "NotificationTemplate",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "SubjectAr", "SubjectEn" },
                values: new object[] { "تم استلام مهمة جديدة", "New task assigned" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "common",
                table: "NotificationTemplate",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "SubjectAr", "SubjectEn" },
                values: new object[] { "New task assigned", "تم استلام مهمة جديدة" });
        }
    }
}
