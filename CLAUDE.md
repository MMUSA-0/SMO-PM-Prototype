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

#### ADR-003: Generic Repository Pattern with Direct Injection
**CRITICAL:** Always use `IRepository<TEntity>` by default in Application Services
- **Default Pattern (95% of cases):** Inject `IRepository<TEntity>` directly into AppServices
- **Custom Repositories (Only when needed):** Create custom repository that inherits from `IRepository<TEntity>`
- Inject repositories DIRECTLY into Application Services (NOT through UnitOfWork)
- UnitOfWork is responsible ONLY for transaction management (SaveChanges)
- Custom repositories are created in Infrastructure layer when default methods are insufficient
- `IRepository<T>` provides: Table, TableNoTracking, CRUD, GetAsync, GetByIdAsync, GetSingleWithDeepRelationsAsync

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

**STANDARD ENTITY BASE CLASS:**
```csharp
// ✅ PRIMARY CHOICE - Use for ALL regular entities
FullAuditedEntityBase<TKey>   // Id + Auto auditing (CreatedBy, CreatedOn, UpdatedBy, UpdatedOn)

// ✅ USE FOR LOOKUP/MASTER DATA - Adds bilingual naming and IsActive flag
LookupEntityBase<TKey>        // Inherits from FullAuditedEntityBase + NameAr, NameEn, IsActive, Order

// ❌ RARELY USED - Only for special cases (no auditing needed)
EntityBase<TKey>              // Only Id property (minimal entity, rarely used)
```

**Entity Design Guidelines:**
- **Default Choice:** Use `FullAuditedEntityBase<TKey>` for ALL regular entities (transactions, business data, etc.)
- **Lookup Tables:** Use `LookupEntityBase<TKey>` for master data (statuses, categories, types, etc.) that need bilingual names
- **Auto Auditing:** `FullAuditedEntityBase` automatically populates CreatedBy, CreatedOn, UpdatedBy, UpdatedOn via BaseDbContext
- **Never Skip Auditing:** Unless you have a specific technical reason, always use entities with audit fields

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

#### Repository Pattern (Direct Injection)

**Default Pattern - Use IRepository&lt;TEntity&gt; (95% of cases):**
```csharp
public class StrategicObjectiveAppService
{
    // Inject IRepository<T> directly (NOT custom repository)
    private readonly IRepository<StrategicObjective> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public StrategicObjectiveAppService(
        IRepository<StrategicObjective> repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    // Read-only queries using default GetAsync method
    public async Task<List<StrategicObjective>> GetActiveAsync(int pillarId)
    {
        return await _repository.GetAsync(
            predicate: x => x.IsActive && x.PillarId == pillarId,
            orderBy: q => q.OrderBy(x => x.Order),
            includes: new List<Expression<Func<StrategicObjective, object>>>
            {
                x => x.Pillar,
                x => x.Initiatives
            },
            disableTracking: true  // TableNoTracking for performance
        );
    }

    // Updates using default GetByIdAsync method
    public async Task<bool> UpdateAsync(int id, string newName)
    {
        var objective = await _repository.GetByIdAsync(id);
        if (objective == null) return false;

        objective.NameEn = newName;
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    // Deep relationship loading using default method
    public async Task<StrategicObjective?> GetWithDetailsAsync(int id)
    {
        return await _repository.GetSingleWithDeepRelationsAsync(
            predicate: x => x.Id == id,
            include: source => source
                .Include(x => x.Pillar)
                .Include(x => x.Initiatives)
                    .ThenInclude(i => i.KPIs)
        );
    }

    // Direct queryable access for complex queries
    public async Task<List<ObjectiveDto>> GetCustomReportAsync()
    {
        return await _repository.TableNoTracking
            .Where(x => x.IsActive)
            .OrderBy(x => x.Order)
            .Select(x => new ObjectiveDto { /* projection */ })
            .ToListAsync();
    }
}
```

**Custom Repository Pattern (Only when default methods are insufficient):**
```csharp
// 1. Define custom interface in Domain/Interfaces
public interface IStrategicObjectiveRepository : IRepository<StrategicObjective>
{
    // Add ONLY custom methods not available in IRepository<T>
    Task<List<StrategicObjective>> GetObjectivesWithComplexCalculationsAsync(int pillarId);
    Task<Dictionary<int, decimal>> GetObjectiveProgressByPillarAsync();
}

// 2. Implement in Infrastructure/Repositories
public class StrategicObjectiveRepository
    : Repository<StrategicObjective>, IStrategicObjectiveRepository
{
    public StrategicObjectiveRepository(IAppDbContext context) : base(context) { }

    // Implement ONLY custom methods
    public async Task<List<StrategicObjective>> GetObjectivesWithComplexCalculationsAsync(int pillarId)
    {
        // Complex SQL or stored procedure call
        return await TableNoTracking
            .Where(x => x.PillarId == pillarId)
            .Include(x => x.Initiatives)
            .Include(x => x.KPIs)
            .ToListAsync();
    }

    public async Task<Dictionary<int, decimal>> GetObjectiveProgressByPillarAsync()
    {
        // Custom aggregation logic
        return await TableNoTracking
            .GroupBy(x => x.PillarId)
            .Select(g => new { PillarId = g.Key, Progress = g.Average(x => x.Progress) })
            .ToDictionaryAsync(x => x.PillarId, x => x.Progress);
    }
}

// 3. Register in DI (Infrastructure/ServiceCollectionExtensions.cs)
services.AddScoped<IStrategicObjectiveRepository, StrategicObjectiveRepository>();

// 4. Inject custom repository in AppService
public class StrategicObjectiveAppService
{
    private readonly IStrategicObjectiveRepository _repository; // Custom interface
    private readonly IUnitOfWork _unitOfWork;

    public StrategicObjectiveAppService(
        IStrategicObjectiveRepository repository, // Inject custom repository
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    // Use default IRepository<T> methods
    public async Task<StrategicObjective?> GetByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id); // From IRepository<T>
    }

    // Use custom methods
    public async Task<List<StrategicObjective>> GetComplexDataAsync(int pillarId)
    {
        return await _repository.GetObjectivesWithComplexCalculationsAsync(pillarId);
    }
}
```

#### Unit of Work Pattern (Transaction Only)

**Pattern: Inject IRepository&lt;T&gt; for each entity + IUnitOfWork for transactions**
```csharp
public class StrategicObjectiveAppService
{
    // Inject IRepository<T> for each entity (NOT custom repositories unless needed)
    private readonly IRepository<StrategicObjective> _objectiveRepository;
    private readonly IRepository<KPI> _kpiRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public StrategicObjectiveAppService(
        IRepository<StrategicObjective> objectiveRepository,
        IRepository<KPI> kpiRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _objectiveRepository = objectiveRepository;
        _kpiRepository = kpiRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    // Create operation with multiple entities - UnitOfWork coordinates transaction
    public async Task<int> CreateObjectiveWithKPIsAsync(ObjectiveDto dto)
    {
        // Use default IRepository<T> methods
        var objective = _mapper.Map<StrategicObjective>(dto);
        await _objectiveRepository.InsertAsync(objective, autoSave: false);

        foreach (var kpiDto in dto.KPIs)
        {
            var kpi = _mapper.Map<KPI>(kpiDto);
            kpi.ObjectiveId = objective.Id;
            await _kpiRepository.InsertAsync(kpi, autoSave: false);
        }

        // UnitOfWork commits ALL changes in single transaction
        return await _unitOfWork.SaveChangesAsync();
    }

    // Update operation - UnitOfWork tracks changes and commits
    public async Task<bool> UpdateObjectiveAsync(int id, string newName)
    {
        // Use default GetByIdAsync from IRepository<T>
        var entity = await _objectiveRepository.GetByIdAsync(id);
        if (entity == null) return false;

        // Modify tracked entity
        entity.NameEn = newName;

        // UnitOfWork saves changes (EF Core tracks modification automatically)
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    // Query operation - no UnitOfWork needed (read-only)
    public async Task<List<StrategicObjective>> GetActiveAsync()
    {
        // Use default GetAsync from IRepository<T>
        return await _objectiveRepository.GetAsync(
            predicate: x => x.IsActive,
            orderBy: q => q.OrderBy(x => x.Order),
            disableTracking: true
        );
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

**For Regular Entities (Transactions, Business Data):**
```csharp
// ✅ Use FullAuditedEntityBase<TKey> for regular entities
public class Initiative : FullAuditedEntityBase<int>
{
    // Business properties
    public string NameAr { get; set; }
    public string NameEn { get; set; }
    public string Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal Budget { get; set; }

    // Foreign keys
    public int ProgramId { get; set; }
    public int StatusId { get; set; }

    // Navigation properties
    public virtual Program Program { get; set; }
    public virtual InitiativeStatus Status { get; set; }
    public virtual ICollection<KPI> KPIs { get; set; }
}
```

**For Lookup/Master Data Tables:**
```csharp
// ✅ Use LookupEntityBase<TKey> for master data with bilingual names
public class InitiativeStatus : LookupEntityBase<int>
{
    // LookupEntityBase already provides:
    // - Id, NameAr, NameEn, Name (culture-based)
    // - IsActive, Order
    // - CreatedBy, CreatedOn, UpdatedBy, UpdatedOn

    // Add entity-specific properties (if needed)
    public string ColorCode { get; set; }  // e.g., #FF5733 for UI
    public string IconClass { get; set; }  // e.g., "fa-check-circle"

    // Navigation properties
    public virtual ICollection<Initiative> Initiatives { get; set; }
}
```

2. **Register in AppDbContext** (`src/SMO.Infrastructure/Data/AppDbContext.cs`)
```csharp
public DbSet<Initiative> Initiatives { get; set; }
public DbSet<InitiativeStatus> InitiativeStatuses { get; set; }
```

3. **Create Entity Configuration** (`src/SMO.Infrastructure/Mapping/`)
```csharp
public class InitiativeConfiguration : IEntityTypeConfiguration<Initiative>
{
    public void Configure(EntityTypeBuilder<Initiative> builder)
    {
        builder.ToTable("Initiatives");

        // Primary key (already defined in FullAuditedEntityBase<int>)
        builder.HasKey(x => x.Id);

        // Properties
        builder.Property(x => x.NameAr).IsRequired().HasMaxLength(250);
        builder.Property(x => x.NameEn).IsRequired().HasMaxLength(250);
        builder.Property(x => x.Description).HasMaxLength(2000);
        builder.Property(x => x.Budget).HasPrecision(18, 2);

        // Relationships
        builder.HasOne(x => x.Program)
            .WithMany(p => p.Initiatives)
            .HasForeignKey(x => x.ProgramId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Status)
            .WithMany(s => s.Initiatives)
            .HasForeignKey(x => x.StatusId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(x => x.ProgramId);
        builder.HasIndex(x => x.StatusId);
        builder.HasIndex(x => x.StartDate);
    }
}

public class InitiativeStatusConfiguration : IEntityTypeConfiguration<InitiativeStatus>
{
    public void Configure(EntityTypeBuilder<InitiativeStatus> builder)
    {
        builder.ToTable("InitiativeStatuses");

        // LookupEntityBase already configures NameAr, NameEn, IsActive, Order
        // Configure additional properties only
        builder.Property(x => x.ColorCode).HasMaxLength(7);
        builder.Property(x => x.IconClass).HasMaxLength(50);
    }
}
```

4. **Apply Configuration** (in `AppDbContext.OnModelCreating`)
```csharp
modelBuilder.ApplyConfiguration(new InitiativeConfiguration());
modelBuilder.ApplyConfiguration(new InitiativeStatusConfiguration());
```

5. **Create Migration**
```bash
cd src/SMO.Api
dotnet ef migrations add Add_InitiativesAndStatuses --context AppDbContext
dotnet ef database update
```

### Adding an Application Service

**Default Pattern (Use IRepository&lt;TEntity&gt;):**

1. **Create Service** (`src/SMO.Application/Services/`)
```csharp
public class MyEntityAppService
{
    // ALWAYS inject IRepository<T> by default (NOT custom repository)
    private readonly IRepository<MyEntity> _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public MyEntityAppService(
        IRepository<MyEntity> repository,  // Generic IRepository<T>
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    // Use default GetAsync method
    public async Task<List<MyEntityDto>> GetAllAsync()
    {
        var entities = await _repository.GetAsync(
            predicate: x => x.IsActive,
            orderBy: q => q.OrderBy(x => x.Order),
            disableTracking: true
        );
        return _mapper.Map<List<MyEntityDto>>(entities);
    }

    // Use default InsertAsync method
    public async Task<MyEntityDto> CreateAsync(MyEntityDto dto)
    {
        var entity = _mapper.Map<MyEntity>(dto);
        await _repository.InsertAsync(entity, autoSave: false);

        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<MyEntityDto>(entity);
    }

    // Use default GetByIdAsync method
    public async Task<bool> UpdateAsync(int id, MyEntityDto dto)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return false;

        _mapper.Map(dto, entity);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    // Use TableNoTracking for custom queries
    public async Task<List<MyEntityDto>> GetCustomReportAsync()
    {
        var data = await _repository.TableNoTracking
            .Where(x => x.IsActive)
            .Select(x => new MyEntityDto { /* projection */ })
            .ToListAsync();
        return data;
    }
}
```

2. **Register Service** (`src/SMO.Application/ServiceCollectionExtensions.cs`)
```csharp
services.AddScoped<MyEntityAppService>();
```

**When to Create Custom Repository:**

Create a custom repository ONLY when:
- You need stored procedures
- You have complex SQL queries that don't fit in GetAsync
- You need specialized aggregation methods
- Default IRepository&lt;T&gt; methods are insufficient

Example of when custom repository is needed:
```csharp
// If you need this kind of method frequently:
public async Task<Dictionary<int, decimal>> GetProgressByCategory()
{
    // Complex aggregation not available in IRepository<T>
    return await context.MyEntities
        .GroupBy(x => x.CategoryId)
        .Select(g => new { Category = g.Key, Avg = g.Average(x => x.Progress) })
        .ToDictionaryAsync(x => x.Category, x => x.Avg);
}

// Then create custom repository:
// 1. Interface in Domain/Interfaces
public interface IMyEntityRepository : IRepository<MyEntity>
{
    Task<Dictionary<int, decimal>> GetProgressByCategory();
}

// 2. Implementation in Infrastructure/Repositories
public class MyEntityRepository : Repository<MyEntity>, IMyEntityRepository
{
    public MyEntityRepository(IAppDbContext context) : base(context) { }

    public async Task<Dictionary<int, decimal>> GetProgressByCategory()
    {
        return await TableNoTracking
            .GroupBy(x => x.CategoryId)
            .Select(g => new { Category = g.Key, Avg = g.Average(x => x.Progress) })
            .ToDictionaryAsync(x => x.Category, x => x.Avg);
    }
}

// 3. Register in DI
services.AddScoped<IMyEntityRepository, MyEntityRepository>();

// 4. Inject in service
public MyEntityAppService(
    IMyEntityRepository repository,  // Custom interface
    IUnitOfWork unitOfWork,
    IMapper mapper)
{
    // Can use both IRepository<T> methods AND custom methods
}
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

### Creating Custom Repositories (Only When Needed)

**When to Create Custom Repository:**
- Stored procedures or raw SQL queries
- Complex aggregations not available in `IRepository<T>`
- Entity-specific business logic in data access
- Performance-critical specialized queries

**Step-by-Step Guide:**

**1. Define Interface in Domain** (`src/SMO.Domain/Interfaces/`)
```csharp
// Custom repository MUST inherit from IRepository<TEntity>
public interface IStrategicObjectiveRepository : IRepository<StrategicObjective>
{
    // Add ONLY methods not available in IRepository<T>
    Task<List<StrategicObjective>> GetWithComplexCalculationsAsync(int pillarId);
    Task<Dictionary<int, decimal>> GetProgressByPillarAsync();
    Task<ObjectiveStatistics> GetStatisticsAsync();
}
```

**2. Implement in Infrastructure** (`src/SMO.Infrastructure/Repositories/`)
```csharp
// Inherit from Repository<TEntity> to get all IRepository<T> methods
public class StrategicObjectiveRepository
    : Repository<StrategicObjective>, IStrategicObjectiveRepository
{
    public StrategicObjectiveRepository(IAppDbContext context) : base(context) { }

    // Implement ONLY custom methods
    public async Task<List<StrategicObjective>> GetWithComplexCalculationsAsync(int pillarId)
    {
        // Use inherited TableNoTracking property
        return await TableNoTracking
            .Where(x => x.PillarId == pillarId)
            .Include(x => x.Initiatives)
            .Include(x => x.KPIs)
            .ToListAsync();
    }

    public async Task<Dictionary<int, decimal>> GetProgressByPillarAsync()
    {
        // Complex aggregation
        return await TableNoTracking
            .GroupBy(x => x.PillarId)
            .Select(g => new { PillarId = g.Key, Progress = g.Average(x => x.Progress) })
            .ToDictionaryAsync(x => x.PillarId, x => x.Progress);
    }

    public async Task<ObjectiveStatistics> GetStatisticsAsync()
    {
        // Use raw SQL if needed
        var stats = await Context.Set<StrategicObjective>()
            .FromSqlRaw("EXEC GetObjectiveStatistics")
            .ToListAsync();
        // Process and return
    }
}
```

**3. Register in DI** (`src/SMO.Infrastructure/ServiceCollectionExtensions.cs`)
```csharp
// Register custom repository
services.AddScoped<IStrategicObjectiveRepository, StrategicObjectiveRepository>();
```

**4. Inject in AppService** (`src/SMO.Application/Services/`)
```csharp
public class StrategicObjectiveAppService
{
    // Inject custom repository interface (which includes IRepository<T> methods)
    private readonly IStrategicObjectiveRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public StrategicObjectiveAppService(
        IStrategicObjectiveRepository repository, // Custom interface
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    // Use IRepository<T> methods (inherited)
    public async Task<StrategicObjective?> GetByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id); // From IRepository<T>
    }

    public async Task<List<StrategicObjective>> GetActiveAsync()
    {
        return await _repository.GetAsync( // From IRepository<T>
            predicate: x => x.IsActive,
            disableTracking: true
        );
    }

    // Use custom methods
    public async Task<List<StrategicObjective>> GetComplexDataAsync(int pillarId)
    {
        return await _repository.GetWithComplexCalculationsAsync(pillarId);
    }

    public async Task<Dictionary<int, decimal>> GetProgressAsync()
    {
        return await _repository.GetProgressByPillarAsync();
    }
}
```

**Important:**
- Custom repository inherits from `Repository<TEntity>` to get all base functionality
- Custom repository implements `ICustomRepository : IRepository<TEntity>` interface
- AppService gets both IRepository<T> methods AND custom methods through single injection
- Only create custom repositories when `IRepository<T>` methods are insufficient

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

1. **Entity Inheritance - CRITICAL:** ALL entities MUST inherit from `FullAuditedEntityBase<TKey>` or `LookupEntityBase<TKey>`
   - Regular entities (transactions, business data): Use `FullAuditedEntityBase<TKey>`
   - Lookup/master data (statuses, categories, types): Use `LookupEntityBase<TKey>`
   - NEVER create entities without audit fields unless absolutely necessary
   - Automatic auditing (CreatedBy, CreatedOn, UpdatedBy, UpdatedOn) is handled by BaseDbContext

2. **No CQRS:** Use single repositories for both reads and writes

3. **Controllers → Services Only:** Never inject repositories in controllers

4. **Default: Use IRepository&lt;T&gt;:** ALWAYS inject `IRepository<TEntity>` by default (95% of cases)

5. **Custom Repositories (Rare):** Create custom repository inheriting from `IRepository<T>` ONLY when:
   - Stored procedures needed
   - Complex aggregations not available in IRepository&lt;T&gt;
   - Entity-specific data access logic required

6. **Direct Repository Injection:** Inject repositories directly into AppServices, NOT through UnitOfWork

7. **UnitOfWork for Transactions Only:** UnitOfWork is responsible ONLY for SaveChanges (transaction management)

8. **Commit After Each Task:** Git commit after completing any task

9. **Use TableNoTracking:** For all read-only queries (better performance)

10. **Explicit Includes:** Always specify navigation properties to avoid N+1

11. **Three Databases:** Main (AppDbContext), Commons, Identity (AppIdentityDbContext)

12. **Framework.Core:** Foundation for all data access patterns

## Additional Resources

- **README.md** - Complete project documentation
- **NuGet Packages** - See README.md Technology Stack section
- **npm Packages** - See README.md Frontend section
- **API Docs** - `https://localhost:7001/swagger` when API is running
- after each commit of task run the agent --agent task-code-reviewer
## Development Best Practices

- Run git commit after each task completed in any story
- DON'T Ever Use CQRS
- Controllers should ONLY inject Application Services, NEVER repositories!
- The PRD contains 33 Use Cases, maintain this use case as an User Story and the grouping as an Epics :
  Epic Breakdown

  Epic 1: معلومات الرؤية (Vision Information Management)

  3 User Stories | Foundation module for managing Vision 2030 core structure

  | #     | User Story              | Description                                                                                       |
  |-------|-------------------------|---------------------------------------------------------------------------------------------------|
  | UC-01 | عرض معلومات الرؤية      | Display full Vision 2030 text with progress dashboard and key indicators                          |
  | UC-02 | إدارة الركائز           | Manage strategic pillars (Vibrant Society, Thriving Economy) - CRUD operations with audit logging |
  | UC-03 | إدارة المحاور التفصيلية | Manage detailed themes linked to pillars, entities, and objectives                                |

  Epic Priority: 🔴 Critical - Foundation for entire systemComplexity: MediumDependencies: None (Starting point)

  ---
  Epic 2: الأهداف الإستراتيجية (Strategic Objectives Management)

  6 User Stories | Core strategic objectives hierarchy (Levels 1-5)

  | #     | User Story                             | Description                                                                              |
  |-------|----------------------------------------|------------------------------------------------------------------------------------------|
  | UC-04 | عرض قائمة الأهداف الإستراتيجية         | Display objectives by levels (1-5) in table/card view with codes, names, and KPIs        |
  | UC-05 | إضافة هدف استراتيجي جديد               | Create new objective with code, name, description, and links to themes/parent objectives |
  | UC-06 | تعديل معلومات الهدف الاستراتيجي        | Edit objective details without changing code/level, with change tracking                 |
  | UC-07 | حذف هدف استراتيجي                      | Delete objective with dependency validation (check linked initiatives)                   |
  | UC-08 | عرض تفاصيل الهدف الاستراتيجي           | View complete objective details including description, KPIs, links, with export options  |
  | UC-09 | البحث والتصفية في الأهداف الإستراتيجية | Filter objectives by level, theme, program with advanced text search                     |

  Epic Priority: 🔴 CriticalComplexity: High (hierarchical structure with 5 levels)Dependencies: Epic 1 (Pillars & Themes)

  ---
  Epic 3: برامج الرؤية (Vision Programs Management)

  7 User Stories | Manage 13 Vision Realization Programs

  | #     | User Story                     | Description                                                       |
  |-------|--------------------------------|-------------------------------------------------------------------|
  | UC-10 | عرض قائمة برامج الرؤية         | Display list of 13 programs with dimensions and linked objectives |
  | UC-11 | إضافة برنامج رؤية جديد         | Create new program with name, description, and objective links    |
  | UC-12 | تعديل معلومات البرنامج         | Edit program data without changing core links, with audit trail   |
  | UC-13 | حذف برنامج                     | Delete program with validation (check linked initiatives/KPIs)    |
  | UC-14 | عرض تفاصيل البرنامج            | View program details including dimensions, objectives, and KPIs   |
  | UC-15 | إدارة الأبعاد                  | Manage program dimensions (add, edit, delete) with links          |
  | UC-16 | التصفية والبحث المتقدم للبرامج | Filter programs by dimensions/objectives with advanced search     |

  Epic Priority: 🟡 HighComplexity: MediumDependencies: Epic 2 (Strategic Objectives)

  ---
  Epic 4: مبادرات الرؤية (Vision Initiatives Management)

  9 User Stories | Most complex module with deliverables, milestones, and document versioning

  | #     | User Story                       | Description                                                                |
  |-------|----------------------------------|----------------------------------------------------------------------------|
  | UC-17 | عرض قائمة المبادرات              | Display initiatives list with basic details and program links              |
  | UC-18 | إضافة مبادرة جديدة               | Create initiative with name, description, and links to objectives/programs |
  | UC-19 | حذف مبادرة                       | Delete initiative with dependency validation (deliverables/milestones)     |
  | UC-20 | عرض تفاصيل المبادرة              | View complete details including deliverables, milestones, and documents    |
  | UC-21 | إدارة مخرجات المبادرة            | Manage initiative deliverables (CRUD) with initiative links                |
  | UC-22 | إدارة معالم المبادرة             | Manage initiative milestones (CRUD with progress tracking)                 |
  | UC-23 | إدارة مستندات المبادرة(تحسين)    | Upload, edit, delete initiative documents with version control             |
  | UC-24 | تتبع تغييرات المبادرة            | Track change request history with timestamps and users                     |
  | UC-25 | التصفية والبحث المتقدم للمبادرات | Filter initiatives by program/status with advanced search                  |

  Epic Priority: 🔴 CriticalComplexity: ⚠️ Very High (Most complex epic - 9 stories with sub-entities)Dependencies: Epic 2, Epic 3

  ---
  Epic 5: مؤشرات الرؤية (Vision KPIs Management)

  8 User Stories | KPI management with formulas and targets

  | #     | User Story                      | Description                                                     |
  |-------|---------------------------------|-----------------------------------------------------------------|
  | UC-26 | عرض قائمة المؤشرات              | Display KPIs list with basic details and links                  |
  | UC-27 | إضافة مؤشر جديد                 | Create new KPI with code, name, description, and formula        |
  | UC-28 | تعديل معلومات المؤشر            | Edit KPI data without changing code, with audit trail           |
  | UC-29 | حذف مؤشر                        | Delete KPI with dependency validation                           |
  | UC-30 | عرض تفاصيل المؤشر               | View KPI details including formula, targets, and performance    |
  | UC-31 | إدارة معادلة المؤشر             | Define/edit KPI calculation formula with components and sources |
  | UC-32 | إدارة مستهدفات المؤشر           | Set and update annual KPI target values with notes              |
  | UC-33 | التصفية والبحث المتقدم للمؤشرات | Filter KPIs by type, frequency, or entity with advanced search  |

  Epic Priority: 🔴 CriticalComplexity: High (Complex formulas and calculations)Dependencies: Epic 2, Epic 3, Epic 4

  ---
  Statistics Summary

  | Metric             | Count               |
  |--------------------|---------------------|
  | Total Epics        | 5                   |
  | Total User Stories | 33                  |
  | Epic 1 Stories     | 3 (9.1%)            |
  | Epic 2 Stories     | 6 (18.2%)           |
  | Epic 3 Stories     | 7 (21.2%)           |
  | Epic 4 Stories     | 9 (27.3%) ⭐ Largest |
  | Epic 5 Stories     | 8 (24.2%)           |

  ---
  Complexity Analysis

  | Epic   | Complexity   | Reason                                                                                      |
  |--------|--------------|---------------------------------------------------------------------------------------------|
  | Epic 1 | 🟢 Medium    | Foundation - 3 simple CRUD entities                                                         |
  | Epic 2 | 🟡 High      | 5-level hierarchical structure, parent-child relationships                                  |
  | Epic 3 | 🟢 Medium    | 13 programs with dimensions management                                                      |
  | Epic 4 | 🔴 Very High | 9 stories, nested entities (deliverables, milestones), document versioning, change tracking |
  | Epic 5 | 🟡 High      | Complex formulas, calculation logic, annual targets                                         |
