# 8. Cross-Cutting Concerns

## 8.1 Logging

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

## 8.2 Exception Handling

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

## 8.3 Localization (Globalization)

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

## 8.4 Caching Strategy

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

## 8.5 Background Jobs (Hangfire)

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

## 8.6 Validation Strategy

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

## 8.7 File Management

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

## 8.8 PDF Generation

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

## 8.9 Excel Operations

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

## 8.10 Real-Time Communication (SignalR)

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

## 8.11 External Service Integrations

### **Nafath Service Integration (Saudi National Authentication)**

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

### **SMS Service Integration**

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

### **ADAA Platform Integration (Performance Management System)**

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

### **GaStat Integration (General Authority for Statistics)**

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
