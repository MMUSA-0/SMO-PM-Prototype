-- =============================================
-- SMO Platform Database Creation Script
-- Version: 1.0
-- Date: December 2024
-- Description: Complete database schema for SMO Platform
-- =============================================

-- Create Database (Run this first separately if needed)
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'SMO_Production')
BEGIN
    CREATE DATABASE SMO_Production;
END
GO

USE SMO_Production;
GO

-- =============================================
-- 1. VISION TABLES
-- =============================================

IF OBJECT_ID('dbo.Visions', 'U') IS NOT NULL DROP TABLE dbo.Visions;
CREATE TABLE dbo.Visions (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Code NVARCHAR(50) UNIQUE,
    Title NVARCHAR(500) NOT NULL,
    TitleAr NVARCHAR(500),
    Description NVARCHAR(MAX),
    DescriptionAr NVARCHAR(MAX),
    StartDate DATE,
    EndDate DATE,
    Status NVARCHAR(50) DEFAULT 'Draft',
    Progress DECIMAL(5,2) DEFAULT 0,
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME2 DEFAULT GETUTCDATE(),
    CreatedBy NVARCHAR(256),
    ModifiedDate DATETIME2,
    ModifiedBy NVARCHAR(256)
);

-- =============================================
-- 2. PROGRAM TABLES
-- =============================================

IF OBJECT_ID('dbo.Programs', 'U') IS NOT NULL DROP TABLE dbo.Programs;
CREATE TABLE dbo.Programs (
    Id INT PRIMARY KEY IDENTITY(1,1),
    VisionId INT FOREIGN KEY REFERENCES Visions(Id),
    Code NVARCHAR(50) UNIQUE,
    Title NVARCHAR(500) NOT NULL,
    TitleAr NVARCHAR(500),
    Description NVARCHAR(MAX),
    DescriptionAr NVARCHAR(MAX),
    Budget DECIMAL(18,2),
    AllocatedBudget DECIMAL(18,2),
    SpentBudget DECIMAL(18,2),
    StartDate DATE,
    EndDate DATE,
    Status NVARCHAR(50) DEFAULT 'Draft',
    Progress DECIMAL(5,2) DEFAULT 0,
    Priority NVARCHAR(20),
    Owner NVARCHAR(256),
    Department NVARCHAR(256),
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME2 DEFAULT GETUTCDATE(),
    CreatedBy NVARCHAR(256),
    ModifiedDate DATETIME2,
    ModifiedBy NVARCHAR(256)
);

-- =============================================
-- 3. INITIATIVE TABLES
-- =============================================

IF OBJECT_ID('dbo.Initiatives', 'U') IS NOT NULL DROP TABLE dbo.Initiatives;
CREATE TABLE dbo.Initiatives (
    Id INT PRIMARY KEY IDENTITY(1,1),
    ProgramId INT FOREIGN KEY REFERENCES Programs(Id),
    Code NVARCHAR(50) UNIQUE,
    Title NVARCHAR(500) NOT NULL,
    TitleAr NVARCHAR(500),
    Description NVARCHAR(MAX),
    DescriptionAr NVARCHAR(MAX),
    Budget DECIMAL(18,2),
    AllocatedBudget DECIMAL(18,2),
    SpentBudget DECIMAL(18,2),
    StartDate DATE,
    EndDate DATE,
    ActualStartDate DATE,
    ActualEndDate DATE,
    Status NVARCHAR(50) DEFAULT 'Draft',
    Progress DECIMAL(5,2) DEFAULT 0,
    Priority NVARCHAR(20),
    Type NVARCHAR(50),
    Category NVARCHAR(100),
    Owner NVARCHAR(256),
    Department NVARCHAR(256),
    Sponsor NVARCHAR(256),
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME2 DEFAULT GETUTCDATE(),
    CreatedBy NVARCHAR(256),
    ModifiedDate DATETIME2,
    ModifiedBy NVARCHAR(256)
);

-- =============================================
-- 4. KPI TABLES
-- =============================================

IF OBJECT_ID('dbo.KPIs', 'U') IS NOT NULL DROP TABLE dbo.KPIs;
CREATE TABLE dbo.KPIs (
    Id INT PRIMARY KEY IDENTITY(1,1),
    InitiativeId INT FOREIGN KEY REFERENCES Initiatives(Id),
    ProgramId INT FOREIGN KEY REFERENCES Programs(Id),
    Code NVARCHAR(50) UNIQUE,
    Name NVARCHAR(500) NOT NULL,
    NameAr NVARCHAR(500),
    Description NVARCHAR(MAX),
    DescriptionAr NVARCHAR(MAX),
    Category NVARCHAR(100),
    Unit NVARCHAR(50),
    UnitAr NVARCHAR(50),
    BaselineValue DECIMAL(18,4),
    TargetValue DECIMAL(18,4),
    ActualValue DECIMAL(18,4),
    Weight DECIMAL(5,2),
    Frequency NVARCHAR(50), -- Daily, Weekly, Monthly, Quarterly, Yearly
    MeasurementMethod NVARCHAR(MAX),
    DataSource NVARCHAR(256),
    Status NVARCHAR(50),
    Trend NVARCHAR(20), -- Up, Down, Stable
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME2 DEFAULT GETUTCDATE(),
    CreatedBy NVARCHAR(256),
    ModifiedDate DATETIME2,
    ModifiedBy NVARCHAR(256)
);

-- =============================================
-- 5. KPI VALUES (Historical tracking)
-- =============================================

IF OBJECT_ID('dbo.KPIValues', 'U') IS NOT NULL DROP TABLE dbo.KPIValues;
CREATE TABLE dbo.KPIValues (
    Id INT PRIMARY KEY IDENTITY(1,1),
    KPIId INT FOREIGN KEY REFERENCES KPIs(Id),
    Period DATE NOT NULL,
    TargetValue DECIMAL(18,4),
    ActualValue DECIMAL(18,4),
    Achievement DECIMAL(5,2), -- Percentage
    Comments NVARCHAR(MAX),
    DataQuality NVARCHAR(50), -- Verified, Estimated, Provisional
    CreatedDate DATETIME2 DEFAULT GETUTCDATE(),
    CreatedBy NVARCHAR(256)
);

-- =============================================
-- 6. MILESTONES
-- =============================================

IF OBJECT_ID('dbo.Milestones', 'U') IS NOT NULL DROP TABLE dbo.Milestones;
CREATE TABLE dbo.Milestones (
    Id INT PRIMARY KEY IDENTITY(1,1),
    InitiativeId INT FOREIGN KEY REFERENCES Initiatives(Id),
    Title NVARCHAR(500) NOT NULL,
    TitleAr NVARCHAR(500),
    Description NVARCHAR(MAX),
    DescriptionAr NVARCHAR(MAX),
    PlannedDate DATE,
    ActualDate DATE,
    Status NVARCHAR(50), -- Pending, InProgress, Completed, Delayed, Cancelled
    Progress DECIMAL(5,2) DEFAULT 0,
    IsCritical BIT DEFAULT 0,
    Dependencies NVARCHAR(MAX), -- JSON array of dependent milestone IDs
    Owner NVARCHAR(256),
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME2 DEFAULT GETUTCDATE(),
    CreatedBy NVARCHAR(256),
    ModifiedDate DATETIME2,
    ModifiedBy NVARCHAR(256)
);

-- =============================================
-- 7. RISKS
-- =============================================

IF OBJECT_ID('dbo.Risks', 'U') IS NOT NULL DROP TABLE dbo.Risks;
CREATE TABLE dbo.Risks (
    Id INT PRIMARY KEY IDENTITY(1,1),
    InitiativeId INT FOREIGN KEY REFERENCES Initiatives(Id),
    ProgramId INT FOREIGN KEY REFERENCES Programs(Id),
    Code NVARCHAR(50) UNIQUE,
    Title NVARCHAR(500) NOT NULL,
    TitleAr NVARCHAR(500),
    Description NVARCHAR(MAX),
    DescriptionAr NVARCHAR(MAX),
    Category NVARCHAR(100), -- Technical, Financial, Operational, Strategic, Compliance
    Probability NVARCHAR(20), -- VeryLow, Low, Medium, High, VeryHigh
    Impact NVARCHAR(20), -- VeryLow, Low, Medium, High, VeryHigh
    RiskScore DECIMAL(5,2), -- Calculated from Probability x Impact
    Status NVARCHAR(50), -- Open, Mitigating, Closed, Escalated
    MitigationPlan NVARCHAR(MAX),
    MitigationPlanAr NVARCHAR(MAX),
    ContingencyPlan NVARCHAR(MAX),
    Owner NVARCHAR(256),
    EscalatedTo NVARCHAR(256),
    IdentifiedDate DATE,
    ClosedDate DATE,
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME2 DEFAULT GETUTCDATE(),
    CreatedBy NVARCHAR(256),
    ModifiedDate DATETIME2,
    ModifiedBy NVARCHAR(256)
);

-- =============================================
-- 8. ACHIEVEMENTS
-- =============================================

IF OBJECT_ID('dbo.Achievements', 'U') IS NOT NULL DROP TABLE dbo.Achievements;
CREATE TABLE dbo.Achievements (
    Id INT PRIMARY KEY IDENTITY(1,1),
    InitiativeId INT FOREIGN KEY REFERENCES Initiatives(Id),
    ProgramId INT FOREIGN KEY REFERENCES Programs(Id),
    Title NVARCHAR(500) NOT NULL,
    TitleAr NVARCHAR(500),
    Description NVARCHAR(MAX),
    DescriptionAr NVARCHAR(MAX),
    AchievementDate DATE,
    Category NVARCHAR(100),
    ImpactLevel NVARCHAR(20), -- Low, Medium, High, Critical
    MediaUrl NVARCHAR(500),
    IsHighlighted BIT DEFAULT 0,
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME2 DEFAULT GETUTCDATE(),
    CreatedBy NVARCHAR(256),
    ModifiedDate DATETIME2,
    ModifiedBy NVARCHAR(256)
);

-- =============================================
-- 9. WORKFLOW APPROVALS
-- =============================================

IF OBJECT_ID('dbo.WorkflowApprovals', 'U') IS NOT NULL DROP TABLE dbo.WorkflowApprovals;
CREATE TABLE dbo.WorkflowApprovals (
    Id INT PRIMARY KEY IDENTITY(1,1),
    EntityType NVARCHAR(50), -- Program, Initiative, KPI, Risk, etc.
    EntityId INT,
    RequestType NVARCHAR(50), -- Create, Update, Delete, Approve, Reject
    RequestedBy NVARCHAR(256),
    RequestedDate DATETIME2,
    ApprovedBy NVARCHAR(256),
    ApprovalDate DATETIME2,
    Status NVARCHAR(50), -- Pending, Approved, Rejected, Cancelled
    Comments NVARCHAR(MAX),
    Priority NVARCHAR(20),
    DueDate DATE,
    CreatedDate DATETIME2 DEFAULT GETUTCDATE()
);

-- =============================================
-- 10. AUDIT LOGS
-- =============================================

IF OBJECT_ID('dbo.AuditLogs', 'U') IS NOT NULL DROP TABLE dbo.AuditLogs;
CREATE TABLE dbo.AuditLogs (
    Id BIGINT PRIMARY KEY IDENTITY(1,1),
    EntityType NVARCHAR(50),
    EntityId INT,
    Action NVARCHAR(50), -- Create, Update, Delete, View, Export
    OldValue NVARCHAR(MAX), -- JSON
    NewValue NVARCHAR(MAX), -- JSON
    ChangedFields NVARCHAR(MAX), -- JSON array of field names
    UserId NVARCHAR(256),
    UserName NVARCHAR(256),
    UserRole NVARCHAR(100),
    IpAddress NVARCHAR(50),
    UserAgent NVARCHAR(500),
    Timestamp DATETIME2 DEFAULT GETUTCDATE()
);

-- =============================================
-- 11. ATTACHMENTS
-- =============================================

IF OBJECT_ID('dbo.Attachments', 'U') IS NOT NULL DROP TABLE dbo.Attachments;
CREATE TABLE dbo.Attachments (
    Id INT PRIMARY KEY IDENTITY(1,1),
    EntityType NVARCHAR(50),
    EntityId INT,
    FileName NVARCHAR(256),
    FileSize BIGINT,
    ContentType NVARCHAR(100),
    FilePath NVARCHAR(500),
    BlobName NVARCHAR(256),
    Description NVARCHAR(500),
    Tags NVARCHAR(500),
    IsActive BIT DEFAULT 1,
    UploadedDate DATETIME2 DEFAULT GETUTCDATE(),
    UploadedBy NVARCHAR(256)
);

-- =============================================
-- 12. NOTIFICATIONS
-- =============================================

IF OBJECT_ID('dbo.Notifications', 'U') IS NOT NULL DROP TABLE dbo.Notifications;
CREATE TABLE dbo.Notifications (
    Id INT PRIMARY KEY IDENTITY(1,1),
    UserId NVARCHAR(256),
    Title NVARCHAR(500),
    TitleAr NVARCHAR(500),
    Message NVARCHAR(MAX),
    MessageAr NVARCHAR(MAX),
    Type NVARCHAR(50), -- Info, Warning, Error, Success, Alert
    Category NVARCHAR(50), -- System, Approval, Reminder, Update
    EntityType NVARCHAR(50),
    EntityId INT,
    ActionUrl NVARCHAR(500),
    IsRead BIT DEFAULT 0,
    ReadDate DATETIME2,
    IsSent BIT DEFAULT 0,
    SentDate DATETIME2,
    Priority NVARCHAR(20),
    ExpiryDate DATETIME2,
    CreatedDate DATETIME2 DEFAULT GETUTCDATE()
);

-- =============================================
-- 13. REPORTS
-- =============================================

IF OBJECT_ID('dbo.Reports', 'U') IS NOT NULL DROP TABLE dbo.Reports;
CREATE TABLE dbo.Reports (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Code NVARCHAR(50) UNIQUE,
    Title NVARCHAR(500) NOT NULL,
    TitleAr NVARCHAR(500),
    Type NVARCHAR(50), -- Dashboard, Performance, Progress, Executive, Custom
    Category NVARCHAR(100),
    Parameters NVARCHAR(MAX), -- JSON
    Schedule NVARCHAR(100), -- Once, Daily, Weekly, Monthly, Quarterly, Yearly
    LastRunDate DATETIME2,
    NextRunDate DATETIME2,
    OutputFormat NVARCHAR(20), -- PDF, Excel, Word, PowerPoint
    FilePath NVARCHAR(500),
    Recipients NVARCHAR(MAX), -- JSON array of emails
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME2 DEFAULT GETUTCDATE(),
    CreatedBy NVARCHAR(256),
    ModifiedDate DATETIME2,
    ModifiedBy NVARCHAR(256)
);

-- =============================================
-- 14. SETTINGS
-- =============================================

IF OBJECT_ID('dbo.Settings', 'U') IS NOT NULL DROP TABLE dbo.Settings;
CREATE TABLE dbo.Settings (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Category NVARCHAR(100),
    [Key] NVARCHAR(256) UNIQUE,
    Value NVARCHAR(MAX),
    DataType NVARCHAR(50), -- String, Number, Boolean, JSON
    Description NVARCHAR(500),
    IsSystemSetting BIT DEFAULT 0,
    IsEncrypted BIT DEFAULT 0,
    ModifiedDate DATETIME2,
    ModifiedBy NVARCHAR(256)
);

-- =============================================
-- 15. THRESHOLDS (For KPI monitoring)
-- =============================================

IF OBJECT_ID('dbo.Thresholds', 'U') IS NOT NULL DROP TABLE dbo.Thresholds;
CREATE TABLE dbo.Thresholds (
    Id INT PRIMARY KEY IDENTITY(1,1),
    KPIId INT FOREIGN KEY REFERENCES KPIs(Id),
    ThresholdType NVARCHAR(50), -- Target, Warning, Critical
    Operator NVARCHAR(20), -- GreaterThan, LessThan, Equal, Between
    Value1 DECIMAL(18,4),
    Value2 DECIMAL(18,4), -- For Between operator
    Color NVARCHAR(20), -- Green, Yellow, Red
    NotificationEnabled BIT DEFAULT 0,
    EscalationEnabled BIT DEFAULT 0,
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME2 DEFAULT GETUTCDATE(),
    CreatedBy NVARCHAR(256),
    ModifiedDate DATETIME2,
    ModifiedBy NVARCHAR(256)
);

-- =============================================
-- CREATE INDEXES FOR PERFORMANCE
-- =============================================

-- Vision indexes
CREATE INDEX IX_Visions_Status ON Visions(Status);
CREATE INDEX IX_Visions_IsActive ON Visions(IsActive);

-- Program indexes
CREATE INDEX IX_Programs_VisionId ON Programs(VisionId);
CREATE INDEX IX_Programs_Status ON Programs(Status);
CREATE INDEX IX_Programs_Owner ON Programs(Owner);

-- Initiative indexes
CREATE INDEX IX_Initiatives_ProgramId ON Initiatives(ProgramId);
CREATE INDEX IX_Initiatives_Status ON Initiatives(Status);
CREATE INDEX IX_Initiatives_Owner ON Initiatives(Owner);

-- KPI indexes
CREATE INDEX IX_KPIs_InitiativeId ON KPIs(InitiativeId);
CREATE INDEX IX_KPIs_ProgramId ON KPIs(ProgramId);
CREATE INDEX IX_KPIs_Status ON KPIs(Status);

-- KPI Values indexes
CREATE INDEX IX_KPIValues_KPIId ON KPIValues(KPIId);
CREATE INDEX IX_KPIValues_Period ON KPIValues(Period);

-- Milestone indexes
CREATE INDEX IX_Milestones_InitiativeId ON Milestones(InitiativeId);
CREATE INDEX IX_Milestones_Status ON Milestones(Status);
CREATE INDEX IX_Milestones_PlannedDate ON Milestones(PlannedDate);

-- Risk indexes
CREATE INDEX IX_Risks_InitiativeId ON Risks(InitiativeId);
CREATE INDEX IX_Risks_ProgramId ON Risks(ProgramId);
CREATE INDEX IX_Risks_Status ON Risks(Status);

-- Workflow indexes
CREATE INDEX IX_WorkflowApprovals_EntityType_EntityId ON WorkflowApprovals(EntityType, EntityId);
CREATE INDEX IX_WorkflowApprovals_Status ON WorkflowApprovals(Status);
CREATE INDEX IX_WorkflowApprovals_RequestedBy ON WorkflowApprovals(RequestedBy);

-- Audit log indexes
CREATE INDEX IX_AuditLogs_EntityType_EntityId ON AuditLogs(EntityType, EntityId);
CREATE INDEX IX_AuditLogs_UserId ON AuditLogs(UserId);
CREATE INDEX IX_AuditLogs_Timestamp ON AuditLogs(Timestamp);

-- Notification indexes
CREATE INDEX IX_Notifications_UserId ON Notifications(UserId);
CREATE INDEX IX_Notifications_IsRead ON Notifications(IsRead);
CREATE INDEX IX_Notifications_CreatedDate ON Notifications(CreatedDate);

-- =============================================
-- INSERT INITIAL DATA
-- =============================================

-- Insert default settings
INSERT INTO Settings (Category, [Key], Value, DataType, Description, IsSystemSetting)
VALUES 
    ('System', 'DefaultLanguage', 'ar', 'String', 'Default system language', 1),
    ('System', 'MaxFileUploadSize', '10485760', 'Number', 'Maximum file upload size in bytes (10MB)', 1),
    ('System', 'SessionTimeout', '30', 'Number', 'Session timeout in minutes', 1),
    ('System', 'PasswordExpiryDays', '90', 'Number', 'Password expiry period in days', 1),
    ('Display', 'ItemsPerPage', '25', 'Number', 'Default items per page in lists', 0),
    ('Display', 'DateFormat', 'dd/MM/yyyy', 'String', 'Date display format', 0),
    ('Notification', 'EmailEnabled', 'true', 'Boolean', 'Enable email notifications', 1),
    ('Notification', 'SMSEnabled', 'false', 'Boolean', 'Enable SMS notifications', 1);

-- Insert sample Vision 2030 data
INSERT INTO Visions (Code, Title, TitleAr, Description, DescriptionAr, StartDate, EndDate, Status, Progress)
VALUES 
    ('VISION2030', 'Saudi Vision 2030', 'رؤية السعودية 2030', 
     'A transformative economic and social reform blueprint that is opening Saudi Arabia up to the world',
     'مخطط إصلاح اقتصادي واجتماعي تحويلي يفتح المملكة العربية السعودية على العالم',
     '2016-04-25', '2030-12-31', 'Active', 35.5);

-- Insert sample programs
DECLARE @VisionId INT = SCOPE_IDENTITY();

INSERT INTO Programs (VisionId, Code, Title, TitleAr, Description, DescriptionAr, Budget, StartDate, EndDate, Status, Progress, Priority)
VALUES 
    (@VisionId, 'QOL', 'Quality of Life Program', 'برنامج جودة الحياة',
     'Improving the quality of life for residents and visitors', 
     'تحسين جودة الحياة للمقيمين والزوار',
     1000000000, '2018-01-01', '2030-12-31', 'Active', 42.0, 'High'),
    
    (@VisionId, 'PIF', 'Public Investment Fund Program', 'برنامج صندوق الاستثمارات العامة',
     'Strengthening the Public Investment Fund''s position', 
     'تعزيز موقع صندوق الاستثمارات العامة',
     2000000000, '2017-01-01', '2030-12-31', 'Active', 38.5, 'High'),
     
    (@VisionId, 'NTP', 'National Transformation Program', 'برنامج التحول الوطني',
     'Achieving governmental operational excellence', 
     'تحقيق التميز الحكومي التشغيلي',
     1500000000, '2016-06-01', '2025-12-31', 'Active', 65.0, 'High');

PRINT 'Database SMO_Production created successfully with all tables and initial data!';

-- =============================================
-- VERIFICATION QUERIES
-- =============================================

-- Check all tables
SELECT 
    t.name AS TableName,
    p.rows AS RowCount
FROM sys.tables t
INNER JOIN sys.partitions p ON t.object_id = p.object_id
WHERE p.index_id IN (0, 1)
ORDER BY t.name;

-- Check Vision and Programs
SELECT v.Title as Vision, COUNT(p.Id) as ProgramCount
FROM Visions v
LEFT JOIN Programs p ON v.Id = p.VisionId
GROUP BY v.Title;
