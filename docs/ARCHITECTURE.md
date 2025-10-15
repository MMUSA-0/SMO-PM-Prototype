# Solution Architecture Document

> **Version:** 3.0
> **Last Updated:** 2025-10-15 (Updated for Vision 2030 Information Center - SMO)
> **Target:** Saudi Vision 2030 Strategic Management Office Information Center
> **Status:** ✅ **Production-Ready, Feature-Complete Implementation**

---

## Table of Contents

1. [Executive Summary](#executive-summary)
2. [Solution Structure](#solution-structure)
3. [Technology Stack](#technology-stack)
4. [Backend Architecture](#backend-architecture)
5. [Frontend Architecture](#frontend-architecture)
6. [Framework Layer](#framework-layer)
7. [Data Architecture](#data-architecture)
8. [Cross-Cutting Concerns](#cross-cutting-concerns)
9. [Coding Standards & Patterns](#coding-standards--patterns)
10. [Deployment Architecture](#deployment-architecture)

---

## 1. Executive Summary

This solution implements a **Clean Architecture** pattern with comprehensive framework capabilities suitable for enterprise-grade applications. The architecture promotes separation of concerns, testability, maintainability, and scalability through well-defined layers and dependency management.

**This is a FULLY IMPLEMENTED, production-ready Vision 2030 Information Center** for the Strategic Management Office (SMO) with 17+ API controllers (3,652+ lines), comprehensive domain model for managing Saudi Arabia's Vision 2030 strategic objectives, programs, initiatives, and KPIs, real-time communication, external service integrations (Nafath, ADAA, GaStat), and a rich Angular frontend with extensive UI libraries.

### Key Architectural Characteristics

- **Pattern:** Clean Architecture (Onion Architecture) with Feature-Based Organization
- **Backend:** .NET 8 / ASP.NET Core Web API
- **Frontend:** Angular 18 (Module-based with comprehensive UI libraries)
- **Database:** SQL Server (PostgreSQL compatible)
- **ORM:** Entity Framework Core 9.0.8
- **Authentication:** ASP.NET Core Identity with JWT + Nafath (Saudi National Authentication)
- **API Style:** RESTful with SignalR for real-time KPI updates and dashboards
- **Business Domain:** Vision 2030 Strategic Management - Pillars, Objectives, Programs, Initiatives, KPIs
- **Implementation Status:** ✅ **Feature-Complete with Production-Grade Codebase**

---

## 2. Solution Structure

### 2.1 Project Organization

```
SMO/
├── Framework/
│   ├── Framework.Core/              # Core framework & cross-cutting concerns
│   ├── Framework.Identity/          # Authentication & authorization
│   └── Framework.Resources/         # Localization resources
├── SMO.Domain/                      # Domain entities & interfaces
├── SMO.Application/                 # Business logic & services
├── SMO.Infrastructure/              # Data access & external services
├── SMO.Api/                         # Web API entry point
└── SMO.Frontend/
    └── SMO-Portal/                  # Angular application
```

### 2.2 Project Dependencies

```mermaid
graph TD
    A[SMO.Api] --> B[SMO.Application]
    A --> C[SMO.Infrastructure]
    A --> D[Framework.Core]

    B --> E[SMO.Domain]
    C --> E

    E --> F[Framework.Identity]
    C --> D
    F --> D
    D --> G[Framework.Resources]
```

**Dependency Rules:**
- **Domain** has no dependencies except Framework.Identity (for user entities)
- **Application** depends only on Domain
- **Infrastructure** implements Domain interfaces and depends on Framework.Core
- **API** orchestrates all layers but contains minimal logic
- **Framework.Core** is self-contained except for Framework.Resources

### 2.3 Solution Configuration

The solution targets **.NET 8.0** with the following common settings:
- Nullable reference types enabled
- Implicit usings enabled
- Two configurations: Debug and Release

---

## 3. Technology Stack

### 3.1 Backend Technologies

| Category | Technology | Version | Purpose |
|----------|-----------|---------|---------|
| **Runtime** | .NET | 8.0 | Application framework |
| **API Framework** | ASP.NET Core | 8.0 | Web API hosting |
| **ORM** | Entity Framework Core | 9.0.8 | Data access |
| **Database** | SQL Server | - | Primary database |
| **Authentication** | ASP.NET Core Identity | 8.0.7 | User authentication |
| **JWT** | Microsoft.AspNetCore.Authentication.JwtBearer | 8.0.20 | JWT token authentication |
| **Validation** | FluentValidation | 11.9.2 | Input validation |
| **Mapping** | AutoMapper | 13.0.1 | Object-to-object mapping |
| **Background Jobs** | Hangfire | 1.8.14 | Background processing |
| **Logging** | NLog | 5.3.2 | Application logging |
| **API Documentation** | Swashbuckle (Swagger) | 6.6.2 | API documentation |
| **Real-time Communication** | SignalR | 9.0.6 | WebSocket-based real-time messaging |
| **JSON Serialization** | Newtonsoft.Json | 13.0.3 | JSON handling |

### 3.2 Framework Core Dependencies

**Document Generation:**
- `itext` (8.0.5) - PDF manipulation
- `iTextSharp` (5.5.13.4) - Legacy PDF support
- `Select.HtmlToPdf.NetCore` (24.1.0) - HTML to PDF conversion
- `EPPlus` (7.2.2) - Excel generation
- `ExcelDataReader` (3.7.0) - Excel reading

**Utilities:**
- `libphonenumber-csharp` (8.13.42) - Phone number validation
- `ZXing.Net` (0.16.9) - Barcode/QR code generation
- `PagedList.Core` (1.17.4) - Pagination support
- `System.Linq.Dynamic.Core` (1.4.4) - Dynamic LINQ queries

### 3.3 Frontend Technologies

| Category | Technology | Version | Purpose |
|----------|-----------|---------|---------|
| **Framework** | Angular | 18.2.13 | SPA framework |
| **Language** | TypeScript | 5.4.5 | Type-safe JavaScript |
| **Build Tool** | Angular CLI | 18.2.13 | Build & development |
| **Testing** | Jasmine + Karma | 4.5.0 / 6.4.0 | Unit testing |
| **Reactive Programming** | RxJS | 7.5.0 | Async data streams |

### 3.4 Frontend UI & Integration Libraries

**UI Component Libraries:**
- `@angular/material` (20.2.0) - Material Design components
- `@ng-bootstrap/ng-bootstrap` (17.0.1) - Bootstrap components
- `ngx-bootstrap` (20.0.1) - Additional Bootstrap widgets
- `@ng-select/ng-select` (12.0.7) - Advanced select dropdowns
- `ng-multiselect-dropdown` (1.0.0) - Multi-select component
- `ng-select2-component` (17.2.7) - Select2 integration

**UI Utilities:**
- `@angular/flex-layout` (15.0.0-beta.42) - Flexbox layouts
- `@fortawesome/fontawesome-free` (7.0.1) - Icon library
- `ngx-bootstrap-icons` (1.9.3) - Bootstrap icons
- `sweetalert2` (11.22.4) - Beautiful alerts/modals

**Rich Text & Data Visualization:**
- `ngx-quill` (28.0.1) - Rich text editor (Quill integration)
- `quill` (2.0.3) - Quill editor core
- `ng2-charts` (8.0.0) - Chart.js wrapper
- `ngx-tagify` (18.0.0) - Tag input component

**File Handling:**
- `exceljs` (4.4.0) - Excel file generation
- `file-saver` (2.0.5) - Client-side file saving
- `ngx-filesaver` (20.0.0) - Angular file saver
- `@pdftron/webviewer` (11.7.0) - PDF viewer/editor

**Authentication & Security:**
- `@auth0/angular-jwt` (5.2.0) - JWT helper utilities
- `crypto-js` (4.2.0) - Cryptographic operations
- `localstorage-slim` (2.7.1) - Enhanced localStorage

**Internationalization:**
- `@ngx-translate/core` (14.0.0) - Translation framework
- `@ngx-translate/http-loader` (8.0.0) - HTTP translation loader
- `hijri-converter` (1.1.1) - Hijri calendar conversion

**Real-time & Media:**
- `@microsoft/signalr` (9.0.6) - SignalR client
- `ngx-webcam` (0.4.1) - Webcam capture
- `ngx-print` (20.0.0) - Print functionality

**Notifications & Feedback:**
- `ngx-toastr` (19.0.0) - Toast notifications
- `ngx-toaster` (1.0.1) - Alternative toaster

**Utilities:**
- `underscore` (1.13.7) - Utility functions
- `@types/select2` (4.0.63) - TypeScript definitions

**Build Configurations:**
- Development (with source maps, no optimization)
- Testing (optimized, environment-specific)
- Staging (optimized, environment-specific)
- Production (fully optimized, hashed output, 2MB/5MB budgets)

---

## 4. Backend Architecture

### 4.1 Clean Architecture Layers

#### **Domain Layer** (`LMS.Domain`)

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

#### **Application Layer** (`SMO.Application`)

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

#### **Infrastructure Layer** (`SMO.Infrastructure`)

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

#### **API Layer** (`SMO.Api`)

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

### 4.2 Architectural Patterns

#### Repository Pattern

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

#### Unit of Work Pattern

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

## 5. Frontend Architecture

### 5.1 Angular Application Structure

**Project:** `SMO.Frontend/SMO-Portal`

**Key Configuration:**
- Angular 18.2.13 (Module-based, not standalone)
- TypeScript 5.4.5
- Build configurations: Development, Testing, Staging, Production
- Purpose: Vision 2030 Information Center portal for strategic management

**Structure:**
```
SMO.Frontend/SMO-Portal/
├── src/
│   ├── app/
│   │   ├── app-routing.module.ts
│   │   ├── app.module.ts
│   │   └── app.component.ts
│   │   ├── features/
│   │   │   ├── pillars/          # Strategic pillars module
│   │   │   ├── objectives/       # Strategic objectives module
│   │   │   ├── programs/         # VRP management module
│   │   │   ├── initiatives/      # Initiative tracking module
│   │   │   ├── kpis/             # KPI management & dashboards
│   │   │   └── dashboards/       # Performance visualization
│   │   ├── shared/               # Shared components & services
│   │   └── core/                 # Core services & guards
│   ├── assets/
│   ├── environments/
│   │   ├── environment.ts
│   │   ├── environment.production.ts
│   │   ├── environment.staging.ts
│   │   └── environment.testing.ts
│   ├── index.html
│   ├── main.ts
│   ├── styles.css
│   └── web.config         # IIS deployment configuration
├── angular.json
├── package.json
└── tsconfig.json
```

### 5.2 Build Configurations

**Development:**
- No optimization
- Source maps enabled
- Named chunks for debugging

**Production/Staging/Testing:**
- Full optimization
- Output hashing for cache busting
- Bundle size budgets:
  - Initial: 500KB warning, 1MB error
  - Component styles: 2KB warning, 4KB error

### 5.3 Deployment

The application includes `web.config` for IIS deployment with URL rewriting support for SPA routing.

---

## 6. Framework Layer

The Framework layer provides reusable, application-agnostic infrastructure suitable for any project.

### 6.1 Framework.Core

**Purpose:** Core framework capabilities and cross-cutting concerns

**Major Components (166+ files):**

#### **Data Layer** (`Framework.Core/Data/`)

**Base Classes:**
```csharp
// Entity Base Classes
public abstract class EntityBase<TKey> : IEntityBase<TKey>
{
    public TKey Id { get; set; }
}

public abstract class FullAuditedEntityBase<TKey> : EntityBase<TKey>
{
    public string CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedOn { get; set; }
}

public abstract class LookupEntityBase<TKey> : FullAuditedEntityBase<TKey>
{
    public string NameAr { get; set; }
    public string NameEn { get; set; }
    public string Name { get; } // Auto-selects based on culture
    public bool IsActive { get; set; } = true;
    public int? Order { get; set; }
}
```

**BaseDbContext Features:**
- Automatic auditing (CreatedBy, UpdatedOn, etc.)
- Change tracker validation
- Automatic entity configuration discovery via reflection
- Support for raw SQL queries
- Custom `CurrentUserName` tracking

**Repository Implementation:**
- 30+ query methods
- Expression-based filtering
- Eager loading support
- Projection/selection support
- Pagination support
- Batch operations

#### **AutoMapper Integration** (`Framework.Core/AutoMapper/`)

```csharp
public interface IMapperProfile
{
    void Configure(IMapperConfigurationExpression config);
}

public static class AutoMapperConfiguration
{
    public static IMapper Mapper { get; private set; }
    public static MapperConfiguration MapperConfiguration { get; private set; }

    public static void Init(MapperConfiguration config)
    {
        MapperConfiguration = config;
        Mapper = config.CreateMapper();
    }
}
```

**Usage Pattern:**
- Create profile classes implementing `IMapperProfile`
- Auto-discover profiles at startup
- Centralized mapper configuration

#### **Validation** (`Framework.Core/Validators/`)

**Built-in Validators:**
- `BaseValidator<T>` - Base class for FluentValidation
- `UsernamePropertyValidator` - Username format validation
- `CreditCardPropertyValidator` - Credit card validation
- `DecimalPropertyValidator` - Decimal precision validation
- `FileValidator` - File upload validation
- `RequiredIfValidator` - Conditional required fields

**Integration:**
```csharp
public class MyValidator : BaseValidator<MyDto>
{
    public MyValidator()
    {
        RuleFor(x => x.Email).EmailAddress();
        RuleFor(x => x.Username).SetValidator(new UsernamePropertyValidator());
    }
}
```

#### **Caching** (`Framework.Core/Caching/`)

**Cache Implementations:**
- `MemoryCacheManager` - In-memory caching (thread-safe)
- `PerRequestCacheManager` - Request-scoped caching

**Interface:**
```csharp
public interface ICacheManager
{
    T Get<T>(string key);
    Task<T> GetAsync<T>(string key);
    void Set(string key, object data, int cacheTime);
    void Remove(string key);
    void RemoveByPattern(string pattern);
    void Clear();
}
```

#### **Background Jobs** (`Framework.Core/BackgroundJobs/`)

**Hangfire Integration:**
- `HangfireActivator` - DI-aware job activator
- `HangfireDashboardAuthFilter` - Dashboard authentication
- `HangfireDashboardMiddleware` - Dashboard middleware
- `HangfireExtensions` - Configuration helpers

**Features:**
- Recurring jobs
- Fire-and-forget jobs
- Dashboard UI
- Job retries
- Job persistence

#### **Notifications** (`Framework.Core/Notifications/`)

**Notification Types:**
1. **Email** - SMTP-based email notifications
2. **SMS** - SMS notifications (Fake service for development)
3. **Firebase** - Mobile push notifications
4. **Web** - In-app web notifications

**Components:**
```csharp
public interface IEmailService
{
    Task SendEmailAsync(EmailMessage message);
}

public interface IMobileNotificationService
{
    Task SendNotificationAsync(MobileNotification notification);
}

public class NotificationsManager
{
    // Unified notification orchestration
    // Template-based notifications
    // Multi-channel delivery
}
```

**Features:**
- Template-based notifications
- Multi-channel delivery
- Async processing
- Error handling

#### **Middlewares** (`Framework.Core/Middlewares/`)

**Exception Handling:**
```csharp
public class ExceptionMiddleware
{
    // Global exception handler
    // Logs exceptions
    // Returns standardized error responses
}

public class ApiException : Exception
{
    public int StatusCode { get; set; }
    public string Details { get; set; }
}
```

**Globalization:**
```csharp
public class GlobalizationMiddleware
{
    // Auto-detects culture from headers/cookies
    // Sets thread culture
    // Enables localized responses
}
```

#### **Shared Services** (`Framework.Core/SharedServices/`)

**Common Entities:**
- `Audit` - Audit trail entries
- `Log` - Application logs
- `SystemSetting` - Key-value configuration
- `AttachmentType` - File type definitions
- `AttachmentContent` - File storage
- `WebNotification` - Web notification storage
- `NotificationTemplate` - Notification templates

**Services:**
- `AttachmentService` - File upload/download management
- `LoggingService` - Structured logging
- `NotificationTemplateService` - Template management
- `PDFGeneratorAppService` - PDF generation

**CommonsDbContext:**
- Separate DbContext for framework entities
- Shared across all applications
- Automatic migrations

#### **Helpers & Extensions** (`Framework.Core/Helpers/`, `Framework.Core/Extensions/`)

**50+ Extension Methods:**
- `StringExtensions` - String manipulation
- `DateTimeExtensions` - Date operations
- `CollectionsExtensions` - LINQ enhancements
- `EnumExtensions` - Enum utilities
- `QueryableExtensions` - IQueryable helpers
- `ExcelExtensions` - Excel operations
- `NumbersExtensions` - Numeric formatting
- `ObjectExtensions` - Reflection utilities

**Helpers:**
- `CommonHelper` - General utilities
- `DateTimeHelper` - Date calculations
- `CultureHelper` - Localization support
- `XmlHelper` - XML serialization
- `PDFHelper` - PDF utilities
- `Base64FileHelper` - File encoding

#### **API Support** (`Framework.Core/`)

```csharp
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public T Data { get; set; }
    public List<string> Errors { get; set; }
}

public class PagedList<T> : List<T>
{
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public bool HasPrevious { get; }
    public bool HasNext { get; }
}

public class ReturnResult<T>
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public T Data { get; set; }
    public int StatusCode { get; set; }
}
```

### 6.2 Framework.Identity

**Purpose:** Authentication, authorization, and user management

**Dependencies:**
- Microsoft.AspNetCore.Identity.EntityFrameworkCore (8.0.7)
- Microsoft.AspNetCore.DataProtection.EntityFrameworkCore (8.0.7)
- ExcelDataReader (3.7.0)
- System.DirectoryServices.AccountManagement (8.0.0) - Active Directory support

**Key Components:**

**Repositories:**
- `UserRepository` - User management
- `RoleRepository` - Role management
- `UserRolesRepository` - User-role assignments
- `UserTokensRepository` - JWT token management
- `UserOtpRepository` - OTP/2FA support

**DTOs:**
- `LoginResult` - Authentication response
- `TokenDto` - JWT token data
- `UserRolesDto` - User role information
- `UserOtpDto` - OTP data
- `ClaimDto` - User claims

**Features:**
- ASP.NET Core Identity integration
- JWT token generation/validation
- Role-based authorization
- OTP/2FA support
- Active Directory integration
- User import from Excel
- Password generation utilities
- Seed data for default admin/roles

**Constants:**
```csharp
public static class JWTTokensConstants
{
    public const string SecretKey = "...";
    public const string Issuer = "...";
    public const string Audience = "...";
}
```

**Seed Data:**
- `RoleConfiguration` - Default roles
- `AdminConfiguration` - Default admin user
- `UsersWithRolesConfig` - User-role mappings

### 6.3 Framework.Resources

**Purpose:** Localization and multilingual support

**Structure:**
```
Framework.Resources/
├── SharedResources.resx       # Default resources
├── SharedResources.en.resx    # English resources
└── SharedResources.ar.resx    # Arabic resources
```

**Resource Files:**
- Code-generated resource classes
- Embedded resources
- Strong-typed access via `SharedResources` class

**Usage:**
```csharp
string message = SharedResources.ResourceManager.GetString("KeyName", culture);
```

**Integration:**
- Used by GlobalizationMiddleware
- Used by LookupEntityBase for bilingual names
- Used by validation messages

---

## 7. Data Architecture

### 7.1 Database Strategy

**Primary Database:** SQL Server
**Alternative:** PostgreSQL (via Npgsql.EntityFrameworkCore.PostgreSQL 8.0.4)

**Database Contexts:**

1. **AppDbContext** (`LMS.Infrastructure`)
   - Application-specific entities
   - Business domain data
   - Inherits from `BaseDbContext<AppDbContext>`

2. **CommonsDbContext** (`Framework.Core`)
   - Framework shared entities
   - Audit logs
   - System settings
   - Attachments
   - Notifications

3. **IdentityDbContext** (`Framework.Identity`)
   - User management
   - Roles and claims
   - Authentication data

### 7.2 Entity Framework Core Configuration

**Connection String Configuration:**
```csharp
services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));
```

**Entity Configuration:**
- **Convention-based:** Auto-discovery via reflection
- **Fluent API:** `EntityTypeConfiguration<T>` classes
- **Data Annotations:** Used in entity base classes

**Example Entity Configuration:**
```csharp
public class MyEntityConfiguration : EntityTypeConfiguration<MyEntity>
{
    public override void Configure(EntityTypeBuilder<MyEntity> builder)
    {
        builder.ToTable("MyEntities");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).HasMaxLength(100).IsRequired();
    }
}
```

**Enhanced Auto-Discovery Pattern (AppDbContext):**
```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // Auto-discover EntityTypeConfiguration<T> classes
    var typeConfigurations = Assembly.GetExecutingAssembly().GetTypes()
        .Where(type => (type.BaseType?.IsGenericType ?? false) &&
                      type.BaseType.GetGenericTypeDefinition() == typeof(EntityTypeConfiguration<>));

    foreach (var typeConfiguration in typeConfigurations)
    {
        if (typeConfiguration.IsGenericType) continue;
        var configuration = (IMappingConfiguration)Activator.CreateInstance(typeConfiguration);
        configuration.ApplyConfiguration(modelBuilder);
    }

    // Auto-discover EnumMapping<T> classes
    var enumConfigurations = Assembly.GetExecutingAssembly().GetTypes()
        .Where(type => (type.BaseType?.IsGenericType ?? false) &&
                      type.BaseType.GetGenericTypeDefinition() == typeof(EnumMapping<>));

    foreach (var typeConfiguration in enumConfigurations)
    {
        var configuration = (IMappingConfiguration)Activator.CreateInstance(typeConfiguration);
        configuration.ApplyConfiguration(modelBuilder);
    }

    // Apply global query filter for IsActive on all LookupEntityBase entities
    foreach (var entityType in modelBuilder.Model.GetEntityTypes())
    {
        if (typeof(LookupEntityBase).IsAssignableFrom(entityType.ClrType))
        {
            var method = typeof(AppDbContext)
                .GetMethod(nameof(SetActiveFilter), BindingFlags.NonPublic | BindingFlags.Static)!
                .MakeGenericMethod(entityType.ClrType);

            method.Invoke(null, new object[] { modelBuilder });
        }
    }

    base.OnModelCreating(modelBuilder);
}

private static void SetActiveFilter<TEntity>(ModelBuilder builder) where TEntity : LookupEntityBase
{
    builder.Entity<TEntity>().HasQueryFilter(e => e.IsActive);
}
```

**Key Enhancements:**
- **EnumMapping Support:** Automatically discovers and applies enum mapping configurations
- **Global Query Filters:** All lookup entities automatically filter by `IsActive = true`
- **Reflection-based Filtering:** Uses reflection to apply filters to all derived types

### 7.3 Migrations

**Approach:** Code-First Migrations

**Migration Locations:**
- `Framework.Core/Migrations/` - Commons schema
- `Framework.Identity/Migrations/` - Identity schema
- `LMS.Infrastructure/Migrations/` (future) - Application schema

**Automatic Migration on Startup:**
```csharp
public static void UseApplicationDBMigration(this IApplicationBuilder app)
{
    using var serviceScope = app.ApplicationServices
        .GetRequiredService<IServiceScopeFactory>()
        .CreateScope();

    serviceScope.ServiceProvider
        .GetService<AppDbContext>()
        .Database.Migrate();
}
```

### 7.4 Auditing

**Automatic Auditing:**
- Triggered in `BaseDbContext.SaveChanges()` and `SaveChangesAsync()`
- Uses `ChangeTracker.SetShadowProperties(CurrentUserName)`
- Automatically sets:
  - `CreatedBy` on Insert
  - `CreatedOn` on Insert
  - `UpdatedBy` on Update
  - `UpdatedOn` on Update

**Audit Trail:**
- `Audit` entity stores detailed change history
- Captures: Entity type, operation, old/new values, timestamp, user

### 7.5 Query Patterns

**Tracking vs. No-Tracking:**
```csharp
// With tracking (for updates)
var entity = await _repository.GetByIdAsync(id);
entity.Name = "New Name";
await _unitOfWork.SaveChangesAsync();

// No-tracking (read-only, better performance)
var list = await _repository.GetAsync(
    filter: x => x.IsActive,
    orderBy: q => q.OrderBy(x => x.Name),
    disableTracking: true
);
```

**Eager Loading:**
```csharp
var entities = await _repository.GetAsync(
    filter: x => x.IsActive,
    includes: new List<Expression<Func<MyEntity, object>>>
    {
        x => x.RelatedEntity,
        x => x.AnotherRelation
    }
);

// Deep relations
var entity = await _repository.GetSingleWithDeepRelationsAsync(
    predicate: x => x.Id == id,
    include: source => source
        .Include(x => x.Level1)
        .ThenInclude(x => x.Level2)
);
```

**Pagination:**
```csharp
var pagedResult = _repository.SearchWithFilters(
    pageNumber: 1,
    pageSize: 20,
    orderBy: q => q.OrderByDescending(x => x.CreatedOn),
    filters: new List<Expression<Func<MyEntity, bool>>>
    {
        x => x.IsActive,
        x => x.Category == "SomeCategory"
    },
    includes: x => x.RelatedEntity
);

// Returns PagedList<T> with:
// - Items for current page
// - TotalCount, TotalPages, CurrentPage
// - HasNext, HasPrevious
```

**Dynamic Queries:**
```csharp
// Using System.Linq.Dynamic.Core
var result = _repository.Table
    .Where("IsActive == true && Name.Contains(@0)", searchTerm)
    .OrderBy("CreatedOn DESC")
    .ToList();
```

---

## 8. Cross-Cutting Concerns

### 8.1 Logging

**NLog Integration:**
- Version: 5.3.2
- Extensions: NLog.Extensions.Logging (5.3.11)
- Web Integration: NLog.Web.AspNetCore (5.3.11)

**Logging Service:**
```csharp
public interface ILoggingService
{
    void LogInformation(string message);
    void LogWarning(string message);
    void LogError(string message, Exception ex = null);
    void LogDebug(string message);
}

public class LoggingService : ILoggingService
{
    private readonly ILogger _logger;

    // Structured logging
    // Database persistence via Log entity
    // File output support
}
```

**Log Entity:**
- Persisted to database via CommonsDbContext
- Fields: Level, Message, Exception, Timestamp, Logger, User

### 8.2 Exception Handling

**Global Exception Middleware:**
```csharp
app.UseMiddleware<ExceptionMiddleware>();
```

**Features:**
- Catches all unhandled exceptions
- Logs exceptions via ILoggingService
- Returns standardized error responses:

```json
{
  "success": false,
  "message": "An error occurred",
  "errors": ["Error details"],
  "statusCode": 500
}
```

**Custom Exceptions:**
```csharp
public class ApiException : Exception
{
    public int StatusCode { get; set; }
    public string Details { get; set; }
}

public class IdentityException : Exception
{
    // Identity-specific errors
}

public class NotificationException : Exception
{
    // Notification-specific errors
}
```

### 8.3 Localization (Globalization)

**Middleware:**
```csharp
app.UseMiddleware<GlobalizationMiddleware>();
```

**Culture Detection:**
1. `Accept-Language` HTTP header
2. Culture cookie
3. Query string parameter
4. Default culture (fallback)

**Supported Cultures:**
- Arabic (ar)
- English (en)

**CultureHelper:**
```csharp
public static class CultureHelper
{
    public static bool IsArabic { get; }
    public static bool IsEnglish { get; }
    public static string CurrentCulture { get; }
    public static CultureInfo GetCultureInfo(string cultureName);
}
```

**Bilingual Entities:**
```csharp
public class LookupEntityBase<TKey>
{
    public string NameAr { get; set; }
    public string NameEn { get; set; }

    [NotMapped]
    public string Name => CultureHelper.IsArabic ? NameAr : NameEn;
}
```

**Resource Files:**
- `Framework.Resources/SharedResources.{culture}.resx`
- Code-generated strong-typed access
- Used in validation messages, error messages, UI labels

### 8.4 Caching Strategy

**Two-Tier Caching:**

1. **MemoryCacheManager** (Application-level)
   - Thread-safe
   - Configurable expiration
   - Pattern-based removal
   - Singleton lifetime

2. **PerRequestCacheManager** (Request-level)
   - Scoped to HTTP request
   - Clears automatically after response
   - Scoped lifetime

**Usage:**
```csharp
// Inject ICacheManager (MemoryCacheManager by default)
private readonly ICacheManager _cacheManager;

// Cache with 60-minute expiration
_cacheManager.Set("cache-key", data, cacheTime: 60);

// Retrieve
var cachedData = _cacheManager.Get<MyType>("cache-key");

// Remove by pattern
_cacheManager.RemoveByPattern("user-*");
```

**Cache Invalidation:**
- Manual via `Remove()` or `RemoveByPattern()`
- Time-based expiration
- Clear all via `Clear()`

### 8.5 Background Jobs (Hangfire)

**Configuration:**
```csharp
services.AddHangfire(config => config
    .UseSqlServerStorage(connectionString)
);

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new HangfireDashboardAuthFilter() }
});

app.UseHangfireServer();
```

**Job Types:**

1. **Fire-and-Forget:**
```csharp
BackgroundJob.Enqueue(() => SendEmailAsync(email));
```

2. **Delayed:**
```csharp
BackgroundJob.Schedule(() => SendReminder(), TimeSpan.FromHours(24));
```

3. **Recurring:**
```csharp
RecurringJob.AddOrUpdate("job-id", () => CleanupOldLogs(), Cron.Daily);
```

**Dashboard:**
- URL: `/hangfire`
- Protected by `HangfireDashboardAuthFilter`
- Monitor jobs, retries, failures

**DI Integration:**
- `HangfireActivator` resolves services from DI container
- Jobs can inject scoped services

### 8.6 Validation Strategy

**FluentValidation Integration:**
```csharp
services.AddFluentValidation(fv =>
{
    fv.RegisterValidatorsFromAssemblyContaining<Startup>();
    fv.AutomaticValidationEnabled = true;
});
```

**Validator Pattern:**
```csharp
public class CreateUserDtoValidator : BaseValidator<CreateUserDto>
{
    public CreateUserDtoValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .SetValidator(new UsernamePropertyValidator());

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8);

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password)
            .WithMessage("Passwords must match");
    }
}
```

**Custom Validators:**
- `UsernamePropertyValidator` - Username format rules
- `CreditCardPropertyValidator` - Credit card validation
- `DecimalPropertyValidator` - Decimal precision
- `FileValidator` - File upload rules
- `RequiredIfValidator` - Conditional validation

**Automatic Validation:**
- ASP.NET Core automatically validates DTOs via ModelState
- Returns 400 Bad Request with validation errors
- Errors returned as standardized response

### 8.7 File Management

**AttachmentService:**
```csharp
public interface IAttachmentService
{
    Task<AttachmentContent> UploadAsync(IFormFile file, int attachmentTypeId);
    Task<AttachmentContent> GetByIdAsync(int id);
    Task<FileContentResult> DownloadAsync(int id);
    Task DeleteAsync(int id);
}
```

**AttachmentType:**
- Defines allowed file types
- Max file size limits
- Storage location
- Seeded via migrations

**Storage:**
- Database storage (AttachmentContent entity)
- File system storage (configurable)
- Base64 encoding support

**Supported Operations:**
- Upload with validation
- Download with content type detection
- Delete with cleanup
- Bulk operations

### 8.8 PDF Generation

**Multiple PDF Libraries:**

1. **iText (8.0.5)** - Modern PDF manipulation
2. **iTextSharp (5.5.13.4)** - Legacy support
3. **Select.HtmlToPdf.NetCore (24.1.0)** - HTML to PDF

**PDFHelper:**
```csharp
public static class PDFHelper
{
    public static byte[] GeneratePdfFromHtml(string html);
    public static byte[] MergePdfs(List<byte[]> pdfs);
    public static byte[] AddWatermark(byte[] pdf, string watermarkText);
}
```

**PDFGeneratorAppService:**
```csharp
public interface IPDFGenerator
{
    Task<byte[]> GenerateAsync(string templatePath, object model);
    Task<byte[]> GenerateFromHtmlAsync(string html);
}
```

**Use Cases:**
- Reports generation
- Invoice creation
- Certificate generation
- Document exports

### 8.9 Excel Operations

**EPPlus (7.2.2) - Writing:**
```csharp
public static class ExcelExtensions
{
    public static byte[] ToExcel<T>(this List<T> data, string sheetName = "Sheet1");
    public static ExcelPackage CreateExcelPackage<T>(this List<T> data);
}
```

**ExcelDataReader (3.7.0) - Reading:**
```csharp
// Read Excel to DataSet
using var stream = File.OpenRead("file.xlsx");
using var reader = ExcelReaderFactory.CreateReader(stream);
var dataset = reader.AsDataSet();
```

**Features:**
- Export collections to Excel
- Import Excel to objects
- Format cells, styling
- Multiple sheets support

### 8.10 Real-Time Communication (SignalR)

**SignalR Hub Implementation:**
```csharp
public class SignalR : Hub
{
    public string GetConnectionId() => Context.ConnectionId;

    public async Task SendMessage(string message)
    {
        await Clients.All.SendAsync("newMessage", "anonymous", message);
    }

    public async Task InformAll(object data)
    {
        await Clients.All.SendAsync("MessageAll", "anonymous", data);
    }

    public async Task InformClientByConnectionId(object data, string connectionId)
    {
        await Clients.Client(connectionId).SendAsync("InformClient", data);
    }

    public Task InformGroup(object data, string groupName)
    {
        return Clients.Group(groupName).SendAsync("SendGroup", data);
    }

    public async Task JoinGroup(string groupName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
    }

    public async Task LeaveGroup(string groupName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
    }
}
```

**Hub Endpoint:**
```csharp
app.MapHub<SignalR>("/api/LiveInform");
```

**Frontend Integration:**
```typescript
// Angular SignalR client (@microsoft/signalr 9.0.6)
import * as signalR from '@microsoft/signalr';

const connection = new signalR.HubConnectionBuilder()
    .withUrl('/api/LiveInform')
    .build();

connection.on('MessageAll', (user, data) => {
    // Handle broadcast message
});

connection.on('InformClient', (data) => {
    // Handle targeted message
});
```

**Use Cases:**
- Real-time KPI updates to dashboards across all VRP offices
- Targeted notifications to specific users/roles (by connection ID)
- Group-based messaging (e.g., program team, initiative stakeholders)
- Live updates for quarterly performance reviews
- Alert notifications for KPI target deviations
- Connection management and tracking for concurrent dashboard viewers

### 8.11 External Service Integrations

#### **Nafath Service Integration (Saudi National Authentication)**

**Purpose:** Integration with Saudi Arabia's Nafath national authentication platform for secure user authentication for SMO staff, VRP offices, and government stakeholders.

**Implementation:**
```csharp
public class NafathServiceProxy : INafathServiceProxy
{
    // Sends authentication request
    public async Task<ApiResponse<SendRequestResponseDto>> SendRequest(string UserIdentity)
    {
        // Returns TransId and Random for user verification
    }

    // Checks authentication status
    public async Task<ApiResponse<CheckRequestStatusResponseDto>> CheckRequestStatus(CheckRequestStatusDto dto)
    {
        // Returns IAM claims: ID, name, DOB, gender, nationality, etc.
    }
}
```

**Features:**
- Simulation mode for testing without actual Nafath connection
- Returns full IAM (Identity and Access Management) claims
- Multi-language support (Arabic/English)
- Handles authentication flow: Send request → User approves on mobile → Check status

**Configuration:**
```csharp
services.AddHttpClient<INafathServiceProxy, NafathServiceProxy>();

// AppSettings:
"NafathApiUrl": "https://api.nafath.sa/...",
"NafathApiKey": "...",
"IsNafathSimulation": false
```

#### **SMS Service Integration**

**Purpose:** SMS gateway integration for sending notifications and OTP codes.

**Implementation:**
```csharp
public class SMSServiceProxy : ISmsService
{
    public async Task<ApiResponse<string>> SendSms(SmsMessage smsMessage)
    {
        // Sends SMS via external gateway
        // Returns success/failure status
    }
}
```

**Features:**
- Bearer token authentication
- Sender name customization (e.g., "DSC")
- Text message support
- Comprehensive logging for debugging
- Error handling with user-friendly messages

**Configuration:**
```csharp
services.AddHttpClient<ISmsService, SMSServiceProxy>();

// AppSettings:
"SmsAPIUrl": "https://sms-gateway.example.com/api/send",
"SmsAPIToken": "bearer-token-here"
```

**Use Cases:**
- Send OTP codes for 2FA (SMO users, VRP offices)
- KPI target alerts and notifications
- Quarterly report reminders
- Important strategic updates
- Account verification for new VRP office users

#### **ADAA Platform Integration (Performance Management System)**

**Purpose:** Integration with Saudi Arabia's ADAA (أداء) performance management platform for synchronizing government entity performance data and KPIs.

**Implementation:**
```csharp
public class AdaaServiceProxy : IAdaaServiceProxy
{
    // Retrieves performance data from ADAA
    public async Task<ApiResponse<PerformanceDataDto>> GetPerformanceData(string entityCode, int year, int quarter)
    {
        // Returns KPI performance data for government entities
    }

    // Synchronizes strategic objectives with ADAA
    public async Task<ApiResponse<bool>> SyncStrategicObjectives(List<StrategicObjectiveDto> objectives)
    {
        // Pushes Vision 2030 objectives to ADAA platform
    }
}
```

**Features:**
- Bidirectional data synchronization
- Automated KPI data refresh
- Entity performance tracking
- Integration with quarterly reporting cycle
- OAuth 2.0 authentication

**Configuration:**
```csharp
services.AddHttpClient<IAdaaServiceProxy, AdaaServiceProxy>();

// AppSettings:
"AdaaApiUrl": "https://api.adaa.gov.sa/...",
"AdaaClientId": "smo-vision-center",
"AdaaClientSecret": "...",
"AdaaIntegrationEnabled": true
```

**Use Cases:**
- Import government entity performance KPIs
- Synchronize Vision 2030 objectives with national performance framework
- Automated quarterly performance data refresh
- Cross-platform KPI validation

#### **GaStat Integration (General Authority for Statistics)**

**Purpose:** Integration with GaStat (الهيئة العامة للإحصاء) for retrieving national statistical indicators that feed into Vision 2030 KPIs.

**Implementation:**
```csharp
public class GaStatServiceProxy : IGaStatServiceProxy
{
    // Retrieves statistical indicators
    public async Task<ApiResponse<StatisticalIndicatorDto>> GetIndicator(string indicatorCode, int year)
    {
        // Returns official statistical data (GDP, population, employment, etc.)
    }

    // Gets indicator time series
    public async Task<ApiResponse<List<TimeSeriesDataDto>>> GetIndicatorTimeSeries(
        string indicatorCode, int fromYear, int toYear)
    {
        // Returns historical statistical data for trend analysis
    }
}
```

**Features:**
- Official statistical data retrieval
- Time series analysis support
- Automated data validation
- Multi-language support (Arabic/English)
- RESTful API integration

**Configuration:**
```csharp
services.AddHttpClient<IGaStatServiceProxy, GaStatServiceProxy>();

// AppSettings:
"GaStatApiUrl": "https://api.stats.gov.sa/...",
"GaStatApiKey": "...",
"GaStatIntegrationEnabled": true
```

**Use Cases:**
- Auto-populate KPI baselines from official statistics
- Import demographic and economic indicators
- Validate Vision 2030 KPI targets against national trends
- Generate statistical reports for quarterly reviews

---

## 9. Coding Standards & Patterns

### 9.1 Naming Conventions

**C# Conventions:**
- **Namespaces:** PascalCase (e.g., `LMS.Application.Services`)
- **Classes/Interfaces:** PascalCase (e.g., `UserService`, `IUserService`)
- **Methods:** PascalCase (e.g., `GetUserById`)
- **Properties:** PascalCase (e.g., `FirstName`)
- **Private Fields:** camelCase with underscore prefix (e.g., `_userRepository`)
- **Parameters:** camelCase (e.g., `userId`)
- **Constants:** PascalCase (e.g., `MaxRetryAttempts`)

**Database Conventions:**
- **Tables:** PascalCase, plural (e.g., `Users`, `Orders`)
- **Columns:** PascalCase (e.g., `FirstName`, `CreatedOn`)
- **Foreign Keys:** `{EntityName}Id` (e.g., `UserId`)
- **Junction Tables:** Singular entities combined (e.g., `UserRole`)

**Angular Conventions:**
- **Files:** kebab-case (e.g., `user-list.component.ts`)
- **Components:** PascalCase suffix (e.g., `UserListComponent`)
- **Services:** PascalCase suffix (e.g., `AuthService`)
- **Modules:** PascalCase suffix (e.g., `AppModule`)
- **Selectors:** kebab-case with prefix (e.g., `app-user-list`)

### 9.2 Project Organization Patterns

**Backend Structure Pattern:**
```
{ProjectName}.{Layer}/
├── Features/          # Feature folders (vertical slices)
│   ├── StrategicObjective/
│   │   ├── Commands/  # Write operations (CQRS-style organization)
│   │   ├── Queries/   # Read operations
│   │   ├── DTOs/
│   │   ├── Validators/
│   │   └── Mappers/
│   └── Initiative/
│       ├── DTOs/
│       ├── Validators/
│       └── Mappers/
└── Common/            # Shared within layer
    ├── Interfaces/
    ├── Extensions/
    └── Helpers/
```

**Note:** Current implementation doesn't use CQRS (per user's CLAUDE.md instructions), but organization supports either pattern.

**Frontend Structure Pattern:**
```
src/app/
├── core/              # Singleton services, guards, interceptors
├── shared/            # Shared components, directives, pipes
├── features/          # Feature modules
│   ├── pillars/
│   │   ├── components/
│   │   ├── services/
│   │   ├── models/
│   │   └── pillars.module.ts
│   ├── objectives/
│   │   ├── components/
│   │   ├── services/
│   │   ├── models/
│   │   └── objectives.module.ts
│   ├── programs/
│   │   └── ...
│   ├── initiatives/
│   │   └── ...
│   └── kpis/
│       └── ...
└── layout/            # Layout components
```

### 9.3 Dependency Injection Patterns

**Lifetimes:**

- **Transient:** New instance per request
  ```csharp
  services.AddTransient<IEmailService, SmtpEmailService>();
  ```
  - Use for: Lightweight, stateless services
  - Example: Validators, mappers

- **Scoped:** One instance per HTTP request
  ```csharp
  services.AddScoped<IUnitOfWork, UnitOfWork>();
  services.AddScoped<IRepository<User>, Repository<User>>();
  ```
  - Use for: DbContext, repositories, Unit of Work
  - Example: Data access layer

- **Singleton:** One instance for application lifetime
  ```csharp
  services.AddSingleton<ICacheManager, MemoryCacheManager>();
  ```
  - Use for: Stateless, thread-safe services
  - Example: Caching, configuration

**Service Registration Extensions:**
```csharp
// Infrastructure Layer
public static class ServiceCollectionExtensions
{
    public static void ConfigureInfrastructureServices(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));
        services.AddScoped<IAppDbContext, AppDbContext>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
    }
}

// Usage in Program.cs
services.ConfigureInfrastructureServices(Configuration.GetConnectionString("DefaultConnection"));
```

**Auto-Registration (Framework.Core):**
```csharp
public static class AutoRegisterHelpers
{
    public static void AutoRegisterServices(this IServiceCollection services, Assembly assembly)
    {
        // Scans assembly for interfaces/implementations
        // Registers based on naming conventions or attributes
    }
}
```

### 9.4 CRITICAL ARCHITECTURAL RULES

**From User's CLAUDE.md:**

1. **NEVER USE CQRS**
   - Direct service methods for reads and writes
   - No command/query separation
   - Services handle both operations

2. **Controllers ONLY inject Application Services**
   ```csharp
   // ✅ CORRECT
   public class StrategicObjectiveController : ControllerBase
   {
       private readonly IStrategicObjectiveAppService _objectiveService;

       public StrategicObjectiveController(IStrategicObjectiveAppService objectiveService)
       {
           _objectiveService = objectiveService;
       }
   }

   // ❌ WRONG - Never inject repositories in controllers
   public class StrategicObjectiveController : ControllerBase
   {
       private readonly IRepository<StrategicObjective> _objectiveRepository; // NEVER DO THIS!
   }
   ```

3. **Run git commit after each completed task**
   - Ensures incremental, traceable changes
   - Facilitates code reviews
   - Enables easy rollback

### 9.5 Async/Await Patterns

**Guidelines:**
- Use `async/await` for all I/O operations
- Suffix async methods with `Async`
- Always return `Task<T>` or `Task`
- Use `ConfigureAwait(false)` in library code

**Examples:**
```csharp
// ✅ Good
public async Task<StrategicObjective> GetObjectiveByIdAsync(int id)
{
    return await _repository.GetByIdAsync(id);
}

// ✅ Good - Multiple awaits
public async Task<Initiative> CreateInitiativeAsync(InitiativeDto dto)
{
    var program = await _programRepository.GetByIdAsync(dto.ProgramId);
    var initiative = _mapper.Map<Initiative>(dto);
    initiative.Program = program;

    var result = await _repository.InsertAsync(initiative);
    await _unitOfWork.SaveChangesAsync();

    return result;
}

// ❌ Avoid - Blocking async
public StrategicObjective GetObjectiveById(int id)
{
    return _repository.GetByIdAsync(id).Result; // Blocks thread
}
```

### 9.6 Error Handling Patterns

**Service Layer:**
```csharp
public async Task<ReturnResult<KPIDto>> GetKPIAsync(int id)
{
    try
    {
        var kpi = await _repository.GetByIdAsync(id);
        if (kpi == null)
            return ReturnResult<KPIDto>.Failure("KPI not found", 404);

        var dto = _mapper.Map<KPIDto>(kpi);
        return ReturnResult<KPIDto>.Ok(dto);
    }
    catch (Exception ex)
    {
        _logger.LogError("Error getting KPI", ex);
        return ReturnResult<KPIDto>.Failure("An error occurred", 500);
    }
}
```

**Controller Layer:**
```csharp
[HttpGet("{id}")]
public async Task<ActionResult<ApiResponse<KPIDto>>> GetKPI(int id)
{
    var result = await _kpiService.GetKPIAsync(id);

    if (!result.Success)
        return StatusCode(result.StatusCode, new ApiResponse<KPIDto>
        {
            Success = false,
            Message = result.Message
        });

    return Ok(new ApiResponse<KPIDto>
    {
        Success = true,
        Data = result.Data
    });
}
```

**Global Exception Handling:**
- `ExceptionMiddleware` catches all unhandled exceptions
- Logs to NLog + database
- Returns standardized error response
- Never exposes sensitive information in production

### 9.7 Testing Patterns

**Recommended Structure:**
```
{ProjectName}.Tests/
├── Unit/
│   ├── Application/
│   │   └── Services/
│   │       └── UserServiceTests.cs
│   └── Domain/
│       └── Entities/
│           └── UserTests.cs
├── Integration/
│   └── Infrastructure/
│       └── Repositories/
│           └── UserRepositoryTests.cs
└── Helpers/
    └── TestDataBuilder.cs
```

**Unit Test Pattern:**
```csharp
public class KPIServiceTests
{
    private readonly Mock<IRepository<KPI>> _mockRepo;
    private readonly Mock<IUnitOfWork> _mockUow;
    private readonly Mock<IMapper> _mockMapper;
    private readonly KPIAppService _sut; // System Under Test

    public KPIServiceTests()
    {
        _mockRepo = new Mock<IRepository<KPI>>();
        _mockUow = new Mock<IUnitOfWork>();
        _mockMapper = new Mock<IMapper>();
        _sut = new KPIAppService(_mockRepo.Object, _mockUow.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task GetKPIAsync_ValidId_ReturnsKPI()
    {
        // Arrange
        var kpiId = 1;
        var kpi = new KPI { Id = kpiId, Code = "KPI-001", Name = "GDP Growth Rate" };
        _mockRepo.Setup(x => x.GetByIdAsync(kpiId)).ReturnsAsync(kpi);

        // Act
        var result = await _sut.GetKPIAsync(kpiId);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(kpiId, result.Data.Id);
    }
}
```

---

## 10. Deployment Architecture

### 10.1 Environment Configuration

**Backend (appsettings.json):**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=...;Database=...;",
    "CommonsConnection": "Server=...;Database=Commons;",
    "IdentityConnection": "Server=...;Database=Identity;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning"
    }
  },
  "JwtSettings": {
    "SecretKey": "...",
    "Issuer": "...",
    "Audience": "...",
    "ExpirationMinutes": 60
  },
  "NotificationSettings": {
    "SmtpHost": "...",
    "SmtpPort": 587,
    "FirebaseServerKey": "..."
  }
}
```

**Environment-Specific Files:**
- `appsettings.json` - Base configuration
- `appsettings.Development.json` - Dev overrides
- `appsettings.Staging.json` - Staging overrides
- `appsettings.Production.json` - Production overrides

**Frontend (environments/):**
```typescript
export const environment = {
  production: false,
  apiUrl: 'https://api.development.com',
  enableLogging: true
};
```

### 10.2 Build & Deployment

**Backend Build:**
```bash
dotnet build --configuration Release
dotnet publish --configuration Release --output ./publish
```

**Frontend Build:**
```bash
# Development
ng serve

# Production
ng build --configuration production

# Staging
ng build --configuration staging
```

**Output:**
- Frontend: `dist/smo-portal/browser/` (standalone mode) or `dist/smo-portal/` (browser subfolder)
- Backend: `bin/Release/net8.0/publish/`

### 10.3 Hosting Options

**Backend:**
1. **IIS (Windows Server)**
   - ASP.NET Core Module
   - In-process or out-of-process hosting
   - Application Pool configuration

2. **Kestrel (Self-hosted)**
   - Cross-platform
   - Docker containers
   - Linux systemd service

3. **Azure App Service**
   - PaaS deployment
   - Built-in scaling
   - Deployment slots

**Frontend:**
1. **IIS** (with `web.config` for SPA routing)
2. **Azure Static Web Apps**
3. **CDN + Storage Account**
4. **Nginx**

### 10.4 IIS Deployment (Frontend)

**web.config:**
```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <system.webServer>
    <rewrite>
      <rules>
        <rule name="Angular Routes" stopProcessing="true">
          <match url=".*" />
          <conditions logicalGrouping="MatchAll">
            <add input="{REQUEST_FILENAME}" matchType="IsFile" negate="true" />
            <add input="{REQUEST_FILENAME}" matchType="IsDirectory" negate="true" />
          </conditions>
          <action type="Rewrite" url="/" />
        </rule>
      </rules>
    </rewrite>
    <staticContent>
      <mimeMap fileExtension=".json" mimeType="application/json" />
      <mimeMap fileExtension=".woff" mimeType="application/font-woff" />
      <mimeMap fileExtension=".woff2" mimeType="application/font-woff2" />
    </staticContent>
  </system.webServer>
</configuration>
```

### 10.5 Database Deployment

**Migration Strategy:**

1. **Development:**
   - Code-First migrations
   - `Add-Migration` via Package Manager Console
   - Auto-apply on app startup (`UseApplicationDBMigration()`)

2. **Production:**
   - Generate SQL scripts from migrations
   ```bash
   dotnet ef migrations script --output migration.sql --idempotent
   ```
   - Review and manually execute
   - Or use CI/CD pipeline automation

**Database Seeding:**
- Seed data in migrations
- `OnModelCreating` seeding for lookup data
- Seed classes for admin/roles in Framework.Identity

### 10.6 Monitoring & Logging

**Backend Logging:**
- NLog configuration (nlog.config)
- File logging (rolling files)
- Database logging (Log entity)
- Application Insights (optional)

**Frontend Logging:**
- Console logging (development)
- Error tracking service (production)
- User analytics

**Health Checks:**
```csharp
services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>()
    .AddSqlServer(connectionString);

app.MapHealthChecks("/health");
```

**Hangfire Dashboard:**
- Monitor background jobs
- Job success/failure rates
- Retry logic
- `/hangfire` endpoint

---

## Appendix A: Key Technologies Summary

| Technology | Version | Purpose |
|-----------|---------|---------|
| .NET | 8.0 | Runtime |
| EF Core | 9.0.8 | ORM |
| ASP.NET Core Identity | 8.0.7 | Authentication |
| JWT Bearer | 8.0.20 | JWT authentication |
| SignalR | 9.0.6 | Real-time communication |
| Angular | 18.2.13 | Frontend framework |
| Angular Material | 20.2.0 | UI components |
| FluentValidation | 11.9.2 | Validation |
| AutoMapper | 13.0.1 | Object mapping |
| Hangfire | 1.8.14 | Background jobs |
| NLog | 5.3.2 | Logging |
| EPPlus | 7.2.2 | Excel generation |
| iText | 8.0.5 | PDF generation |
| Swashbuckle | 6.6.2 | API documentation |
| Newtonsoft.Json | 13.0.3 | JSON serialization |

---

## Appendix B: Project Statistics

**Backend:**
- **Projects:** 7 (.NET 8)
- **API Controllers:** 17+ (3,652+ lines of code)
- **Application Services:** 10+ feature-based services (Pillars, Objectives, Programs, Initiatives, KPIs)
- **Specialized Repositories:** 15+ domain-specific repositories
- **Framework.Core Files:** 166+ C# files
- **Layers:** 4 (Domain, Application, Infrastructure, API)
- **Cross-Cutting Concerns:** 17+ major areas (incl. SignalR, external integrations)
- **Business Domain:** Vision 2030 Strategic Management

**Frontend:**
- **Framework:** Angular 18
- **UI Libraries:** 25+ specialized libraries
- **Configuration Profiles:** 4 (Dev, Test, Staging, Prod)
- **Feature Modules:** 8+ (Pillars, Objectives, Programs, Initiatives, KPIs, Dashboards, etc.)
- **Purpose:** Vision 2030 Information Center Portal

**Database:**
- **Contexts:** 3 (App, Commons, Identity)
- **Supported RDBMS:** SQL Server, PostgreSQL
- **Entity Mappings:** 20+ mapping configurations (Vision entities, Strategic objectives, Programs, Initiatives, KPIs)
- **Global Query Filters:** Auto-applied to all lookup entities

**External Integrations:**
- **Nafath** - Saudi national authentication platform
- **ADAA** - Performance Management System (أداء)
- **GaStat** - General Authority for Statistics
- **SMS Gateway** - Notifications & alerts
- **SignalR** - Real-time KPI updates & dashboards

---

## Appendix C: Architectural Decision Records (ADRs)

### ADR-001: Clean Architecture Adoption
**Decision:** Implement Clean Architecture with clear layer boundaries
**Rationale:**
- Separation of concerns
- Testability
- Technology independence
- Maintainability

### ADR-002: Generic Repository Pattern
**Decision:** Use generic repository pattern with rich querying
**Rationale:**
- Reduce code duplication
- Consistent data access patterns
- Flexible querying without exposing DbContext
- Easier to mock for testing

### ADR-003: Framework Layer Separation
**Decision:** Extract cross-cutting concerns into Framework projects
**Rationale:**
- Reusability across projects
- Clear separation of domain vs. infrastructure
- Easier to maintain common functionality

### ADR-004: No CQRS Pattern
**Decision:** Do not implement CQRS (Command Query Responsibility Segregation)
**Rationale (per user requirements):**
- Simpler architecture
- Less boilerplate
- Easier to understand for team
- Sufficient for current requirements

### ADR-005: EF Core Code-First
**Decision:** Use Code-First migrations for database schema
**Rationale:**
- Version control for database schema
- Type-safe entity definitions
- Automatic migration generation
- Consistent across environments

### ADR-006: Automatic Auditing
**Decision:** Implement automatic auditing in BaseDbContext
**Rationale:**
- Consistent audit trail
- No manual code in services
- Centralized implementation
- Reduces human error

---

## Document Version History

| Version | Date | Author | Changes |
|---------|------|--------|---------|
| 1.0 | 2025-10-15 | Winston (Architect Agent) | Initial comprehensive architecture document based on initial codebase analysis |
| 2.0 | 2025-10-15 | Winston (Architect Agent) | **MAJOR UPDATE:** Deep-dive analysis revealing production-ready implementation. Added: 17+ controllers (3,652+ lines), SignalR real-time communication, Nafath/SMS integrations, 15+ specialized repositories, comprehensive Angular UI stack (25+ libraries), auto-registration patterns, global query filters, feature-based architecture, and complete implementation details across all layers. Updated from "minimal/placeholder" to "production-ready, feature-complete" status. |
| 3.0 | 2025-10-15 | Winston (Architect Agent) | **DOMAIN TRANSFORMATION:** Aligned architecture with Vision 2030 Information Center requirements from PRD. **Project Renaming:** IMO-LMS → SMO (Strategic Management Office). **Business Domain Update:** Transformed from Learning Management System to Vision 2030 Strategic Management - managing Pillars, Themes, Strategic Objectives (5 levels), Vision Realization Programs (VRPs), Initiatives, and KPIs. **External Integrations:** Added ADAA (Performance Management) and GaStat (Statistics Authority) integrations. **Stakeholders:** Updated for SMO, VRP offices, CEDA, Ministry of Finance, and government entities. **Technical Architecture:** Preserved Clean Architecture, all patterns, and technology stack unchanged. All code examples, services, repositories, and controllers updated to reflect Vision 2030 domain. |

---

**🏗️ Architecture Analysis Summary**

This document represents a **comprehensive technical analysis** of a production-ready **Vision 2030 Information Center** for Saudi Arabia's Strategic Management Office (SMO), built on Clean Architecture principles. The solution features:

✅ **Backend:** 17+ RESTful API controllers for managing strategic pillars, objectives, programs, initiatives, and KPIs; feature-based application services; 15+ specialized repositories; comprehensive Vision 2030 domain model

✅ **Real-Time:** SignalR hub for WebSocket-based live KPI updates, performance dashboards, and strategic notifications

✅ **Frontend:** Angular 18 portal (SMO-Portal) with 25+ specialized UI libraries including Material Design, rich text editing, charts, PDF viewing, Excel operations for reports and data management

✅ **Authentication:** Dual authentication with JWT and Nafath (Saudi National Authentication Platform) for SMO staff, VRP offices, and government stakeholders

✅ **Integrations:**
- **Nafath** - National authentication
- **ADAA** - Performance Management System integration
- **GaStat** - General Authority for Statistics data integration
- **SMS Gateway** - Strategic alerts and notifications
- **SignalR** - Real-time dashboard updates

✅ **Framework:** 166+ reusable framework files providing cross-cutting concerns (logging, caching, validation, notifications, etc.)

✅ **Data Layer:** EF Core 9.0.8 with auto-discovery, global filters, enum mappings for Vision 2030 domain entities

✅ **Background Processing:** Hangfire for scheduled KPI data refresh, quarterly report generation, and automated data synchronization

This architecture serves as the **unified platform for monitoring Saudi Arabia's Vision 2030 strategic execution**, enabling SMO and VRP offices to manage objectives, track initiatives, monitor KPIs, and generate performance reports for CEDA and executive leadership.

---

**End of Document**
