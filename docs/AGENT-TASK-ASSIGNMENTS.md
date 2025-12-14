# Agent Task Assignments - SMO Platform

## Quick Reference for Multi-Agent Collaboration

### 📦 Module Structure - INTERCONNECTED ARCHITECTURE
**IMPORTANT:** The platform has SEPARATE modules that SHARE common resources:

**Core Business Modules (Separate):**
- **Performance Management Module** - Employee performance, evaluations, goals
- **Vision 2030 Module** - Strategic programs, initiatives, KPIs

**Shared Resource Modules (Used by BOTH):**
- **Risk Management** - Risks can be linked to BOTH Performance Goals AND Vision Initiatives
- **Milestone Tracking** - Milestones track BOTH Employee Goals AND Strategic Initiatives
- **Change Requests** - Changes can affect BOTH modules
- **Support Requests** - Support tickets for all modules
- **Documents** - Shared file attachments
- **Notifications** - Unified notification system

**KEY:** When updating shared resources (Risk, Milestone, etc.), changes reflect in BOTH modules!

### 🎯 Current Sprint Goals
**Sprint 1 (Current):** Complete backend APIs and connect first 5 pages  
**Overall Progress:** 35% Complete (21/60 tasks)

---

## 👨‍💻 Dev Agent (James)
**Status:** ACTIVE  
**Current Focus:** Controllers & Entity Framework

### Immediate Tasks - BY MODULE

#### Performance Management Module (SEPARATE)
```markdown
Task X: Create PerformanceController for employee performance
Task X: Create EvaluationController for performance reviews
Task X: Create GoalsController for employee goals/objectives
Task X: Complete EF mappings for Performance entities
```

#### Vision 2030 Module
```markdown
Task 14: Create MilestoneController using InitiativeMilestone entity
Task 16: Create RiskController using ProgramRisk/InitiativeRisk
Task 57: Complete Entity Framework mappings for Vision 2030 entities
Fix: Update VisionController to use VisionProgram entity
```

### Commands to Execute
```bash
# In Cursor, activate Dev agent:
"As dev, create MilestoneController using existing InitiativeMilestone entity"

# After controllers done:
"As dev, create Entity Framework configurations for Initiative, KPI, and Risk entities"
```

---

## 🏗️ Architect Agent (Winston)
**Status:** READY  
**Current Focus:** Database schema & EF migrations

### Immediate Tasks
```markdown
Task 56: Align database schema with domain entities
- Review existing entities vs database tables
- Create migration strategy
- Optimize indexes
```

### Commands to Execute
```bash
# In Cursor, activate Architect:
"As architect, review the entity to database mappings and create EF migration plan"

# Then:
"As architect, update AppDbContext with all required DbSets"
```

---

## 🧪 QA Agent (Quinn)
**Status:** WAITING  
**Current Focus:** Preparing test scenarios

### Immediate Tasks
```markdown
Task 58: Test API endpoints with Postman/Swagger
Task 59: Verify loading-states.js integration
Task 60: Test CRUD operations end-to-end
```

### Commands to Execute
```bash
# After Dev completes controllers:
"As qa, create test scenarios for Program, Initiative, and KPI controllers"

# Then:
"As qa, test the API endpoints and document any issues"
```

---

## 📋 Product Owner (Sarah)
**Status:** READY  
**Current Focus:** Prioritization & validation

### Immediate Tasks
```markdown
- Review completed controllers against requirements
- Prioritize remaining 40+ tasks
- Define MVP scope
- Validate acceptance criteria
```

### Commands to Execute
```bash
# In Cursor:
"As po, review the completed work and prioritize remaining tasks for MVP"
```

---

## 🎭 Orchestrator Agent
**Status:** AVAILABLE  
**Use When:** Coordination needed between agents

### Commands to Execute
```bash
# For coordination:
"As bmad-orchestrator, coordinate the database migration with dev and architect agents"
```

---

## 📊 Task Distribution Matrix

| Priority | Tasks | Primary Agent | Support Agent | Status |
|----------|-------|--------------|---------------|--------|
| 🔴 Critical | 14, 16, 56, 57 | Dev | Architect | 🔄 Active |
| 🟡 High | 4-8, 19-24 | Dev | QA | ⏳ Next |
| 🟢 Medium | 31-36, 41-44 | Dev | PO | ⏳ Week 2 |
| 🔵 Low | 45-55 | QA | Architect | ⏳ Week 3 |

---

## 🔄 Workflow Sequence

### This Week's Flow
```mermaid
Dev Creates Controllers → Architect Reviews → Dev Does EF Mappings → 
Architect Creates Migration → Dev Runs Migration → QA Tests APIs →
Dev Connects Frontend → QA Validates → PO Approves
```

### Daily Standup Topics
1. **Dev:** Controllers status, blockers
2. **Architect:** Schema alignment, migration readiness
3. **QA:** Test preparation, found issues
4. **PO:** Requirement clarifications, priority changes

---

## 🚀 Quick Start Commands

### For Dev Agent
```bash
cd src/SMO.Api
dotnet build
dotnet run
# Navigate to https://localhost:7001/swagger
```

### For QA Agent
```bash
# Test API
curl https://localhost:7001/api/program
curl https://localhost:7001/api/initiative
curl https://localhost:7001/api/kpi
```

### For Frontend Testing
```bash
cd SMO-Platform-UI
python -m http.server 8080

# Performance Management Module Pages (SEPARATE):
# http://localhost:8080/performance-evaluations.html
# http://localhost:8080/performance-goals.html
# http://localhost:8080/performance-reviews.html

# Vision 2030 Module Pages:
# http://localhost:8080/vision-programs.html
# http://localhost:8080/vision-initiatives.html
# http://localhost:8080/vision-kpis.html
```

---

## 📅 This Week's Schedule

### Monday-Tuesday
- **Dev:** Complete remaining controllers
- **Architect:** Review and create EF configs

### Wednesday
- **Dev & Architect:** Run database migrations
- **QA:** Begin API testing

### Thursday-Friday
- **Dev:** Connect frontend pages
- **QA:** End-to-end testing
- **PO:** Review and feedback

---

## 🎯 Success Criteria for Week 1

### Must Complete
- [ ] All 6 main controllers created
- [ ] EF mappings complete
- [ ] Database migration successful
- [ ] 3+ pages showing real data

### Nice to Have
- [ ] Authentication working
- [ ] 5+ pages connected
- [ ] Basic error handling

---

## 💬 Agent Communication

### When to Tag Other Agents
- **Dev → Architect:** "Need review of entity mappings"
- **Dev → QA:** "Controller ready for testing"
- **QA → Dev:** "Found issue in [controller]"
- **All → PO:** "Need clarification on requirement"

### Handoff Template
```markdown
HANDOFF: [From Agent] → [To Agent]
Task: [Task Number & Description]
Status: [Complete/Blocked/In Progress]
Notes: [Any important details]
Next: [What the receiving agent should do]
```

---

## 🔴 Current Blockers

1. **Database migrations not run** → Architect + Dev
2. **Authentication not configured** → Dev
3. **Frontend disconnected** → Waiting for backend

---

## 📝 Notes

- Each agent should update their task status in this document
- Use TODO list for detailed tracking
- Commit code frequently with clear messages
- Test locally before marking complete

---

## 🔗 Architecture Summary

### The SMO Platform uses INTERCONNECTED MODULES:

1. **Core Business Modules (Separate Logic):**
   - Performance Management (Employee-focused)
   - Vision 2030 (Strategy-focused)

2. **Shared Resources (Common Data):**
   - When you create/update a Risk, Milestone, Change Request, etc.
   - It's available to BOTH modules immediately
   - Updates cascade to all linked records

3. **Example Flow:**
   ```
   User in Vision 2030 → Creates Milestone → 
   Links to Initiative → Same Milestone appears in Performance → 
   Manager links it to Employee Goals → 
   When Milestone completes → BOTH modules updated!
   ```

This architecture enables:
- Single source of truth for shared data
- Automatic synchronization between modules
- Reduced duplication and data entry
- Unified reporting across the platform

*Last Updated: December 2024*  
*Architecture Type: Interconnected Modules with Shared Resources*  
*Next Sync: After shared controller implementation*
