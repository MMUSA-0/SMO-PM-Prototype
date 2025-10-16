using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Framework.Identity.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Insert Roles if they don't exist
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM [identity].[Roles] WHERE [Id] = '4684F03A-D163-4E92-AF57-069771C31E97')
                BEGIN
                    INSERT INTO [identity].[Roles] ([Id], [Code], [Name], [NormalizedName], [DisplayNameAr], [DisplayNameEn], [DescriptionAr], [DescriptionEn], [RoleGroup], [IsDefault], [CreatedBy], [CreatedOn], [ConcurrencyStamp])
                    VALUES ('4684F03A-D163-4E92-AF57-069771C31E97', 101, 'SuperAdmin', 'SUPERADMIN', N'مدير النظام', 'Administrator', N'مدير النظام', 'Administrator', NULL, 0, 'System', GETDATE(), '4b8eb291-eb4e-48e1-a755-dc08fba57b93')
                END

                IF NOT EXISTS (SELECT 1 FROM [identity].[Roles] WHERE [Id] = 'A3B0E143-6F4D-4C5B-9E2D-01D1D78D1111')
                BEGIN
                    INSERT INTO [identity].[Roles] ([Id], [Code], [Name], [NormalizedName], [DisplayNameAr], [DisplayNameEn], [DescriptionAr], [DescriptionEn], [RoleGroup], [IsDefault], [CreatedBy], [CreatedOn], [ConcurrencyStamp])
                    VALUES ('A3B0E143-6F4D-4C5B-9E2D-01D1D78D1111', 102, 'LearningPartner', 'LEARNINGPARTNER', N'شريك التعلم', 'Learning Partner', N'شريك التعلم', 'Learning Partner', NULL, 0, 'System', GETDATE(), '868b51bf-e66f-4a42-a6e2-2f4b75ae6156')
                END

                IF NOT EXISTS (SELECT 1 FROM [identity].[Roles] WHERE [Id] = 'B4C1F254-7A5E-4D6F-AF2E-02E2E89E2222')
                BEGIN
                    INSERT INTO [identity].[Roles] ([Id], [Code], [Name], [NormalizedName], [DisplayNameAr], [DisplayNameEn], [DescriptionAr], [DescriptionEn], [RoleGroup], [IsDefault], [CreatedBy], [CreatedOn], [ConcurrencyStamp])
                    VALUES ('B4C1F254-7A5E-4D6F-AF2E-02E2E89E2222', 103, 'Coaches', 'COACHES', N'١٠٠ مدرب', '100 Coaches', N'١٠٠ مدرب', '100 Coaches', NULL, 0, 'System', GETDATE(), 'a21ea675-bba8-454c-a2f1-e4cd307719e2')
                END

                IF NOT EXISTS (SELECT 1 FROM [identity].[Roles] WHERE [Id] = 'C5D2F365-8B6F-4E7F-BF3E-03F3F90F3333')
                BEGIN
                    INSERT INTO [identity].[Roles] ([Id], [Code], [Name], [NormalizedName], [DisplayNameAr], [DisplayNameEn], [DescriptionAr], [DescriptionEn], [RoleGroup], [IsDefault], [CreatedBy], [CreatedOn], [ConcurrencyStamp])
                    VALUES ('C5D2F365-8B6F-4E7F-BF3E-03F3F90F3333', 104, 'Participant', 'PARTICIPANT', N'مشارك', 'Participant', N'مشارك', 'Participant', NULL, 0, 'System', GETDATE(), 'ed2710a9-31d5-46df-8c37-1d2171322288')
                END
            ");

            // Insert Users if they don't exist
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM [identity].[Users] WHERE [Id] = '4684F03A-D163-4E92-AF57-069771C31E97')
                BEGIN
                    INSERT INTO [identity].[Users] ([Id], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnabled], [LockoutEnd], [AccessFailedCount], [FullNameEn], [FullNameAr], [TitleEn], [TitleAr], [DateOfBirth], [IsActive], [IdentityNo], [CreatedBy], [CreatedOn])
                    VALUES ('4684F03A-D163-4E92-AF57-069771C31E97', 'SuperAdmin@sure.com.sa', 'SUPERADMIN@SURE.COM.SA', 'SuperAdmin@sure.com.sa', 'SUPERADMIN@SURE.COM.SA', 1, 'AQAAAAIAAYagAAAAED1Z2w8n622iA0eJNayEeVPMU45yPeNJNUxsYVV6n+ysb559YSi1BO/zY02c/w7CNQ==', '4684f03a-d163-4e92-af57-069771c31e97', 'e4371cfb-cf19-4da6-b1e6-6e8d20b169be', '+9661234567890', 1, 0, 0, NULL, 0, 'System Administrator', N'مدير النظام', 'Mr.', N'السيد', NULL, 1, NULL, 'System', GETDATE())
                END

                IF NOT EXISTS (SELECT 1 FROM [identity].[Users] WHERE [Id] = 'A3B0E143-6F4D-4C5B-9E2D-01D1D78D1111')
                BEGIN
                    INSERT INTO [identity].[Users] ([Id], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnabled], [LockoutEnd], [AccessFailedCount], [FullNameEn], [FullNameAr], [TitleEn], [TitleAr], [DateOfBirth], [IsActive], [IdentityNo], [CreatedBy], [CreatedOn])
                    VALUES ('A3B0E143-6F4D-4C5B-9E2D-01D1D78D1111', 'learningPartner@sure.com.sa', 'LEARNINGPARTNER@SURE.COM.SA', 'learningPartner@sure.com.sa', 'LEARNINGPARTNER@SURE.COM.SA', 1, 'AQAAAAIAAYagAAAAEFJkLKN+pkXoQMLTQstDOtAqf+7UMmbdlp/aNiGtTE0wt4Y+WnxVZTJJALnMe0xQvQ==', 'a3b0e143-6f4d-4c5b-9e2d-01d1d78d1111', 'af63c4a4-6225-42f4-bac1-bd45c383e47d', '+9661234567891', 1, 0, 0, NULL, 0, 'Learning Partner Manager', N'مدير الشريك التعليمي', 'Mr.', N'السيد', NULL, 1, NULL, 'System', GETDATE())
                END

                IF NOT EXISTS (SELECT 1 FROM [identity].[Users] WHERE [Id] = 'B4C1F254-7A5E-4D6F-AF2E-02E2E89E2222')
                BEGIN
                    INSERT INTO [identity].[Users] ([Id], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnabled], [LockoutEnd], [AccessFailedCount], [FullNameEn], [FullNameAr], [TitleEn], [TitleAr], [DateOfBirth], [IsActive], [IdentityNo], [CreatedBy], [CreatedOn])
                    VALUES ('B4C1F254-7A5E-4D6F-AF2E-02E2E89E2222', 'coaches@sure.com.sa', 'COACHES@SURE.COM.SA', 'coaches@sure.com.sa', 'COACHES@SURE.COM.SA', 1, 'AQAAAAIAAYagAAAAECITB5GY1g1s0Q5WBJFAcVpZ6lcOguFG/Xk2PzntB8dVwY5laTLg4EaMA0G1N50yhQ==', 'b4c1f254-7a5e-4d6f-af2e-02e2e89e2222', 'e22b5738-8080-415b-a766-2a76a78fed0f', '+9661234567892', 1, 0, 0, NULL, 0, 'Senior Coach', N'المدرب الأول', 'Mr.', N'السيد', NULL, 1, NULL, 'System', GETDATE())
                END

                IF NOT EXISTS (SELECT 1 FROM [identity].[Users] WHERE [Id] = 'C5D2F365-8B6F-4E7F-BF3E-03F3F90F3333')
                BEGIN
                    INSERT INTO [identity].[Users] ([Id], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnabled], [LockoutEnd], [AccessFailedCount], [FullNameEn], [FullNameAr], [TitleEn], [TitleAr], [DateOfBirth], [IsActive], [IdentityNo], [CreatedBy], [CreatedOn])
                    VALUES ('C5D2F365-8B6F-4E7F-BF3E-03F3F90F3333', 'participant@sure.com.sa', 'PARTICIPANT@SURE.COM.SA', 'participant@sure.com.sa', 'PARTICIPANT@SURE.COM.SA', 1, 'AQAAAAIAAYagAAAAECXqZgHx2shJsj1YiirSCGJaRDHFNI/7J3u09C0m/Lg/9DRG6Cic8p212NHPDxJcmQ==', 'c5d2f365-8b6f-4e7f-bf3e-03f3f90f3333', '5ea62a09-c15c-4054-9eb7-4d810fdebe38', '+9661234567893', 1, 0, 0, NULL, 0, 'Demo Participant', N'المشارك التجريبي', 'Mr.', N'السيد', NULL, 1, NULL, 'System', GETDATE())
                END
            ");

            // Insert UserRoles if they don't exist
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM [identity].[UserRoles] WHERE [UserId] = '4684F03A-D163-4E92-AF57-069771C31E97' AND [RoleId] = '4684F03A-D163-4E92-AF57-069771C31E97')
                BEGIN
                    INSERT INTO [identity].[UserRoles] ([Id], [UserId], [RoleId], [CreatedBy], [CreatedOn])
                    VALUES (NEWID(), '4684F03A-D163-4E92-AF57-069771C31E97', '4684F03A-D163-4E92-AF57-069771C31E97', 'System', GETDATE())
                END

                IF NOT EXISTS (SELECT 1 FROM [identity].[UserRoles] WHERE [UserId] = 'A3B0E143-6F4D-4C5B-9E2D-01D1D78D1111' AND [RoleId] = 'A3B0E143-6F4D-4C5B-9E2D-01D1D78D1111')
                BEGIN
                    INSERT INTO [identity].[UserRoles] ([Id], [UserId], [RoleId], [CreatedBy], [CreatedOn])
                    VALUES (NEWID(), 'A3B0E143-6F4D-4C5B-9E2D-01D1D78D1111', 'A3B0E143-6F4D-4C5B-9E2D-01D1D78D1111', 'System', GETDATE())
                END

                IF NOT EXISTS (SELECT 1 FROM [identity].[UserRoles] WHERE [UserId] = 'B4C1F254-7A5E-4D6F-AF2E-02E2E89E2222' AND [RoleId] = 'B4C1F254-7A5E-4D6F-AF2E-02E2E89E2222')
                BEGIN
                    INSERT INTO [identity].[UserRoles] ([Id], [UserId], [RoleId], [CreatedBy], [CreatedOn])
                    VALUES (NEWID(), 'B4C1F254-7A5E-4D6F-AF2E-02E2E89E2222', 'B4C1F254-7A5E-4D6F-AF2E-02E2E89E2222', 'System', GETDATE())
                END

                IF NOT EXISTS (SELECT 1 FROM [identity].[UserRoles] WHERE [UserId] = 'C5D2F365-8B6F-4E7F-BF3E-03F3F90F3333' AND [RoleId] = 'C5D2F365-8B6F-4E7F-BF3E-03F3F90F3333')
                BEGIN
                    INSERT INTO [identity].[UserRoles] ([Id], [UserId], [RoleId], [CreatedBy], [CreatedOn])
                    VALUES (NEWID(), 'C5D2F365-8B6F-4E7F-BF3E-03F3F90F3333', 'C5D2F365-8B6F-4E7F-BF3E-03F3F90F3333', 'System', GETDATE())
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove UserRoles
            migrationBuilder.Sql(@"
                DELETE FROM [identity].[UserRoles] 
                WHERE [UserId] IN ('4684F03A-D163-4E92-AF57-069771C31E97', 'A3B0E143-6F4D-4C5B-9E2D-01D1D78D1111', 'B4C1F254-7A5E-4D6F-AF2E-02E2E89E2222', 'C5D2F365-8B6F-4E7F-BF3E-03F3F90F3333')
            ");

            // Remove Users
            migrationBuilder.Sql(@"
                DELETE FROM [identity].[Users] 
                WHERE [Id] IN ('4684F03A-D163-4E92-AF57-069771C31E97', 'A3B0E143-6F4D-4C5B-9E2D-01D1D78D1111', 'B4C1F254-7A5E-4D6F-AF2E-02E2E89E2222', 'C5D2F365-8B6F-4E7F-BF3E-03F3F90F3333')
            ");

            // Remove Roles
            migrationBuilder.Sql(@"
                DELETE FROM [identity].[Roles] 
                WHERE [Id] IN ('4684F03A-D163-4E92-AF57-069771C31E97', 'A3B0E143-6F4D-4C5B-9E2D-01D1D78D1111', 'B4C1F254-7A5E-4D6F-AF2E-02E2E89E2222', 'C5D2F365-8B6F-4E7F-BF3E-03F3F90F3333')
            ");
        }
    }
}
