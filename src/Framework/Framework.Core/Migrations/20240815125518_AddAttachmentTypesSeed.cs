using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Framework.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddAttachmentTypesSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "common",
                table: "AttachmentType",
                columns: new[] { "Id", "AllowedFilesExtension", "Code", "CreatedBy", "CreatedOn", "ImageMaxHeight", "ImageMaxWidth", "IsImage", "IsMandatory", "MaxSizeInMegabytes", "NameAr", "NameEn", "UpdatedBy", "UpdatedOn" },
                values: new object[] { 1, ".jpg,.jpeg,.png,.tif,.tiff,.gif,.bmp,.mp3,.wav,.txt,.rtf,.pdf,.csv,.xls,.xlsx,.xlt,.xltx,.doc,.docx,.dot,.dotx,.ppt,.ppsx,.pptx,.sldx,.zip,.rar", "General", "System", new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, false, false, 15, "عام", "General", null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "common",
                table: "AttachmentType",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
