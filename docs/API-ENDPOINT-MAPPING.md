# API Endpoint Mapping - Using Existing Assets

## Overview
This document maps the frontend API calls to the actual backend controllers, using EXISTING domain entities and infrastructure.

## Key Principle: USE WHAT EXISTS
- We are NOT creating new entities
- We are USING existing domain models from `SMO.Domain.Entities.Core`
- We are USING existing repository pattern from `SMO.Infrastructure`
- We are CONNECTING existing UI to existing backend

---

## Entity Mapping

### Frontend → Backend Entity Mapping

| Frontend Concept | Actual Domain Entity | Location |
|-----------------|---------------------|----------|
| Vision | VisionProgram | `SMO.Domain.Entities.Core.VisionProgram` |
| Program | VisionProgram | Same entity as Vision |
| Initiative | Initiative | `SMO.Domain.Entities.Core.Initiative` |
| KPI | KPI | `SMO.Domain.Entities.Core.KPI` |
| KPI Value | KPIValue | `SMO.Domain.Entities.Core.KPIValue` |
| KPI Target | KPITarget | `SMO.Domain.Entities.Core.KPITarget` |
| Milestone | InitiativeMilestone | `SMO.Domain.Entities.Core.InitiativeMilestone` |
| Program Risk | ProgramRisk | `SMO.Domain.Entities.Risk.ProgramRisk` |
| Initiative Risk | InitiativeRisk | `SMO.Domain.Entities.Risk.InitiativeRisk` |

---

## API Endpoint Mapping

### Core Entities

| Frontend Call | Backend Endpoint | Controller | Entity Used |
|--------------|-----------------|------------|-------------|
| getPrograms() | GET /api/program | ProgramController | VisionProgram |
| getProgramDetails(id) | GET /api/program/{id} | ProgramController | VisionProgram |
| createProgram(data) | POST /api/program | ProgramController | VisionProgram |
| updateProgram(id, data) | PUT /api/program/{id} | ProgramController | VisionProgram |
| deleteProgram(id) | DELETE /api/program/{id} | ProgramController | VisionProgram |
| getProgramInitiatives(id) | GET /api/program/{id}/initiatives | ProgramController | Initiative |
| getProgramKPIs(id) | GET /api/program/{id}/kpis | ProgramController | KPI |

### Initiatives

| Frontend Call | Backend Endpoint | Controller | Entity Used |
|--------------|-----------------|------------|-------------|
| getInitiatives() | GET /api/initiative | InitiativeController | Initiative |
| getInitiativeDetails(id) | GET /api/initiative/{id} | InitiativeController | Initiative |
| createInitiative(data) | POST /api/initiative | InitiativeController | Initiative |
| updateInitiative(id, data) | PUT /api/initiative/{id} | InitiativeController | Initiative |
| deleteInitiative(id) | DELETE /api/initiative/{id} | InitiativeController | Initiative |
| getInitiativeMilestones(id) | GET /api/initiative/{id}/milestones | InitiativeController | InitiativeMilestone |
| getInitiativeKPIs(id) | GET /api/initiative/{id}/kpis | InitiativeController | KPI |

### KPIs

| Frontend Call | Backend Endpoint | Controller | Entity Used |
|--------------|-----------------|------------|-------------|
| getKPIs() | GET /api/kpi | KPIController | KPI |
| getKPIDetails(id) | GET /api/kpi/{id} | KPIController | KPI |
| createKPI(data) | POST /api/kpi | KPIController | KPI |
| updateKPI(id, data) | PUT /api/kpi/{id} | KPIController | KPI |
| deleteKPI(id) | DELETE /api/kpi/{id} | KPIController | KPI |
| getKPIValues(id) | GET /api/kpi/{id}/values | KPIController | KPIValue |
| addKPIValue(id, value) | POST /api/kpi/{id}/values | KPIController | KPIValue |
| getKPITargets(id) | GET /api/kpi/{id}/targets | KPIController | KPITarget |
| setKPITarget(id, target) | POST /api/kpi/{id}/targets | KPIController | KPITarget |

### Risks

| Frontend Call | Backend Endpoint | Controller | Entity Used |
|--------------|-----------------|------------|-------------|
| getProgramRisks(programId) | GET /api/risk/program/{id} | RiskController | ProgramRisk |
| getInitiativeRisks(initiativeId) | GET /api/risk/initiative/{id} | RiskController | InitiativeRisk |
| createRisk(data) | POST /api/risk | RiskController | ProgramRisk/InitiativeRisk |
| updateRisk(id, data) | PUT /api/risk/{id} | RiskController | ProgramRisk/InitiativeRisk |
| deleteRisk(id) | DELETE /api/risk/{id} | RiskController | ProgramRisk/InitiativeRisk |

---

## Database Table Mapping

The existing entities map to database tables as follows:

| Entity | Database Table | Notes |
|--------|---------------|--------|
| VisionProgram | Programs | Main program table |
| Initiative | Initiatives | Initiative tracking |
| KPI | KPIs | Key Performance Indicators |
| KPIValue | KPIValues | Historical KPI values |
| KPITarget | KPITargets | KPI target values |
| InitiativeMilestone | Milestones | Initiative milestones |
| ProgramRisk | Risks | Program-level risks |
| InitiativeRisk | Risks | Initiative-level risks |

---

## Frontend Files to Update

### HTML Pages Using These Endpoints

1. **performance-vision.html** → Uses `/api/program` (VisionProgram entity)
2. **performance-programs.html** → Uses `/api/program` endpoints
3. **performance-initiatives.html** → Uses `/api/initiative` endpoints
4. **indicators.html** → Uses `/api/kpi` endpoints
5. **milestone-tracking.html** → Uses `/api/initiative/{id}/milestones`
6. **risk-escalation.html** → Uses `/api/risk` endpoints

### JavaScript Files

1. **api-service.js** - Main API service (UPDATED)
2. **crud-operations.js** - CRUD utilities (already compatible)
3. **loading-states.js** - Loading UI (already compatible)

---

## Implementation Status

### ✅ Completed
- ProgramController (using VisionProgram entity)
- API endpoint mapping in api-service.js

### 🔄 In Progress
- Testing connection between frontend and backend
- Updating database schema to match existing entities

### ⏳ Pending
- InitiativeController (using existing Initiative entity)
- KPIController (using existing KPI entity)
- RiskController (using existing ProgramRisk/InitiativeRisk)
- MilestoneController (using existing InitiativeMilestone)

---

## Important Notes

1. **NO NEW ENTITIES**: We are using ALL existing entities from `SMO.Domain.Entities.Core`
2. **REPOSITORY PATTERN**: Using existing generic repository from `SMO.Infrastructure.Data.Repository<T>`
3. **UNIT OF WORK**: Using existing IUnitOfWork for transaction management
4. **AUDIT TRACKING**: BaseDbContext already handles CreatedBy, UpdatedBy, etc.
5. **SOFT DELETE**: Using IsDeleted flag from existing entities

---

## Next Steps

1. Create remaining controllers using existing entities
2. Update database schema to match entity properties
3. Test API endpoints with existing frontend
4. Connect HTML pages to real endpoints
5. Remove mock data from frontend

---

*This document ensures we USE existing assets rather than creating duplicates.*
