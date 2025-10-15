# 7. Data Architecture

## 7.1 Database Strategy

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

## 7.2 Entity Framework Core Configuration

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

## 7.3 Migrations

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

## 7.4 Auditing

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

## 7.5 Query Patterns

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
