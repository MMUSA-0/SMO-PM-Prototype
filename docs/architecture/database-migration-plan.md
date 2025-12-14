# Database Migration Plan - SMO Platform

## Executive Summary
**Date:** December 2024  
**Architect:** Winston  
**Status:** Entity Framework alignment in progress  
**Critical Issue:** Performance Management entities missing  

## Current State Analysis

### ✅ What Exists
1. **Database Schema** (Create-SMO-Database.sql)
   - Complete table structure for Vision 2030
   - Tables for shared resources (Risk, Milestone, etc.)
   - No Performance Management tables

2. **Domain Entities**
   - Vision 2030 entities (partial)
   - Some risk entities
   - Missing shared resource entities
   - Missing Performance Management entities

3. **EF Configurations**
   - VisionProgramConfiguration ✅
   - InitiativeConfiguration ✅ (created)
   - KPIConfiguration ✅ (created)
   - Others missing

### 🔴 Critical Gaps

#### 1. Missing Performance Management Entities
Need to create in `src/SMO.Domain/Entities/Performance/`:
- PerformanceEvaluation.cs
- PerformanceGoal.cs
- PerformanceReview.cs
- PerformanceRating.cs
- DevelopmentPlan.cs

#### 2. Missing Shared Resource Entities
Created in `src/SMO.Domain/Entities/Shared/`:
- Risk.cs ✅ (created)
- Milestone.cs ✅ (created)
- PerformanceRisk.cs ✅ (created)
- PerformanceGoalMilestone.cs ✅ (created)
Still needed:
- ChangeRequest.cs
- SupportRequest.cs
- Document.cs
- Notification.cs
- AuditLog.cs

## Migration Strategy

### Phase 1: Entity Creation (URGENT)
**Responsible:** Dev Agent
```csharp
// Create Performance Management entities
1. PerformanceEvaluation entity
2. PerformanceGoal entity  
3. PerformanceReview entity
4. PerformanceRating entity
5. DevelopmentPlan entity
```

### Phase 2: EF Configurations
**Responsible:** Architect (in progress)
```csharp
// Already Created:
✅ VisionProgramConfiguration
✅ InitiativeConfiguration
✅ KPIConfiguration

// Still Needed:
- RiskConfiguration (for shared Risk entity)
- MilestoneConfiguration (for shared Milestone entity)
- PerformanceEvaluationConfiguration
- PerformanceGoalConfiguration
- Others...
```

### Phase 3: Database Migration
**Commands to Execute:**
```bash
# In Package Manager Console or CLI
cd src/SMO.Infrastructure

# Add initial migration
dotnet ef migrations add InitialCreate --context AppDbContext --startup-project ../SMO.Api

# Review the migration file
# Check src/SMO.Infrastructure/Migrations/

# Apply migration to database
dotnet ef database update --context AppDbContext --startup-project ../SMO.Api
```

## Database Schema Alignment

### Table Mapping Strategy

| Database Table | Domain Entity | Status | Notes |
|---------------|--------------|---------|--------|
| Programs | VisionProgram | ✅ Configured | Name mismatch handled |
| Initiatives | Initiative | ✅ Configured | Column mappings done |
| KPIs | KPI | ✅ Configured | Foreign keys mapped |
| Risks | Risk (shared) | ⏳ Entity created | Config needed |
| Milestones | Milestone (shared) | ⏳ Entity created | Config needed |
| **Missing Tables for Performance Management** |
| PerformanceEvaluations | PerformanceEvaluation | ❌ Missing | Need entity & table |
| PerformanceGoals | PerformanceGoal | ❌ Missing | Need entity & table |
| PerformanceReviews | PerformanceReview | ❌ Missing | Need entity & table |

### Column Mapping Issues Resolved

1. **Name vs NameAr/NameEn**
   - Database has: Title, TitleAr
   - Entity has: NameEn, NameAr
   - Solution: Column mapping in configuration

2. **IsActive vs IsDeleted**
   - Database has: IsActive
   - Entity has: IsDeleted (from AuditableEntity)
   - Solution: Value conversion in configuration

3. **Date Column Names**
   - Database has: CreatedDate, ModifiedDate
   - Entity has: CreatedOn, UpdatedOn
   - Solution: Column name mapping

## Interconnected Architecture Implementation

### Shared Resources Strategy
```csharp
// Shared entities can be linked to both modules
public class Risk 
{
    // Can be linked to:
    - Vision 2030 Initiatives (via InitiativeRisk)
    - Vision 2030 Programs (via ProgramRisk)
    - Performance Goals (via PerformanceRisk)
    - Development Plans (via PerformanceRisk)
}

public class Milestone
{
    // Can track:
    - Strategic Initiatives (via InitiativeMilestone)
    - Employee Goals (via PerformanceGoalMilestone)
}
```

### Cascade Update Rules
```csharp
// When shared resource updates:
1. Update triggers notification to both modules
2. Audit log records change for both contexts
3. Related entities reflect new status
4. Dashboards update automatically
```

## Index Optimization

### Current Indexes (from SQL)
✅ Already defined in database schema
✅ Mapped in EF configurations

### Recommended Additional Indexes
```sql
-- For shared resource queries
CREATE INDEX IX_Risks_OriginModule ON Risks(OriginModule);
CREATE INDEX IX_Milestones_OriginModule ON Milestones(OriginModule);

-- For performance queries
CREATE INDEX IX_PerformanceGoals_EmployeeId ON PerformanceGoals(EmployeeId);
CREATE INDEX IX_PerformanceEvaluations_Period ON PerformanceEvaluations(Period);
```

## Migration Execution Plan

### Week 1 (Current)
- [x] Create EF configurations for Vision entities
- [x] Create shared resource entities
- [ ] **DEV AGENT:** Create Performance Management entities
- [ ] Create remaining EF configurations
- [ ] Generate and review migration

### Week 2
- [ ] Apply migration to development database
- [ ] Test CRUD operations
- [ ] Verify cascade operations
- [ ] Performance testing

## Risk Mitigation

### Data Migration Risks
1. **Risk:** Existing data incompatibility
   - **Mitigation:** Create data migration scripts
   
2. **Risk:** Performance degradation
   - **Mitigation:** Index optimization before go-live

3. **Risk:** Missing cascade rules
   - **Mitigation:** Comprehensive testing of shared resource updates

## Commands for Dev Agent

```bash
# After entities are created, run:
dotnet build

# Generate migration
dotnet ef migrations add InitialSMOSchema --context AppDbContext --startup-project ../SMO.Api --project ../SMO.Infrastructure

# Review generated migration
# Check: src/SMO.Infrastructure/Migrations/

# Apply to database
dotnet ef database update --context AppDbContext --startup-project ../SMO.Api
```

## Validation Checklist

- [ ] All domain entities created
- [ ] All EF configurations complete
- [ ] AppDbContext has all DbSets
- [ ] Migration generated successfully
- [ ] Migration applied to development
- [ ] CRUD operations tested
- [ ] Shared resource updates work
- [ ] Performance acceptable

## Next Steps for Dev Agent

**CRITICAL - Create these Performance Management entities immediately:**

1. `src/SMO.Domain/Entities/Performance/PerformanceEvaluation.cs`
2. `src/SMO.Domain/Entities/Performance/PerformanceGoal.cs`
3. `src/SMO.Domain/Entities/Performance/PerformanceReview.cs`
4. `src/SMO.Domain/Entities/Performance/PerformanceRating.cs`
5. `src/SMO.Domain/Entities/Performance/DevelopmentPlan.cs`

These should:
- Inherit from `AuditableEntity`
- Include navigation properties to shared resources
- Support the interconnected architecture

## Notes

- Database schema exists but doesn't match domain model
- Using column mapping to bridge differences
- Shared resources enable cross-module data flow
- Performance Management is highest priority

---

**Status:** Awaiting Dev Agent to create Performance Management entities  
**Handoff:** Dev Agent should create entities, then I'll complete EF configurations
