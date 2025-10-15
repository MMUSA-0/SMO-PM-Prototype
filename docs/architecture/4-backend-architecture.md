# 4. Backend Architecture

## 4.1 Clean Architecture Layers

### **Domain Layer** (`LMS.Domain`)

The innermost layer containing business entities and interfaces.

**Responsibilities:**
- Define domain entities
- Define repository interfaces
- Define business rules (through domain entities)
- NO dependencies on infrastructure or frameworks

**Key Components:**
```csharp
// Domain Interfaces
- IRepository<TEntity>         // Generic repository contract
- IUnitOfWork                  // Transaction management
- IAppDbContext               // Database context abstraction
- IRepositoryBaseStoreProcedure // Stored procedure support
```

**Structure:**
```
SMO.Domain/
├── Entities/          # Vision 2030 domain entities
│   ├── Pillar.cs              # Strategic Pillars (3 pillars)
│   ├── Theme.cs               # Detailed Themes (Mahawer)
│   ├── StrategicObjective.cs  # Strategic Objectives (5 levels)
│   ├── VisionProgram.cs       # Vision Realization Programs (VRPs)
│   ├── Initiative.cs          # Strategic Initiatives
│   ├── KPI.cs                 # Key Performance Indicators
│   ├── InitiativeMilestone.cs # Initiative milestones
│   └── InitiativeOutput.cs    # Initiative deliverables
├── Enums/
│   └── Enums.cs      # Domain enumerations (ObjectiveLevel, KPIStatus, etc.)
└── Interfaces/
    ├── IRepository.cs
    ├── IUnitOfWork.cs
    ├── IAppDbContext.cs
    └── IRepositoryBaseStoreProcedure.cs
```

### **Application Layer** (`SMO.Application`)

Contains business logic and orchestrates domain operations for Vision 2030 strategic management.

**Responsibilities:**
- Implement use cases / business services
- Orchestrate domain entities
- Define DTOs for data transfer
- Coordinate transactions via Unit of Work
- NEVER inject repositories directly (use services pattern)

**Implementation Status:** ✅ **Fully Implemented with Feature-Based Organization**

**Actual Structure:**
```
SMO.Application/
├── Features/                  # Feature-based interfaces
│   ├── Attachment/           # IAttachmentAppService
│   ├── Pillar/               # IPillarAppService
│   ├── Theme/                # IThemeAppService
│   ├── StrategicObjective/   # IStrategicObjectiveAppService
│   ├── VisionProgram/        # IVisionProgramAppService
│   ├── Initiative/           # IInitiativeAppService
│   ├── KPI/                  # IKPIAppService
│   ├── InitiativeMilestone/  # IInitiativeMilestoneAppService
│   ├── Lookup/               # ILookupAppService
│   └── Workflow/             # IWorkflowAppService (change management)
├── Services/                 # Concrete implementations
│   ├── Attachment/           # File management services
│   ├── Pillar/               # Strategic pillar management
│   ├── Theme/                # Theme management
│   ├── StrategicObjective/   # Objective management (5 levels)
│   ├── VisionProgram/        # VRP management
│   ├── Initiative/           # Initiative tracking & management
│   ├── KPI/                  # KPI definition & tracking
│   └── Dashboard/            # Performance dashboards
├── MappingProfiles/          # AutoMapper profiles
│   └── ApplicationAutoMappingProfile.cs
└── ServiceCollectionExtensions.cs  # DI registration with auto-discovery
```

**Key Services:**
- `IAttachmentAppService` - Document attachments for initiatives/programs
- `IPillarAppService` - Strategic Pillar management (3 pillars of Vision 2030)
- `IThemeAppService` - Detailed themes linked to pillars
- `IStrategicObjectiveAppService` - Strategic objectives (5-level hierarchy)
- `IVisionProgramAppService` - Vision Realization Program (VRP) management
- `IInitiativeAppService` - Strategic initiative tracking & milestones
- `IKPIAppService` - KPI definition, targets, and performance tracking
- `ExcelImportService` - Bulk data import for objectives/initiatives/KPIs

**Auto-Registration Pattern:**
```csharp
services.RegisterAssemblyPublicNonGenericClasses(Assembly.GetAssembly(typeof(InitiativeAppService)))
    .Where(c => c.Name.EndsWith("AppService"))
    .AsConcreteTypesScoped();

services.RegisterAssemblyPublicNonGenericClasses(Assembly.GetAssembly(typeof(IKPIAppService)))
    .Where(c => c.Name.EndsWith("AppService"))
    .AsPublicImplementedInterfaces(ServiceLifetime.Scoped);
```

**Background Jobs:**
- Recurring job for cleaning up unused attachments (monthly)
- Periodic KPI data refresh from external systems (ADAA, GaStat)
- Quarterly report generation
```csharp
RecurringJob.AddOrUpdate("UnUsedAttachments",
    () => attachmentUnUsedServices.DeleteUnUsedAttachments(),
    Cron.Monthly);

RecurringJob.AddOrUpdate("RefreshKPIData",
    () => kpiDataRefreshService.RefreshFromExternalSystems(),
    Cron.Daily);
```

### **Infrastructure Layer** (`SMO.Infrastructure`)

Implements external concerns and data access for Vision 2030 strategic data.

**Responsibilities:**
- Implement repository interfaces
- Implement Unit of Work pattern
- Configure EF Core DbContext
- Implement external service integrations
- Database migrations

**Key Files:**

```csharp
// AppDbContext.cs - Main database context
public class AppDbContext : BaseDbContext<AppDbContext>, IAppDbContext
{
    // Inherits from Framework.Core.Data.BaseDbContext
    // Auto-discovers entity configurations via reflection
    // Extracts username from HttpContext claims
}

// Repository.cs - Generic repository implementation
public class Repository<TEntity> : RepositoryBase<IAppDbContext, TEntity>, IRepository<TEntity>
{
    // Inherits comprehensive CRUD from Framework.Core
}

// UnitOfWork.cs - Transaction coordinator
public sealed class UnitOfWork : UnitOfWorkBase<IAppDbContext>, IUnitOfWork
{
    // Manages transaction boundaries
}

// ServiceCollectionExtensions.cs - DI registration with auto-discovery
public static void ConfigureInfrastructureServices(this IServiceCollection services, string connectionString)
{
    services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

    services.AddScoped<IAppDbContext, AppDbContext>();
    services.AddScoped<IUnitOfWork, UnitOfWork>();
    services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

    // Register HTTP clients for external services
    services.AddHttpClient<ISmsService, SMSServiceProxy>();
    services.AddHttpClient<INafathServiceProxy, NafathServiceProxy>();

    // Auto-register all specialized repositories
    Assembly.GetExecutingAssembly()
        .GetTypes()
        .Where(a => a.Name.EndsWith("Repository") && !a.IsAbstract && !a.IsInterface)
        .Select(a => new { assignedType = a, serviceTypes = a.GetInterfaces().ToList() })
        .ToList()
        .ForEach(typesToRegister =>
        {
            typesToRegister.serviceTypes.ForEach(typeToRegister =>
                services.AddScoped(typeToRegister, typesToRegister.assignedType));
        });
}
```

**Structure:**
```
SMO.Infrastructure/
├── Data/
│   ├── AppDbContext.cs        # Enhanced with global filters
│   ├── Repository.cs
│   └── UnitOfWork.cs
├── Repositories/              # 15+ Specialized repositories
│   ├── Vision/
│   │   ├── PillarRepository.cs
│   │   ├── ThemeRepository.cs
│   │   └── VisionInformationRepository.cs
│   ├── StrategicObjective/
│   │   ├── StrategicObjectiveRepository.cs
│   │   ├── ObjectiveRelationshipRepository.cs
│   │   └── ObjectiveLevelRepository.cs
│   ├── Program/
│   │   ├── VisionProgramRepository.cs
│   │   ├── ProgramDimensionRepository.cs
│   │   ├── ProgramCommunicationPlanRepository.cs
│   │   └── ProgramImplementationPlanRepository.cs
│   ├── Initiative/
│   │   ├── InitiativeRepository.cs
│   │   ├── InitiativeMilestoneRepository.cs
│   │   ├── InitiativeOutputRepository.cs
│   │   └── InitiativeChangeRequestRepository.cs
│   ├── KPI/
│   │   ├── KPIRepository.cs
│   │   ├── KPIFormulaRepository.cs
│   │   ├── KPITargetRepository.cs
│   │   └── KPIActualRepository.cs
│   └── ProgramDocumentRepository.cs
├── Mapping/                   # EF Core entity configurations
│   ├── Vision/                # Pillars, Themes
│   ├── StrategicObjective/    # Objectives hierarchy
│   ├── Program/               # VRP configurations
│   ├── Initiative/            # Initiatives & milestones
│   ├── KPI/                   # KPI definitions & data
│   ├── Lookup/
│   └── Attachment/
├── ApiClients/                # External service integrations
│   ├── NafathServiceProxy.cs # Saudi national auth integration
│   ├── SMSServiceProxy.cs    # SMS gateway integration
│   ├── AdaaServiceProxy.cs   # ADAA platform integration
│   └── GaStatServiceProxy.cs # GaStat data integration
└── ServiceCollectionExtensions.cs
```

### **API Layer** (`SMO.Api`)

ASP.NET Core Web API entry point for Vision 2030 strategic management.

**Responsibilities:**
- Expose HTTP endpoints for Vision 2030 data management
- Handle HTTP concerns (routing, model binding, status codes)
- Inject application services (NEVER repositories!)
- Configure middleware pipeline
- Configure dependency injection

**Implementation Status:** ✅ **Fully Implemented - 17+ Controllers, 3,652+ Lines**

**Implemented Controllers:**
1. `AccountController` - User authentication & management (SMO users, VRP offices)
2. `AttachmentController` - File upload/download for documents & reports
3. `PillarController` - Strategic pillar management
4. `ThemeController` - Detailed theme management
5. `StrategicObjectiveController` - Strategic objective management (5 levels)
6. `VisionProgramController` - Vision Realization Program (VRP) management
7. `ProgramDimensionController` - Program dimension management
8. `InitiativeController` - Strategic initiative management
9. `InitiativeMilestoneController` - Initiative milestone tracking
10. `InitiativeOutputController` - Initiative output/deliverable management
11. `KPIController` - KPI definition & management
12. `KPITargetController` - KPI target setting
13. `KPIActualController` - KPI actual performance recording
14. `LookupController` - Lookup/reference data
15. `NafathController` - Saudi national auth integration (Nafath)
16. `DashboardController` - Performance dashboards & visualizations
17. `WorkflowController` - Change management workflow orchestration

**Program.cs Configuration (Comprehensive Setup):**
```csharp
var builder = WebApplication.CreateBuilder(args);

// NLog configuration
builder.Logging.ClearProviders();
builder.Host.UseNLog();
builder.Services.AddHttpLogging(/* ... */);

// Database & services
services.ConfigureSharedApplicationServices(connectionString);
services.ConfigureApplicationServices();
services.ConfigureInfrastructureServices(connectionString);
services.IdentityConfigureServices(connectionString);
services.AddDataProtection(connectionString);
services.InitHangFireServices(connectionString);
services.AddSignalR();

// JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => { /* JWT config */ });

// JSON configuration
builder.Services.AddControllers().AddJsonOptions(options => {
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    /* ... */
});

// CORS
builder.Services.AddCors(options => {
    options.AddPolicy("corsapp", builder => { /* CORS config */ });
});

// Swagger with JWT support
builder.Services.AddSwaggerGen(options => {
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme() { /* ... */ });
    options.AddSecurityRequirement(/* ... */);
});

var app = builder.Build();

// Middleware pipeline
app.UseSharedMiddlewares();      // Exception, globalization, etc.
app.UseHttpLogging();
app.UseRouting();
app.UseHttpsRedirection();
app.UseCors("corsapp");
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints => {
    endpoints.MapControllers();
    endpoints.MapHangfireDashboard();
    endpoints.MapHub<SignalR>("/api/LiveInform");  // SignalR hub
});

// Database migrations
app.UseIdentityDBMigration();
app.UseDataKeysMigration();
app.UseSharedCommonDBMigration();
app.UseApplicationDBMigration();

app.InitHangfireDashboard();
app.Run();
```

**Structure:**
```
SMO.Api/
├── Controllers/       # 17+ API controllers (3,652+ lines)
├── Program.cs        # Comprehensive startup configuration
└── appsettings.json  # Multi-environment configuration
```

## 4.2 Architectural Patterns

### Repository Pattern

**Generic Repository Interface:**
```csharp
public interface IRepositoryBase<TContext, TEntity>
{
    IQueryable<TEntity> Table { get; }
    IQueryable<TEntity> TableNoTracking { get; }

    // CRUD Operations
    TEntity Insert(TEntity entity, bool autoSave = false);
    Task<TEntity> InsertAsync(TEntity entity, bool autoSave = false);
    TEntity Update(TEntity entity, bool autoSave = false);
    void Delete(TEntity entity, bool autoSave = false);

    // Querying
    Task<TEntity> GetByIdAsync(object id);
    Task<List<TEntity>> GetAsync(
        Expression<Func<TEntity, bool>> predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        List<Expression<Func<TEntity, object>>> includes = null,
        bool disableTracking = true);

    // Advanced Queries
    Task<TEntity> GetSingleWithDeepRelationsAsync(
        Expression<Func<TEntity, bool>> predicate = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
        bool disableTracking = true);

    // Pagination
    PagedList<TEntity> SearchWithFilters(
        int pageNumber, int pageSize,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy,
        IEnumerable<Expression<Func<TEntity, bool>>> filters = null,
        params Expression<Func<TEntity, object>>[] includes);
}
```

**Key Features:**
- Generic implementation for all entities
- Tracking vs. No-Tracking query support
- Flexible filtering with expression trees
- Deep relation loading support
- Built-in pagination
- Optional auto-save per operation

### Unit of Work Pattern

```csharp
public interface IUnitOfWorkBase<TContext> where TContext : IBaseDbContext
{
    // Transaction management
    int SaveChanges();
    Task<int> SaveChangesAsync();

    // Repository access
    IRepositoryBase<TContext, TEntity> Repository<TEntity>() where TEntity : class;
}
```

**Benefits:**
- Ensures atomic transactions
- Coordinates multiple repository operations
- Single point of persistence control

---
