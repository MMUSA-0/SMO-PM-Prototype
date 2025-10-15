# 9. Coding Standards & Patterns

## 9.1 Naming Conventions

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

## 9.2 Project Organization Patterns

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

## 9.3 Dependency Injection Patterns

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

## 9.4 CRITICAL ARCHITECTURAL RULES

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

## 9.5 Async/Await Patterns

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

## 9.6 Error Handling Patterns

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

## 9.7 Testing Patterns

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
