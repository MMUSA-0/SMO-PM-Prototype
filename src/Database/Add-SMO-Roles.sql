-- =============================================
-- Add SMO-Specific Roles to Identity System
-- Version: 1.0
-- Date: December 2024
-- Description: Adds SMO platform specific roles
-- =============================================

USE SMO_Production;
GO

-- Check if Roles table exists in Identity schema
IF OBJECT_ID('dbo.Roles', 'U') IS NOT NULL
BEGIN
    -- Insert SMO-specific roles if they don't already exist
    IF NOT EXISTS (SELECT 1 FROM dbo.Roles WHERE Name = 'ExecutiveOffice')
    BEGIN
        INSERT INTO dbo.Roles (Id, Code, Name, NormalizedName, DisplayNameEn, DisplayNameAr, 
                              DescriptionEn, DescriptionAr, RoleGroup, IsDefault, 
                              CreatedBy, CreatedOn, ConcurrencyStamp)
        VALUES (NEWID(), 10, 'ExecutiveOffice', 'EXECUTIVEOFFICE', 
                'Executive Office', N'المكتب التنفيذي',
                'Executive Office with full administrative privileges', 
                N'المكتب التنفيذي مع كامل الصلاحيات الإدارية',
                'Executive', 0, 'System', GETUTCDATE(), NEWID());
    END

    IF NOT EXISTS (SELECT 1 FROM dbo.Roles WHERE Name = 'VRO')
    BEGIN
        INSERT INTO dbo.Roles (Id, Code, Name, NormalizedName, DisplayNameEn, DisplayNameAr, 
                              DescriptionEn, DescriptionAr, RoleGroup, IsDefault, 
                              CreatedBy, CreatedOn, ConcurrencyStamp)
        VALUES (NEWID(), 11, 'VRO', 'VRO', 
                'Vision Realization Office', N'مكتب تحقيق الرؤية',
                'Vision Realization Office for strategic oversight', 
                N'مكتب تحقيق الرؤية للإشراف الاستراتيجي',
                'Strategic', 0, 'System', GETUTCDATE(), NEWID());
    END

    IF NOT EXISTS (SELECT 1 FROM dbo.Roles WHERE Name = 'VRP')
    BEGIN
        INSERT INTO dbo.Roles (Id, Code, Name, NormalizedName, DisplayNameEn, DisplayNameAr, 
                              DescriptionEn, DescriptionAr, RoleGroup, IsDefault, 
                              CreatedBy, CreatedOn, ConcurrencyStamp)
        VALUES (NEWID(), 12, 'VRP', 'VRP', 
                'Vision Realization Program', N'برنامج تحقيق الرؤية',
                'Vision Realization Program management', 
                N'إدارة برنامج تحقيق الرؤية',
                'Program', 0, 'System', GETUTCDATE(), NEWID());
    END

    IF NOT EXISTS (SELECT 1 FROM dbo.Roles WHERE Name = 'SMO')
    BEGIN
        INSERT INTO dbo.Roles (Id, Code, Name, NormalizedName, DisplayNameEn, DisplayNameAr, 
                              DescriptionEn, DescriptionAr, RoleGroup, IsDefault, 
                              CreatedBy, CreatedOn, ConcurrencyStamp)
        VALUES (NEWID(), 13, 'SMO', 'SMO', 
                'Strategic Management Office', N'مكتب الإدارة الاستراتيجية',
                'Strategic Management Office for performance management', 
                N'مكتب الإدارة الاستراتيجية لإدارة الأداء',
                'Management', 0, 'System', GETUTCDATE(), NEWID());
    END

    IF NOT EXISTS (SELECT 1 FROM dbo.Roles WHERE Name = 'ADAA')
    BEGIN
        INSERT INTO dbo.Roles (Id, Code, Name, NormalizedName, DisplayNameEn, DisplayNameAr, 
                              DescriptionEn, DescriptionAr, RoleGroup, IsDefault, 
                              CreatedBy, CreatedOn, ConcurrencyStamp)
        VALUES (NEWID(), 14, 'ADAA', 'ADAA', 
                'National Center for Performance Management (ADAA)', N'المركز الوطني لقياس الأداء (أداء)',
                'National Center for Government Performance Management', 
                N'المركز الوطني لقياس أداء الأجهزة العامة',
                'Performance', 0, 'System', GETUTCDATE(), NEWID());
    END

    IF NOT EXISTS (SELECT 1 FROM dbo.Roles WHERE Name = 'ProgramOwner')
    BEGIN
        INSERT INTO dbo.Roles (Id, Code, Name, NormalizedName, DisplayNameEn, DisplayNameAr, 
                              DescriptionEn, DescriptionAr, RoleGroup, IsDefault, 
                              CreatedBy, CreatedOn, ConcurrencyStamp)
        VALUES (NEWID(), 15, 'ProgramOwner', 'PROGRAMOWNER', 
                'Program Owner', N'مالك البرنامج',
                'Owner of specific Vision 2030 programs', 
                N'مالك برامج رؤية 2030 المحددة',
                'Program', 0, 'System', GETUTCDATE(), NEWID());
    END

    IF NOT EXISTS (SELECT 1 FROM dbo.Roles WHERE Name = 'InitiativeOwner')
    BEGIN
        INSERT INTO dbo.Roles (Id, Code, Name, NormalizedName, DisplayNameEn, DisplayNameAr, 
                              DescriptionEn, DescriptionAr, RoleGroup, IsDefault, 
                              CreatedBy, CreatedOn, ConcurrencyStamp)
        VALUES (NEWID(), 16, 'InitiativeOwner', 'INITIATIVEOWNER', 
                'Initiative Owner', N'مالك المبادرة',
                'Owner of specific initiatives', 
                N'مالك المبادرات المحددة',
                'Initiative', 0, 'System', GETUTCDATE(), NEWID());
    END

    IF NOT EXISTS (SELECT 1 FROM dbo.Roles WHERE Name = 'PerformanceManager')
    BEGIN
        INSERT INTO dbo.Roles (Id, Code, Name, NormalizedName, DisplayNameEn, DisplayNameAr, 
                              DescriptionEn, DescriptionAr, RoleGroup, IsDefault, 
                              CreatedBy, CreatedOn, ConcurrencyStamp)
        VALUES (NEWID(), 17, 'PerformanceManager', 'PERFORMANCEMANAGER', 
                'Performance Manager', N'مدير الأداء',
                'Manages performance evaluations and KPIs', 
                N'يدير تقييمات الأداء ومؤشرات الأداء الرئيسية',
                'Performance', 0, 'System', GETUTCDATE(), NEWID());
    END

    IF NOT EXISTS (SELECT 1 FROM dbo.Roles WHERE Name = 'DataAnalyst')
    BEGIN
        INSERT INTO dbo.Roles (Id, Code, Name, NormalizedName, DisplayNameEn, DisplayNameAr, 
                              DescriptionEn, DescriptionAr, RoleGroup, IsDefault, 
                              CreatedBy, CreatedOn, ConcurrencyStamp)
        VALUES (NEWID(), 18, 'DataAnalyst', 'DATAANALYST', 
                'Data Analyst', N'محلل البيانات',
                'Analyzes performance data and generates reports', 
                N'يحلل بيانات الأداء وينشئ التقارير',
                'Analytics', 0, 'System', GETUTCDATE(), NEWID());
    END

    -- Add a sample user with multiple roles (for testing)
    -- This creates a test user if Users table exists
    IF OBJECT_ID('dbo.Users', 'U') IS NOT NULL
    BEGIN
        DECLARE @UserId UNIQUEIDENTIFIER = NEWID();
        DECLARE @UserName NVARCHAR(256) = 'mohammed.ahmed';
        
        IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE UserName = @UserName)
        BEGIN
            INSERT INTO dbo.Users (Id, UserName, NormalizedUserName, Email, NormalizedEmail,
                                  EmailConfirmed, PasswordHash, SecurityStamp, ConcurrencyStamp,
                                  PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled,
                                  AccessFailedCount, FullNameEn, FullNameAr, TitleEn, TitleAr,
                                  IsActive, CreatedBy, CreatedOn)
            VALUES (@UserId, @UserName, UPPER(@UserName), 'mohammed.ahmed@smo.gov.sa', 
                   UPPER('mohammed.ahmed@smo.gov.sa'), 1, 
                   -- Password: P@ssw0rd123 (this is a hash, in production use proper hashing)
                   'AQAAAAEAACcQAAAAEH3hJ2kSJV6VkBwzJ+zKZYXM8HjmcNv6cUYoNkOQ8vUfPQ==',
                   NEWID(), NEWID(), '+966501234567', 1, 0, 1, 0,
                   'Mohammed Ahmed Al-Salem', N'محمد أحمد السالم',
                   'Performance Management Director', N'مدير إدارة الأداء',
                   1, 'System', GETUTCDATE());
            
            PRINT 'Test user created: mohammed.ahmed';
            
            -- Assign multiple roles to the test user
            IF OBJECT_ID('dbo.UserRoles', 'U') IS NOT NULL
            BEGIN
                DECLARE @RoleId UNIQUEIDENTIFIER;
                
                -- Assign SMO role
                SELECT @RoleId = Id FROM dbo.Roles WHERE Name = 'SMO';
                IF @RoleId IS NOT NULL
                BEGIN
                    INSERT INTO dbo.UserRoles (Id, UserId, RoleId, CreatedBy, CreatedOn)
                    VALUES (NEWID(), @UserId, @RoleId, 'System', GETUTCDATE());
                END
                
                -- Assign VRO role
                SELECT @RoleId = Id FROM dbo.Roles WHERE Name = 'VRO';
                IF @RoleId IS NOT NULL
                BEGIN
                    INSERT INTO dbo.UserRoles (Id, UserId, RoleId, CreatedBy, CreatedOn)
                    VALUES (NEWID(), @UserId, @RoleId, 'System', GETUTCDATE());
                END
                
                -- Assign PerformanceManager role
                SELECT @RoleId = Id FROM dbo.Roles WHERE Name = 'PerformanceManager';
                IF @RoleId IS NOT NULL
                BEGIN
                    INSERT INTO dbo.UserRoles (Id, UserId, RoleId, CreatedBy, CreatedOn)
                    VALUES (NEWID(), @UserId, @RoleId, 'System', GETUTCDATE());
                END
                
                -- Assign ADAA role
                SELECT @RoleId = Id FROM dbo.Roles WHERE Name = 'ADAA';
                IF @RoleId IS NOT NULL
                BEGIN
                    INSERT INTO dbo.UserRoles (Id, UserId, RoleId, CreatedBy, CreatedOn)
                    VALUES (NEWID(), @UserId, @RoleId, 'System', GETUTCDATE());
                END
                
                PRINT 'Multiple roles assigned to test user';
            END
        END
        ELSE
        BEGIN
            PRINT 'Test user already exists';
        END
    END
    
    PRINT 'SMO roles added successfully!';
    
    -- Display all roles
    SELECT Code, Name, DisplayNameAr, DisplayNameEn, RoleGroup 
    FROM dbo.Roles 
    ORDER BY Code;
END
ELSE
BEGIN
    PRINT 'Roles table not found. Please ensure Identity tables are created first.';
END
GO


