-- =============================================
-- SMO Platform - Add Shared Tables for Interconnected Architecture
-- Version: 1.1
-- Date: December 2024
-- Description: Creates additional tables for shared resources and Performance Management
-- =============================================

USE SMO_Production;
GO

-- =============================================
-- 1. SHARED MILESTONES TABLE (for cross-module tracking)
-- =============================================

IF OBJECT_ID('dbo.SharedMilestones', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.SharedMilestones (
        Id INT PRIMARY KEY IDENTITY(1,1),
        Title NVARCHAR(500) NOT NULL,
        TitleAr NVARCHAR(500),
        Description NVARCHAR(MAX),
        DescriptionAr NVARCHAR(MAX),
        PlannedDate DATE NOT NULL,
        ActualDate DATE,
        Status NVARCHAR(50) DEFAULT 'Pending', -- Pending, InProgress, Completed, Delayed, Cancelled
        Progress DECIMAL(5,2) DEFAULT 0,
        IsCritical BIT DEFAULT 0,
        Dependencies NVARCHAR(MAX), -- JSON array of milestone IDs
        Owner NVARCHAR(256),
        OriginModule NVARCHAR(50), -- Performance, Vision2030, Shared
        IsActive BIT DEFAULT 1,
        CreatedDate DATETIME2 DEFAULT GETUTCDATE(),
        CreatedBy NVARCHAR(256),
        ModifiedDate DATETIME2,
        ModifiedBy NVARCHAR(256)
    );

    CREATE INDEX IX_SharedMilestones_Status ON SharedMilestones(Status);
    CREATE INDEX IX_SharedMilestones_PlannedDate ON SharedMilestones(PlannedDate);
    CREATE INDEX IX_SharedMilestones_OriginModule ON SharedMilestones(OriginModule);
    CREATE INDEX IX_SharedMilestones_Owner ON SharedMilestones(Owner);
END
GO

-- =============================================
-- 2. LINKING TABLES FOR INTERCONNECTED ARCHITECTURE
-- =============================================

-- Performance Goal to Milestone linking
IF OBJECT_ID('dbo.PerformanceGoalMilestones', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.PerformanceGoalMilestones (
        Id INT PRIMARY KEY IDENTITY(1,1),
        MilestoneId INT FOREIGN KEY REFERENCES SharedMilestones(Id),
        PerformanceGoalId INT, -- Will reference PerformanceGoals table when created
        EmployeeId INT,
        ContributionPercentage DECIMAL(5,2),
        Role NVARCHAR(50), -- Primary, Supporting, Observer
        CreatedDate DATETIME2 DEFAULT GETUTCDATE(),
        CreatedBy NVARCHAR(256),
        ModifiedDate DATETIME2,
        ModifiedBy NVARCHAR(256),
        IsActive BIT DEFAULT 1
    );

    CREATE INDEX IX_PerformanceGoalMilestones_MilestoneId ON PerformanceGoalMilestones(MilestoneId);
    CREATE INDEX IX_PerformanceGoalMilestones_PerformanceGoalId ON PerformanceGoalMilestones(PerformanceGoalId);
    CREATE INDEX IX_PerformanceGoalMilestones_EmployeeId ON PerformanceGoalMilestones(EmployeeId);
END
GO

-- Initiative to Shared Milestone linking (different from initiative-specific milestones)
IF OBJECT_ID('dbo.InitiativeSharedMilestones', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.InitiativeSharedMilestones (
        Id INT PRIMARY KEY IDENTITY(1,1),
        InitiativeId INT FOREIGN KEY REFERENCES Initiatives(Id),
        MilestoneId INT FOREIGN KEY REFERENCES SharedMilestones(Id),
        ContributionPercentage DECIMAL(5,2),
        CreatedDate DATETIME2 DEFAULT GETUTCDATE(),
        CreatedBy NVARCHAR(256),
        IsActive BIT DEFAULT 1
    );

    CREATE INDEX IX_InitiativeSharedMilestones_InitiativeId ON InitiativeSharedMilestones(InitiativeId);
    CREATE INDEX IX_InitiativeSharedMilestones_MilestoneId ON InitiativeSharedMilestones(MilestoneId);
END
GO

-- Performance Risk linking
IF OBJECT_ID('dbo.PerformanceRisks', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.PerformanceRisks (
        Id INT PRIMARY KEY IDENTITY(1,1),
        RiskId INT FOREIGN KEY REFERENCES Risks(Id),
        PerformanceEntityId INT,
        PerformanceEntityType NVARCHAR(50), -- Goal, Evaluation, Review, DevelopmentPlan
        RelationshipType NVARCHAR(50), -- Threatens, Mitigates, Monitors
        Notes NVARCHAR(MAX),
        CreatedDate DATETIME2 DEFAULT GETUTCDATE(),
        CreatedBy NVARCHAR(256),
        ModifiedDate DATETIME2,
        ModifiedBy NVARCHAR(256),
        IsActive BIT DEFAULT 1
    );

    CREATE INDEX IX_PerformanceRisks_RiskId ON PerformanceRisks(RiskId);
    CREATE INDEX IX_PerformanceRisks_PerformanceEntityId ON PerformanceRisks(PerformanceEntityId);
    CREATE INDEX IX_PerformanceRisks_EntityType ON PerformanceRisks(PerformanceEntityType);
END
GO

-- =============================================
-- 3. PERFORMANCE MANAGEMENT TABLES
-- =============================================

-- Performance Evaluations
IF OBJECT_ID('dbo.PerformanceEvaluations', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.PerformanceEvaluations (
        Id INT PRIMARY KEY IDENTITY(1,1),
        EmployeeId INT NOT NULL,
        EvaluationPeriod NVARCHAR(50), -- Q1-2024, Annual-2024
        StartDate DATE NOT NULL,
        EndDate DATE NOT NULL,
        Status NVARCHAR(50) DEFAULT 'Draft', -- Draft, InProgress, Completed, Approved
        OverallScore DECIMAL(5,2),
        ManagerComments NVARCHAR(MAX),
        EmployeeComments NVARCHAR(MAX),
        ManagerId INT,
        ReviewDate DATE,
        ApprovalDate DATE,
        ApprovedBy NVARCHAR(256),
        CreatedDate DATETIME2 DEFAULT GETUTCDATE(),
        CreatedBy NVARCHAR(256),
        ModifiedDate DATETIME2,
        ModifiedBy NVARCHAR(256),
        IsActive BIT DEFAULT 1
    );

    CREATE INDEX IX_PerformanceEvaluations_EmployeeId ON PerformanceEvaluations(EmployeeId);
    CREATE INDEX IX_PerformanceEvaluations_Period ON PerformanceEvaluations(EvaluationPeriod);
    CREATE INDEX IX_PerformanceEvaluations_Status ON PerformanceEvaluations(Status);
END
GO

-- Performance Goals
IF OBJECT_ID('dbo.PerformanceGoals', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.PerformanceGoals (
        Id INT PRIMARY KEY IDENTITY(1,1),
        EmployeeId INT NOT NULL,
        EvaluationId INT FOREIGN KEY REFERENCES PerformanceEvaluations(Id),
        Title NVARCHAR(500) NOT NULL,
        TitleAr NVARCHAR(500),
        Description NVARCHAR(MAX),
        DescriptionAr NVARCHAR(MAX),
        Category NVARCHAR(100), -- Strategic, Operational, Development, Innovation
        Weight DECIMAL(5,2), -- Percentage weight in evaluation
        TargetDate DATE,
        ActualCompletionDate DATE,
        Status NVARCHAR(50) DEFAULT 'NotStarted', -- NotStarted, InProgress, Completed, Cancelled
        Progress DECIMAL(5,2) DEFAULT 0,
        SuccessCriteria NVARCHAR(MAX),
        MeasurementMethod NVARCHAR(MAX),
        Score DECIMAL(5,2),
        ManagerFeedback NVARCHAR(MAX),
        CreatedDate DATETIME2 DEFAULT GETUTCDATE(),
        CreatedBy NVARCHAR(256),
        ModifiedDate DATETIME2,
        ModifiedBy NVARCHAR(256),
        IsActive BIT DEFAULT 1
    );

    CREATE INDEX IX_PerformanceGoals_EmployeeId ON PerformanceGoals(EmployeeId);
    CREATE INDEX IX_PerformanceGoals_EvaluationId ON PerformanceGoals(EvaluationId);
    CREATE INDEX IX_PerformanceGoals_Status ON PerformanceGoals(Status);
    CREATE INDEX IX_PerformanceGoals_Category ON PerformanceGoals(Category);
END
GO

-- Performance Reviews
IF OBJECT_ID('dbo.PerformanceReviews', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.PerformanceReviews (
        Id INT PRIMARY KEY IDENTITY(1,1),
        EvaluationId INT FOREIGN KEY REFERENCES PerformanceEvaluations(Id),
        ReviewType NVARCHAR(50), -- Monthly, Quarterly, Annual, Mid-Year
        ReviewDate DATE,
        ReviewerId INT,
        ReviewerName NVARCHAR(256),
        ReviewerRole NVARCHAR(100),
        OverallRating DECIMAL(5,2),
        StrengthsIdentified NVARCHAR(MAX),
        AreasForImprovement NVARCHAR(MAX),
        DevelopmentRecommendations NVARCHAR(MAX),
        NextSteps NVARCHAR(MAX),
        EmployeeAcknowledged BIT DEFAULT 0,
        AcknowledgedDate DATE,
        CreatedDate DATETIME2 DEFAULT GETUTCDATE(),
        CreatedBy NVARCHAR(256),
        ModifiedDate DATETIME2,
        ModifiedBy NVARCHAR(256),
        IsActive BIT DEFAULT 1
    );

    CREATE INDEX IX_PerformanceReviews_EvaluationId ON PerformanceReviews(EvaluationId);
    CREATE INDEX IX_PerformanceReviews_ReviewType ON PerformanceReviews(ReviewType);
    CREATE INDEX IX_PerformanceReviews_ReviewDate ON PerformanceReviews(ReviewDate);
END
GO

-- Performance Ratings
IF OBJECT_ID('dbo.PerformanceRatings', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.PerformanceRatings (
        Id INT PRIMARY KEY IDENTITY(1,1),
        Code NVARCHAR(50) UNIQUE,
        Name NVARCHAR(100) NOT NULL,
        NameAr NVARCHAR(100),
        Description NVARCHAR(500),
        DescriptionAr NVARCHAR(500),
        MinScore DECIMAL(5,2),
        MaxScore DECIMAL(5,2),
        Color NVARCHAR(20), -- For UI display
        SortOrder INT,
        IsActive BIT DEFAULT 1,
        CreatedDate DATETIME2 DEFAULT GETUTCDATE(),
        CreatedBy NVARCHAR(256),
        ModifiedDate DATETIME2,
        ModifiedBy NVARCHAR(256)
    );

    CREATE INDEX IX_PerformanceRatings_Code ON PerformanceRatings(Code);
    
    -- Insert default rating scales
    INSERT INTO PerformanceRatings (Code, Name, NameAr, MinScore, MaxScore, Color, SortOrder)
    VALUES 
        ('OUTSTANDING', 'Outstanding', 'متميز', 90, 100, 'green', 1),
        ('EXCEEDS', 'Exceeds Expectations', 'يفوق التوقعات', 80, 89.99, 'lightgreen', 2),
        ('MEETS', 'Meets Expectations', 'يلبي التوقعات', 70, 79.99, 'yellow', 3),
        ('NEEDS_IMPROVEMENT', 'Needs Improvement', 'يحتاج إلى تحسين', 60, 69.99, 'orange', 4),
        ('UNSATISFACTORY', 'Unsatisfactory', 'غير مرض', 0, 59.99, 'red', 5);
END
GO

-- Development Plans
IF OBJECT_ID('dbo.DevelopmentPlans', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.DevelopmentPlans (
        Id INT PRIMARY KEY IDENTITY(1,1),
        EmployeeId INT NOT NULL,
        EvaluationId INT FOREIGN KEY REFERENCES PerformanceEvaluations(Id),
        Title NVARCHAR(500) NOT NULL,
        TitleAr NVARCHAR(500),
        Objectives NVARCHAR(MAX),
        ObjectivesAr NVARCHAR(MAX),
        SkillsToAddress NVARCHAR(MAX), -- JSON array
        TrainingRequired NVARCHAR(MAX),
        MentorAssigned NVARCHAR(256),
        StartDate DATE,
        EndDate DATE,
        Status NVARCHAR(50) DEFAULT 'Draft', -- Draft, Active, Completed, OnHold
        Progress DECIMAL(5,2) DEFAULT 0,
        Budget DECIMAL(18,2),
        ActualCost DECIMAL(18,2),
        OutcomeAssessment NVARCHAR(MAX),
        CreatedDate DATETIME2 DEFAULT GETUTCDATE(),
        CreatedBy NVARCHAR(256),
        ModifiedDate DATETIME2,
        ModifiedBy NVARCHAR(256),
        IsActive BIT DEFAULT 1
    );

    CREATE INDEX IX_DevelopmentPlans_EmployeeId ON DevelopmentPlans(EmployeeId);
    CREATE INDEX IX_DevelopmentPlans_EvaluationId ON DevelopmentPlans(EvaluationId);
    CREATE INDEX IX_DevelopmentPlans_Status ON DevelopmentPlans(Status);
END
GO

-- =============================================
-- 4. ADDITIONAL SHARED RESOURCE TABLES
-- =============================================

-- Change Requests (shared across modules)
IF OBJECT_ID('dbo.ChangeRequests', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.ChangeRequests (
        Id INT PRIMARY KEY IDENTITY(1,1),
        RequestNumber NVARCHAR(50) UNIQUE,
        Title NVARCHAR(500) NOT NULL,
        TitleAr NVARCHAR(500),
        Description NVARCHAR(MAX),
        DescriptionAr NVARCHAR(MAX),
        RequestType NVARCHAR(50), -- Scope, Schedule, Budget, Resource, Technical
        Priority NVARCHAR(20), -- Critical, High, Medium, Low
        Status NVARCHAR(50) DEFAULT 'Pending', -- Pending, UnderReview, Approved, Rejected, Implemented
        ImpactAssessment NVARCHAR(MAX),
        AffectedModules NVARCHAR(500), -- JSON array: ["Performance", "Vision2030"]
        RequestedBy NVARCHAR(256),
        RequestedDate DATE,
        ReviewedBy NVARCHAR(256),
        ReviewDate DATE,
        ApprovedBy NVARCHAR(256),
        ApprovalDate DATE,
        ImplementedDate DATE,
        RejectionReason NVARCHAR(MAX),
        CreatedDate DATETIME2 DEFAULT GETUTCDATE(),
        CreatedBy NVARCHAR(256),
        ModifiedDate DATETIME2,
        ModifiedBy NVARCHAR(256),
        IsActive BIT DEFAULT 1
    );

    CREATE INDEX IX_ChangeRequests_RequestNumber ON ChangeRequests(RequestNumber);
    CREATE INDEX IX_ChangeRequests_Status ON ChangeRequests(Status);
    CREATE INDEX IX_ChangeRequests_Priority ON ChangeRequests(Priority);
END
GO

-- Support Requests (shared across modules)
IF OBJECT_ID('dbo.SupportRequests', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.SupportRequests (
        Id INT PRIMARY KEY IDENTITY(1,1),
        TicketNumber NVARCHAR(50) UNIQUE,
        Title NVARCHAR(500) NOT NULL,
        TitleAr NVARCHAR(500),
        Description NVARCHAR(MAX),
        DescriptionAr NVARCHAR(MAX),
        Category NVARCHAR(100), -- Technical, Training, Process, Data, Access
        Module NVARCHAR(50), -- Performance, Vision2030, Shared
        Priority NVARCHAR(20), -- Critical, High, Medium, Low
        Status NVARCHAR(50) DEFAULT 'Open', -- Open, InProgress, Resolved, Closed, Escalated
        RequestedBy NVARCHAR(256),
        RequestedDate DATETIME2,
        AssignedTo NVARCHAR(256),
        AssignedDate DATETIME2,
        ResolvedBy NVARCHAR(256),
        ResolutionDate DATETIME2,
        ResolutionDetails NVARCHAR(MAX),
        CustomerSatisfaction INT, -- 1-5 rating
        CreatedDate DATETIME2 DEFAULT GETUTCDATE(),
        CreatedBy NVARCHAR(256),
        ModifiedDate DATETIME2,
        ModifiedBy NVARCHAR(256),
        IsActive BIT DEFAULT 1
    );

    CREATE INDEX IX_SupportRequests_TicketNumber ON SupportRequests(TicketNumber);
    CREATE INDEX IX_SupportRequests_Status ON SupportRequests(Status);
    CREATE INDEX IX_SupportRequests_Module ON SupportRequests(Module);
    CREATE INDEX IX_SupportRequests_Priority ON SupportRequests(Priority);
END
GO

-- =============================================
-- 5. KPI TARGETS TABLE (Missing from original schema)
-- =============================================

IF OBJECT_ID('dbo.KPITargets', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.KPITargets (
        Id INT PRIMARY KEY IDENTITY(1,1),
        KPIId INT FOREIGN KEY REFERENCES KPIs(Id),
        Year INT NOT NULL,
        Quarter INT,
        Month INT,
        TargetValue DECIMAL(18,4) NOT NULL,
        StretchTarget DECIMAL(18,4),
        MinimumAcceptable DECIMAL(18,4),
        JustificationAr NVARCHAR(MAX),
        JustificationEn NVARCHAR(MAX),
        IsApproved BIT DEFAULT 0,
        ApprovedBy NVARCHAR(256),
        ApprovalDate DATETIME2,
        CreatedDate DATETIME2 DEFAULT GETUTCDATE(),
        CreatedBy NVARCHAR(256),
        ModifiedDate DATETIME2,
        ModifiedBy NVARCHAR(256),
        IsDeleted BIT DEFAULT 0
    );

    CREATE INDEX IX_KPITargets_KPIId ON KPITargets(KPIId);
    CREATE INDEX IX_KPITargets_Period ON KPITargets(Year, Quarter, Month);
    CREATE INDEX IX_KPITargets_IsApproved ON KPITargets(IsApproved);
    CREATE UNIQUE INDEX UX_KPITargets_KPI_Period ON KPITargets(KPIId, Year, Quarter, Month);
END
GO

-- =============================================
-- 6. OPTIMIZED INDEXES FOR INTERCONNECTED QUERIES
-- =============================================

-- Composite indexes for cross-module queries
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Risks_Module_Status')
    CREATE INDEX IX_Risks_Module_Status ON Risks(Status, Category) INCLUDE (Title, Owner);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_SharedMilestones_Cross_Module')
    CREATE INDEX IX_SharedMilestones_Cross_Module ON SharedMilestones(OriginModule, Status, PlannedDate);

-- Indexes for performance queries
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Performance_Employee_Period')
    CREATE INDEX IX_Performance_Employee_Period ON PerformanceEvaluations(EmployeeId, EvaluationPeriod, Status);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Goals_Employee_Status')
    CREATE INDEX IX_Goals_Employee_Status ON PerformanceGoals(EmployeeId, Status) INCLUDE (Progress, TargetDate);

PRINT 'Shared tables and Performance Management tables created successfully!';
PRINT 'Optimized indexes added for interconnected architecture queries.';

-- =============================================
-- VERIFICATION QUERIES
-- =============================================

-- Check all new tables
SELECT 
    t.name AS TableName,
    p.rows AS RowCount
FROM sys.tables t
INNER JOIN sys.partitions p ON t.object_id = p.object_id
WHERE p.index_id IN (0, 1)
AND t.name IN (
    'SharedMilestones', 'PerformanceGoalMilestones', 'InitiativeSharedMilestones',
    'PerformanceRisks', 'PerformanceEvaluations', 'PerformanceGoals',
    'PerformanceReviews', 'PerformanceRatings', 'DevelopmentPlans',
    'ChangeRequests', 'SupportRequests', 'KPITargets'
)
ORDER BY t.name;
