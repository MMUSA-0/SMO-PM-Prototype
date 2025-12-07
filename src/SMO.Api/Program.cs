using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NLog;
using NLog.Web;
using SMO.Domain.Interfaces;
using SMO.Infrastructure.Data;

// Configure NLog for early logging
var logger = LogManager.Setup().LoadConfigurationFromFile("nlog.config").GetCurrentClassLogger();
logger.Info("SMO Vision Center API starting up...");

try
{
    var builder = WebApplication.CreateBuilder(args);

    #region NLog Configuration

    // Clear default logging providers and use NLog
    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    #endregion

    #region Database Configuration

    // Get connection strings from appsettings
    var defaultConnection = builder.Configuration.GetConnectionString("DefaultConnection");
    var commonsConnection = builder.Configuration.GetConnectionString("CommonsConnection");
    var identityConnection = builder.Configuration.GetConnectionString("IdentityConnection");

    // Register AppDbContext with SQL Server
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(defaultConnection,
            sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorNumbersToAdd: null);
            }));

    // Register IAppDbContext for dependency injection
    builder.Services.AddScoped<IAppDbContext>(provider =>
        provider.GetRequiredService<AppDbContext>());

    // Register IUnitOfWork for transaction coordination
    builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

    // Register generic IRepository<> for data access
    builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

    // TODO: Placeholder service registration extension methods (will be implemented in Task 23 and future stories)
    // services.ConfigureSharedApplicationServices(commonsConnection);
    // services.ConfigureApplicationServices();
    // services.ConfigureInfrastructureServices(defaultConnection);
    // services.IdentityConfigureServices(identityConnection);
    // services.AddDataProtection(identityConnection);
    // services.InitHangFireServices(commonsConnection);

    #endregion

    #region JWT Authentication

    // Configure JWT authentication
    var jwtSettings = builder.Configuration.GetSection("JwtSettings");
    var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");
    var issuer = jwtSettings["Issuer"] ?? "SMO.Api";
    var audience = jwtSettings["Audience"] ?? "SMO.Frontend";

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ClockSkew = TimeSpan.FromMinutes(5) // Allow 5 minutes clock skew
        };

        // Configure JWT for SignalR
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;

                // If the request is for SignalR hub, read token from query string
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/api/LiveInform"))
                {
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            }
        };
    });

    builder.Services.AddAuthorization();

    #endregion

    #region JSON Serialization

    // Configure JSON serialization for controllers
    builder.Services.AddControllers(options =>
        {
            // Add development authentication bypass filter (only active in Development)
            if (builder.Environment.IsDevelopment())
            {
                options.Filters.Add<SMO.Api.Filters.DevelopmentAuthBypassFilter>();
            }
        })
        .AddJsonOptions(options =>
        {
            // Handle circular references (important for EF Core navigation properties)
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;

            // Use camelCase for property names (JavaScript convention)
            options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;

            // Include fields in serialization (if needed)
            options.JsonSerializerOptions.IncludeFields = false;

            // Write indented JSON in development for readability
            options.JsonSerializerOptions.WriteIndented = builder.Environment.IsDevelopment();
        });

    // Also configure Newtonsoft.Json settings for compatibility with legacy code
    builder.Services.AddControllers()
        .AddNewtonsoftJson(options =>
        {
            options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
        });

    #endregion

    #region CORS Configuration

    // Get CORS origins from appsettings
    var corsOrigins = builder.Configuration.GetSection("CorsOrigins").Get<string[]>() ?? new[] { "http://localhost:4200" };

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("corsapp", policy =>
        {
            policy.WithOrigins(corsOrigins)
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials(); // Required for SignalR
        });
    });

    #endregion

    #region SignalR Configuration

    // Add SignalR for real-time communication (KPI updates, notifications, etc.)
    builder.Services.AddSignalR(options =>
    {
        // Configure SignalR options
        options.EnableDetailedErrors = builder.Environment.IsDevelopment();
        options.KeepAliveInterval = TimeSpan.FromSeconds(15);
        options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
    });

    #endregion

    #region Swagger/OpenAPI Configuration

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "SMO Vision Center API",
            Version = "v1",
            Description = "API for Saudi Arabia's Strategic Management Office (SMO) - Vision 2030 Information Center",
            Contact = new OpenApiContact
            {
                Name = "SMO Development Team",
                Email = "support@smo.gov.sa"
            }
        });

        // Configure JWT Bearer authentication in Swagger
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n" +
                          "Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\n" +
                          "Example: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...\""
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
    });

    #endregion

    #region HTTP Configuration

    // Add HTTP context accessor for accessing current user in repositories/services
    builder.Services.AddHttpContextAccessor();

    // Add HTTP logging for debugging
    builder.Services.AddHttpLogging(options =>
    {
        options.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.All;
    });

    #endregion

    var app = builder.Build();

    #region Middleware Pipeline

    // Swagger UI (development and staging)
    if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "SMO Vision Center API v1");
            options.RoutePrefix = "swagger"; // Access at /swagger
            options.DocumentTitle = "SMO Vision Center API";
        });
    }

    // HTTP logging (development only)
    if (app.Environment.IsDevelopment())
    {
        app.UseHttpLogging();
    }

    // TODO: Shared middlewares from Framework.Core (will be implemented in future stories)
    // app.UseSharedMiddlewares();

    // Standard ASP.NET Core middleware pipeline
    app.UseHttpsRedirection();
    app.UseRouting();

    // CORS must be before Authentication/Authorization
    app.UseCors("corsapp");

    // Authentication & Authorization
    app.UseAuthentication();
    app.UseAuthorization();

    // Map controllers
    app.MapControllers();

    // Map SignalR hub for real-time updates
    // TODO: SignalR hub implementation (will be created in future stories)
    // app.MapHub<SignalRHub>("/api/LiveInform");

    #endregion

    #region Database Migrations (Disabled until entities are created)

    // TODO: Uncomment these when entities and migrations are created in future stories
    // Database migrations will fail now because no entities exist yet - this is expected
    // app.UseIdentityDBMigration();
    // app.UseDataKeysMigration();
    // app.UseSharedCommonDBMigration();
    // app.UseApplicationDBMigration();

    #endregion

    #region Hangfire Dashboard (Disabled until Hangfire is configured)

    // TODO: Uncomment when Hangfire is fully configured in future stories
    // app.InitHangfireDashboard();

    #endregion

    logger.Info("SMO Vision Center API started successfully");
    app.Run();
}
catch (Exception ex)
{
    logger.Error(ex, "Application stopped due to exception");
    throw;
}
finally
{
    LogManager.Shutdown();
}
