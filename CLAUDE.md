# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

SMO Vision Center - Strategic Management Office platform for managing Saudi Vision 2030 strategic objectives, initiatives, KPIs, and performance monitoring.

**Architecture:** Clean Architecture (Onion Architecture) with NO CQRS pattern
**Tech Stack:** .NET 8 + Angular 18

## Commands

### Backend (.NET 8)

```bash
# Build solution
dotnet build

# Run API (from solution root)
cd src/SMO.Api
dotnet run

# Run with hot reload
dotnet watch run

# Clean solution
dotnet clean

# Restore packages
dotnet restore

# Database migrations
dotnet ef migrations add MigrationName --context AppDbContext
dotnet ef database update --context AppDbContext

# Run tests
dotnet test
```

### Frontend (Angular 18)

```bash
# Navigate to frontend
cd src/SMO.Frontend/SMO-Portal

# Install dependencies
npm install

# Development server (port 4200)
ng serve

# Build for specific environment
ng build --configuration development
ng build --configuration testing
ng build --configuration staging
ng build --configuration production

# Run tests
ng test

# E2E tests
ng e2e
```

### Environments

| Environment | Backend API | Frontend Build | Use Case |
|------------|-------------|----------------|----------|
| Development | `dotnet run` | `ng serve` | Local development |
| Testing | - | `ng build --configuration testing` | QA testing |
| Staging | - | `ng build --configuration staging` | Pre-production |
| Production | - | `ng build --configuration production` | Live deployment |

## Architecture & Design Principles

### Clean Architecture Layers

```
┌─────────────────────────────────────────┐
│         SMO.Api (Presentation)          │ ← Controllers, Middleware, Program.cs
├─────────────────────────────────────────┤
│      SMO.Application (Use Cases)        │ ← Application Services, DTOs, Validators
├─────────────────────────────────────────┤
│        SMO.Domain (Core Logic)          │ ← Entities, Interfaces, Business Rules
├─────────────────────────────────────────┤
│    SMO.Infrastructure (Data Access)     │ ← DbContext, Repositories, External APIs
└─────────────────────────────────────────┘

┌─────────────────────────────────────────┐
│     Framework.* (Shared Framework)      │ ← Cross-cutting concerns, reusable logic
└─────────────────────────────────────────┘
```

### Dependency Rules
- **Direction:** Dependencies point INWARD (toward Domain)
- **Domain Layer:** No dependencies on other layers (pure business logic)
- **Application Layer:** Depends only on Domain
- **Infrastructure Layer:** Implements Domain interfaces
- **API Layer:** Depends on Application and Infrastructure

### Key Architectural Decisions

#### ADR-001: No CQRS Pattern
**CRITICAL:** Do NOT implement CQRS (Command Query Responsibility Segregation)
- Use single repository for both reads and writes
- Application services handle both commands and queries
- Simpler architecture per project requirements

#### ADR-002: Controllers Inject Application Services ONLY
**CRITICAL:** Controllers should NEVER inject repositories directly
```csharp
// ✅ CORRECT
public class ObjectivesController : ControllerBase
{
    private readonly StrategicObjectiveAppService _objectiveService;

    public ObjectivesController(StrategicObjectiveAppService objectiveService)
    {
        _objectiveService = objectiveService;
    }
}

// ❌ WRONG - Never inject IRepository or IUnitOfWork in controllers
public class ObjectivesController : ControllerBase
{
    private readonly IRepository<StrategicObjective> _repo; // WRONG!
}
```

#### ADR-003: Generic Repository Pattern
- Use `IRepository<TEntity>` for all data access
- No entity-specific repositories unless absolutely necessary
- Repository provides: Table, TableNoTracking, CRUD, querying, pagination

## Framework Layer Architecture

### Framework.Core - The Foundation

**Core Responsibilities:**
- Base entities and DbContext
- Generic repository pattern
- Unit of Work pattern
- Automatic auditing
- Validation, caching, notifications
- AutoMapper helpers

**Key Classes:**

#### Entity Base Classes
```csharp
// All entities should inherit from one of these:
EntityBase<TKey>              // Simple entity with Id
FullAuditedEntityBase<TKey>   // Auto auditing (CreatedBy, UpdatedBy, etc.)
LookupEntityBase<TKey>        // Lookup tables (NameAr, NameEn, IsActive)
```

#### BaseDbContext - Automatic Auditing
- Automatically sets `CreatedBy`, `CreatedOn`, `UpdatedBy`, `UpdatedOn`
- Uses `CurrentUserName` property (set via middleware)
- Validates entities on save
- All domain DbContexts should inherit from `BaseDbContext<TContext>`

```csharp
// Example usage in AppDbContext
public class AppDbContext : BaseDbContext<AppDbContext>, IAppDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // CurrentUserName is set automatically by middleware from HttpContext
}
```

#### Repository Pattern
```csharp
// Get repository from UnitOfWork
var repo = _unitOfWork.Repository<StrategicObjective>();

// Read-only queries (no tracking for better performance)
var objectives = await repo.GetAsync(
    predicate: x => x.IsActive && x.PillarId == pillarId,
    orderBy: q => q.OrderBy(x => x.Order),
    includes: new List<Expression<Func<StrategicObjective, object>>>
    {
        x => x.Pillar,
        x => x.Initiatives
    },
    disableTracking: true  // Use TableNoTracking internally
);

// Queries for updates (with tracking)
var objectiveToUpdate = await repo.GetByIdAsync(id);
objectiveToUpdate.NameEn = "Updated Name";
repo.Update(objectiveToUpdate);
await _unitOfWork.SaveChangesAsync();

// Deep relationship loading
var initiative = await repo.GetSingleWithDeepRelationsAsync(
    predicate: x => x.Id == id,
    include: source => source
        .Include(x => x.Program)
            .ThenInclude(p => p.Pillar)
        .Include(x => x.Objectives)
            .ThenInclude(o => o.KPIs)
);

// Direct queryable access
var query = repo.TableNoTracking  // For reads
    .Where(x => x.IsActive)
    .OrderBy(x => x.Order);

var tracked = repo.Table  // For updates
    .Where(x => x.Id == id)
    .Include(x => x.Related);
```

#### Unit of Work Pattern
```csharp
// Application service example
public class StrategicObjectiveAppService
{
    private readonly IUnitOfWork _unitOfWork;

    public async Task<int> CreateObjectiveWithKPIsAsync(ObjectiveDto dto)
    {
        // Get repositories
        var objectiveRepo = _unitOfWork.Repository<StrategicObjective>();
        var kpiRepo = _unitOfWork.Repository<KPI>();

        // Perform operations (tracked but not saved)
        var objective = _mapper.Map<StrategicObjective>(dto);
        await objectiveRepo.InsertAsync(objective, autoSave: false);

        foreach (var kpiDto in dto.KPIs)
        {
            var kpi = _mapper.Map<KPI>(kpiDto);
            kpi.ObjectiveId = objective.Id;
            await kpiRepo.InsertAsync(kpi, autoSave: false);
        }

        // Commit all changes in single transaction
        return await _unitOfWork.SaveChangesAsync();
    }
}
```

### Framework.Identity - Authentication & Authorization

**Core Responsibilities:**
- ASP.NET Core Identity integration
- JWT token management
- User/Role/Claims management
- OTP and verification
- Active Directory integration
- Password policies

**Key Features:**
- `AppIdentityDbContext` - Separate identity database
- `ApplicationUser` / `ApplicationRole` - Custom identity entities
- `JwtAuthAppService` - JWT token generation/validation
- `UserAppService` - User CRUD operations
- `RoleAppService` - Role management
- Auto-registered services via reflection (`IdentityConfigureServices`)

**Usage in Program.cs:**
```csharp
// Identity database configuration
services.IdentityConfigureServices(identityConnection);
services.AddDataProtection(identityConnection);

// Apply migrations
app.UseIdentityDBMigration();
app.UseDataKeysMigration();
```

### Framework.Resources - Localization

**Structure:**
- `SharedResources.resx` - Default/fallback
- `SharedResources.ar.resx` - Arabic
- `SharedResources.en.resx` - English

**Usage:**
```csharp
// In entities: LookupEntityBase provides Name property
public string Name => CultureHelper.IsArabic ? NameAr : NameEn;
```

## Common Development Tasks

### Adding a New Entity

1. **Define in Domain Layer** (`src/SMO.Domain/Entities/`)
```csharp
public class MyEntity : LookupEntityBase<int>
{
    // Additional properties
    public string Description { get; set; }

    // Navigation properties
    public virtual ICollection<RelatedEntity> RelatedEntities { get; set; }
}
```

2. **Register in AppDbContext** (`src/SMO.Infrastructure/Data/AppDbContext.cs`)
```csharp
public DbSet<MyEntity> MyEntities { get; set; }
```

3. **Create Entity Configuration** (`src/SMO.Infrastructure/Mapping/`)
```csharp
public class MyEntityConfiguration : IEntityTypeConfiguration<MyEntity>
{
    public void Configure(EntityTypeBuilder<MyEntity> builder)
    {
        builder.ToTable("MyEntities");
        // Configure relationships, indexes, etc.
    }
}
```

4. **Apply Configuration** (in `AppDbContext.OnModelCreating`)
```csharp
modelBuilder.ApplyConfiguration(new MyEntityConfiguration());
```

5. **Create Migration**
```bash
cd src/SMO.Api
dotnet ef migrations add Add_MyEntity --context AppDbContext
dotnet ef database update
```

### Adding an Application Service

1. **Create Service** (`src/SMO.Application/Services/`)
```csharp
public class MyEntityAppService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public MyEntityAppService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<MyEntityDto>> GetAllAsync()
    {
        var repo = _unitOfWork.Repository<MyEntity>();
        var entities = await repo.GetAsync(
            predicate: x => x.IsActive,
            orderBy: q => q.OrderBy(x => x.Order),
            disableTracking: true
        );
        return _mapper.Map<List<MyEntityDto>>(entities);
    }
}
```

2. **Register Service** (`src/SMO.Application/ServiceCollectionExtensions.cs`)
```csharp
services.AddScoped<MyEntityAppService>();
```

3. **Create Controller** (`src/SMO.Api/Controllers/`)
```csharp
[Route("api/[controller]")]
[ApiController]
[Authorize] // Require authentication
public class MyEntityController : ControllerBase
{
    private readonly MyEntityAppService _service;

    public MyEntityController(MyEntityAppService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<MyEntityDto>>> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }
}
```

### Working with Migrations

**Three Separate Databases:**
1. `SMO_VisionCenter` (Main) - Use `AppDbContext`
2. `SMO_Commons` - Shared data (will be configured in future stories)
3. `SMO_Identity` - Identity data - Use `AppIdentityDbContext`

```bash
# Main database migration
dotnet ef migrations add MigrationName --context AppDbContext
dotnet ef database update --context AppDbContext

# Identity database migration (from Framework.Identity)
cd src/Framework/Framework.Identity
dotnet ef migrations add MigrationName --context AppIdentityDbContext
dotnet ef database update --context AppIdentityDbContext
```

### Best Practices for Queries

**Use TableNoTracking for Read-Only Queries:**
```csharp
// ✅ GOOD - No tracking, better performance
var reports = await repo.GetAsync(
    predicate: x => x.IsActive,
    disableTracking: true
);

// ✅ GOOD - Direct TableNoTracking access
var query = repo.TableNoTracking
    .Where(x => x.CreatedOn >= startDate)
    .Select(x => new ReportDto { /* ... */ });

// ❌ AVOID - Tracking overhead for read-only data
var reports = await repo.GetAsync(
    predicate: x => x.IsActive,
    disableTracking: false  // Default should be true
);
```

**Use Explicit Includes for Navigation Properties:**
```csharp
// ✅ GOOD - Explicit includes, prevents N+1 queries
var objectives = await repo.GetAsync(
    includes: new List<Expression<Func<StrategicObjective, object>>>
    {
        x => x.Pillar,
        x => x.Initiatives,
        x => x.KPIs
    }
);

// ❌ AVOID - Lazy loading causes N+1 queries
var objectives = await repo.GetAsync(); // No includes
foreach (var obj in objectives)
{
    var pillar = obj.Pillar; // N+1 query!
}
```

## Project Structure Reference

```
src/
├── Framework/                          # Shared Framework (Reusable)
│   ├── Framework.Core/                 # Core framework
│   │   ├── Data/                       # Base entities, DbContext, Repository
│   │   │   ├── BaseDbContext.cs        # Auto-auditing context
│   │   │   ├── EntityBase.cs           # Base entity classes
│   │   │   ├── IRepository.cs          # Generic repository interface
│   │   │   ├── RepositoryBase.cs       # Generic repository implementation
│   │   │   └── ChangeTrackerExtensions.cs  # Automatic audit field population
│   │   ├── AutoMapper/                 # AutoMapper utilities
│   │   ├── Validators/                 # FluentValidation base classes
│   │   ├── Notifications/              # Email/SMS/Push services
│   │   ├── Caching/                    # Caching services
│   │   └── Extensions/                 # Extension methods
│   │
│   ├── Framework.Identity/             # Identity & Auth
│   │   ├── Data/
│   │   │   ├── AppIdentityDbContext.cs
│   │   │   ├── Entities/               # User, Role, Claims
│   │   │   ├── Repositories/           # User/Role repositories
│   │   │   ├── Services/               # Auth services
│   │   │   └── IdentityRegisterDependencies.cs
│   │   └── Migrations/
│   │
│   └── Framework.Resources/            # Localization
│       ├── SharedResources.resx
│       ├── SharedResources.ar.resx
│       └── SharedResources.en.resx
│
├── SMO.Api/                            # Web API (Presentation)
│   ├── Controllers/                    # API endpoints
│   ├── Program.cs                      # App startup, DI, middleware
│   ├── appsettings.json                # Configuration
│   └── nlog.config                     # Logging config
│
├── SMO.Application/                    # Use Cases / Business Logic
│   ├── Features/                       # Feature-based organization (future)
│   ├── Services/                       # Application services
│   └── MappingProfiles/                # AutoMapper profiles
│
├── SMO.Domain/                         # Core Business Layer
│   ├── Entities/                       # Domain entities
│   ├── Enums/                          # Enumerations
│   └── Interfaces/
│       ├── IAppDbContext.cs            # DbContext contract
│       ├── IRepository.cs              # Repository contract
│       └── IUnitOfWork.cs              # UoW contract
│
├── SMO.Infrastructure/                 # Data Access & External Services
│   ├── Data/
│   │   ├── AppDbContext.cs             # Main EF Core context
│   │   ├── Repository.cs               # Repository implementation
│   │   └── UnitOfWork.cs               # UoW implementation
│   ├── Repositories/                   # Specialized repositories (if needed)
│   ├── Mapping/                        # EF Core entity configurations
│   └── ApiClients/                     # External API integrations
│
└── SMO.Frontend/SMO-Portal/            # Angular 18 Frontend
    ├── src/
    │   ├── app/
    │   │   ├── features/               # Feature modules
    │   │   ├── core/                   # Core services
    │   │   ├── shared/                 # Shared components
    │   │   └── layout/                 # App layout
    │   └── environments/               # Environment configs
    └── angular.json
```

## Configuration Management

### appsettings.json Structure
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "...",    // Main app database
    "CommonsConnection": "...",    // Shared/commons database
    "IdentityConnection": "..."    // Identity database
  },
  "JwtSettings": {
    "SecretKey": "...",            // Min 64 chars, change in production
    "Issuer": "SMO.VisionCenter",
    "Audience": "SMO.VisionCenter.Client",
    "ExpirationMinutes": 60
  },
  "NotificationSettings": {
    "SmtpHost": "smtp.office365.com",
    "SmtpPort": 587,
    // ...
  },
  "ExternalAPIs": {
    "NafathBaseUrl": "...",
    "SmsGatewayUrl": "...",
    // ...
  },
  "CorsOrigins": ["http://localhost:4200"]
}
```

### Environment-Specific Settings
- `appsettings.Development.json` - Extended JWT (480 min), debug logging
- `appsettings.Production.json` - Production secrets (not in git)
- `appsettings.Staging.json` - Staging environment (not in git)
- `appsettings.Testing.json` - QA environment (not in git)

## Git Workflow

### Commit After Each Task
**CRITICAL:** Run `git commit` after completing each task in a story
```bash
git add .
git commit -m "Task X Complete: Brief Description

Detailed changes:
- Change 1
- Change 2

🤖 Generated with [Claude Code](https://claude.com/claude-code)

Co-Authored-By: Claude <noreply@anthropic.com>"
```

### Branch Strategy
```bash
# Create feature branch
git checkout -b feature/your-feature-name

# Make changes and commit frequently

# Push to remote
git push origin feature/your-feature-name
```

## API Documentation

### Swagger Access
- Development: `https://localhost:7001/swagger`
- Swagger UI shows all endpoints with JWT authentication support

### JWT Authentication
```http
# Login to get token
POST /api/auth/login
Content-Type: application/json

{
  "username": "admin",
  "password": "Admin@123"
}

# Use token in subsequent requests
GET /api/objectives
Authorization: Bearer {token}
```

## Troubleshooting

### Database Connection Issues
```bash
# Verify SQL Server is running
# Check connection string in appsettings.Development.json
# Apply migrations
dotnet ef database update --context AppDbContext
```

### JWT Token Issues
- Verify SecretKey is 64+ characters
- Check token expiration
- Ensure correct `Authorization: Bearer {token}` header
- Clock skew tolerance: 5 minutes

### Build/Package Issues
```bash
# Backend
dotnet clean
dotnet restore
dotnet build

# Frontend
cd src/SMO.Frontend/SMO-Portal
rm -rf node_modules package-lock.json .angular
npm install
```

## Important Notes

1. **No CQRS:** Use single repositories for both reads and writes
2. **Controllers → Services Only:** Never inject repositories in controllers
3. **Commit After Each Task:** Git commit after completing any task
4. **Use TableNoTracking:** For all read-only queries (better performance)
5. **Explicit Includes:** Always specify navigation properties to avoid N+1
6. **Three Databases:** Main (AppDbContext), Commons, Identity (AppIdentityDbContext)
7. **Automatic Auditing:** CreatedBy/UpdatedBy set automatically by BaseDbContext
8. **Framework.Core:** Foundation for all data access patterns

## Additional Resources

- **README.md** - Complete project documentation
- **NuGet Packages** - See README.md Technology Stack section
- **npm Packages** - See README.md Frontend section
- **API Docs** - `https://localhost:7001/swagger` when API is running
