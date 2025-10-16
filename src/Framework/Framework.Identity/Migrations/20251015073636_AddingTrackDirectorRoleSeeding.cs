using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Framework.Identity.Migrations
{
    /// <inheritdoc />
    public partial class AddingTrackDirectorRoleSeeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Insert Roles if they don't exist
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM [identity].[Roles] WHERE [Id] = 'f6fe3d61-e40e-4c5d-b428-6e1711525563')
                BEGIN
                    INSERT INTO [identity].[Roles] ([Id], [Code], [Name], [NormalizedName], [DisplayNameAr], [DisplayNameEn], [DescriptionAr], [DescriptionEn], [RoleGroup], [IsDefault], [CreatedBy], [CreatedOn], [ConcurrencyStamp])
                    VALUES ('f6fe3d61-e40e-4c5d-b428-6e1711525563', 105, 'TrackDirector', 'TRACKDIRECTOR', N'مدير المسار', 'TrackDirector', N'مدير المسار', 'TrackDirector', NULL, 0, 'System', GETDATE(), 'ae587e63-e408-4c33-aa7c-a006fae53de7')
                END
 
            ");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove Roles
            migrationBuilder.Sql(@"
                DELETE FROM [identity].[Roles] 
                WHERE [Id] IN ('f6fe3d61-e40e-4c5d-b428-6e1711525563')
            ");
        }
    }
}
