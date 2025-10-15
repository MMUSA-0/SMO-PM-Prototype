# 6. Framework Layer

The Framework layer provides reusable, application-agnostic infrastructure suitable for any project.

## 6.1 Framework.Core

**Purpose:** Core framework capabilities and cross-cutting concerns

**Major Components (166+ files):**

### **Data Layer** (`Framework.Core/Data/`)

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

### **AutoMapper Integration** (`Framework.Core/AutoMapper/`)

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

### **Validation** (`Framework.Core/Validators/`)

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

### **Caching** (`Framework.Core/Caching/`)

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

### **Background Jobs** (`Framework.Core/BackgroundJobs/`)

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

### **Notifications** (`Framework.Core/Notifications/`)

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

### **Middlewares** (`Framework.Core/Middlewares/`)

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

### **Shared Services** (`Framework.Core/SharedServices/`)

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

### **Helpers & Extensions** (`Framework.Core/Helpers/`, `Framework.Core/Extensions/`)

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

### **API Support** (`Framework.Core/`)

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

## 6.2 Framework.Identity

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

## 6.3 Framework.Resources

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
