# SMO UI Builder Skill

## Purpose
Comprehensive skill for integrating SMO-Platform-UI templates into the Angular application and generating complete full-stack user stories with backend (.NET 8), frontend (Angular 18), unit tests, and E2E tests.

## When to Use This Skill
- When implementing any user story from the PRD (docs/PRD.md)
- When creating new CRUD features for Vision 2030 entities
- When needing to convert SMO-Platform-UI HTML templates to Angular components
- When setting up bilingual (Arabic/English) RTL/LTR support

## SMO-Platform-UI Structure Reference

### Available HTML Pages
Located in `/SMO-Platform-UI/`:
- **login.html** - Nafath login page with centered card
- **indicators.html** - Complete CRUD interface with table/card views, filters, pagination
- **vision.html** - Information display page with statistics, tabs, article content

### Available UI Components (Extracted from HTML)

#### 1. **Page Header** (indicators.html:31-152, vision.html:31-152)
```html
<header class="page-header">
  - Logo
  - Sidebar toggle button
  - Search menu (expandable)
  - Grayscale toggle
  - Notifications dropdown (with unread count, tabs)
  - User dropdown (profile, settings, logout)
</header>
```

#### 2. **Page Sidebar Navigation** (indicators.html:155-241, vision.html:155-241)
```html
<aside class="page-sidebar">
  - Hierarchical navigation
  - Active state indicators
  - Expandable submenus
  - Icon + text layout
</aside>
```

#### 3. **Breadcrumb** (indicators.html:250-260, vision.html:250-260)
```html
<nav aria-label="breadcrumb">
  <ol class="breadcrumb">
    - Home icon link
    - Active page
  </ol>
</nav>
```

#### 4. **Statistics Cards** (indicators.html:272-316, vision.html:272-316)
```html
<div class="statistic-card">
  - Count/percentage display
  - Label
  - Brief description
</div>
```

#### 5. **Accordion** (indicators.html:264-320, vision.html:264-348)
```html
<div class="accordion">
  - Collapsible header
  - Expandable body
  - Default show/hide state
</div>
```

#### 6. **Tabs** (indicators.html:325-339, vision.html:352-369)
```html
<ul class="nav nav-tabs">
  - Active state
  - Click navigation
</ul>
```

#### 7. **Data Table** (indicators.html:383-783)
```html
<table class="table table-borderless">
  <thead> - Column headers with sortable fields
  <tbody> - Data rows with:
    - Text fields
    - Progress bars (color-coded: success/danger)
    - Currency formatting (SAR)
    - Date fields
    - Pills/badges (increasing/decreasing values)
    - Actions dropdown (view/edit/delete)
</table>
```

#### 8. **Card Grid View** (indicators.html:788-1142)
```html
<div class="indicator-card">
  - Header (pill badge + actions dropdown)
  - Title
  - Details list (label + data pairs)
  - Progress bars, currency, dates
</div>
```

#### 9. **Search & Filters** (indicators.html:356-380)
```html
- Search input (form-control search-control)
- Select dropdowns (form-select)
- View toggle buttons (list/grid icons)
```

#### 10. **Pagination** (indicators.html:1144-1180)
```html
<ul class="pagination">
  - Previous/Next with icons
  - Numbered pages
  - Active state
  - Disabled state
</ul>
```

#### 11. **Actions Dropdown** (indicators.html:420-443)
```html
<div class="dropdown actions-dropdown">
  - Toggle button (three dots)
  - Menu items:
    - View (view-icon-green.svg)
    - Edit (edit-icon-green.svg)
    - Delete (delete-icon-green.svg)
</div>
```

#### 12. **Notifications Dropdown** (indicators.html:53-113, vision.html:53-113)
```html
<div class="dropdown notifications-dropdown">
  - Header (icon + title)
  - Tabs (All / New)
  - List items:
    - User photo
    - Title
    - Description
    - Date/time
    - Unread indicator
  - Footer ("View all" button)
</div>
```

#### 13. **Page Preloader** (All pages:1197-1208)
```html
<div class="page-preloader">
  - Loading bar animation
  - Spinner with logo
</div>
```

#### 14. **Login Card** (login.html:42-54)
```html
<div class="login-card">
  - Logo (NIC)
  - Title + description
  - Primary action button
</div>
```

#### 15. **Article Content** (vision.html:374-411)
```html
<article class="page-article">
  - Images grid
  - Vision logo
  - Title
  - Points list
  - Rich text paragraphs
</article>
```

#### 16. **Latest Achievements Marquee** (vision.html:319-342)
```html
<div class="latest-achievements">
  - Swiper carousel
  - Auto-scroll items
  - Ranking icon
</div>
```

### CSS/SASS Structure
Located in `/SMO-Platform-UI/`:
- **css/** - Compiled stylesheets
  - `bootstrap.rtl.min.css` - RTL Bootstrap
  - `styles.min.css` - Custom styles
  - `bootstrap-icons.min.css` - Icons
- **sass/** - Source SASS files
  - `abstracts/_variables.scss` - Colors, spacing, fonts
  - `components/` - Individual component styles
    - `_buttons.sass`, `_forms.sass`, `_table.sass`, `_modal.sass`, etc.
  - `pages/` - Page-specific styles

### Fonts
**29LT Bukra** - Arabic font family with weights:
- Thin, Light, Regular, Medium, SemiBold, Bold
- Formats: .eot, .svg, .ttf, .woff, .woff2

### Icons & Images
- SVG icons in `/images/nav-icons/` for navigation
- Action icons (add, edit, delete, view)
- Logo files (SMO, Vision 2030, NIC)
- Favicons

## User Story Implementation Workflow

### Phase 1: Parse PRD and Plan

**Input:** User story number or description (e.g., "UC-04: عرض قائمة الأهداف الإستراتيجية")

**Steps:**
1. Read `docs/PRD.md` to locate the use case
2. Extract:
   - Use case name (Arabic + English)
   - Description
   - User roles/permissions
   - Pre-conditions
   - Main workflow steps
   - Fields and validation rules
   - Business rules
3. Identify which SMO-Platform-UI page matches the use case:
   - List views → `indicators.html` patterns
   - Information display → `vision.html` patterns
   - Login/auth → `login.html` patterns
4. Create TodoWrite list with ALL tasks:
   - Backend entity creation
   - Backend service/repository/controller
   - Backend unit tests
   - Frontend module/components
   - Frontend services
   - Frontend unit tests
   - E2E tests
   - Migration
   - Git commit

### Phase 2: Backend Implementation (.NET 8 Clean Architecture)

#### Step 1: Create Domain Entity

**Location:** `src/SMO.Domain/Entities/`

**Template:**
```csharp
using Framework.Core.Data;

namespace SMO.Domain.Entities
{
    // For regular entities (transactions, business data)
    public class {EntityName} : FullAuditedEntityBase<int>
    {
        // Bilingual properties
        public string NameAr { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;

        // Business properties from PRD
        public string Code { get; set; } = string.Empty;
        public string? DescriptionAr { get; set; }
        public string? DescriptionEn { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; } = true;

        // Foreign keys
        public int? ParentId { get; set; }

        // Navigation properties
        public virtual {ParentEntity}? Parent { get; set; }
        public virtual ICollection<{ChildEntity}> Children { get; set; } = new List<{ChildEntity}>();
    }

    // For lookup/master data tables
    public class {EntityName}Status : LookupEntityBase<int>
    {
        // LookupEntityBase provides: NameAr, NameEn, Name, IsActive, Order, audit fields
        // Add only entity-specific properties
        public string? ColorCode { get; set; }
        public string? IconClass { get; set; }

        // Navigation properties
        public virtual ICollection<{EntityName}> Items { get; set; } = new List<{EntityName}>();
    }
}
```

**Rules:**
- ALWAYS inherit from `FullAuditedEntityBase<int>` for regular entities
- Use `LookupEntityBase<int>` for status/type/category lookup tables
- Include NameAr and NameEn for bilingual support
- Follow PRD field requirements exactly
- Add XML comments from PRD descriptions

#### Step 2: Create EF Core Entity Configuration

**Location:** `src/SMO.Infrastructure/Mapping/{EntityName}Configuration.cs`

**Template:**
```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SMO.Domain.Entities;

namespace SMO.Infrastructure.Mapping
{
    public class {EntityName}Configuration : IEntityTypeConfiguration<{EntityName}>
    {
        public void Configure(EntityTypeBuilder<{EntityName}> builder)
        {
            builder.ToTable("{TableName}"); // Plural form

            // Primary key (already in FullAuditedEntityBase)
            builder.HasKey(x => x.Id);

            // Required fields with max length from PRD
            builder.Property(x => x.Code)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.NameAr)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(x => x.NameEn)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(x => x.DescriptionAr)
                .HasMaxLength(2000);

            builder.Property(x => x.DescriptionEn)
                .HasMaxLength(2000);

            // Decimal precision
            builder.Property(x => x.Budget)
                .HasPrecision(18, 2);

            // Relationships
            builder.HasOne(x => x.Parent)
                .WithMany(p => p.Children)
                .HasForeignKey(x => x.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes for performance
            builder.HasIndex(x => x.Code).IsUnique();
            builder.HasIndex(x => x.ParentId);
            builder.HasIndex(x => x.IsActive);
            builder.HasIndex(x => x.Order);
        }
    }
}
```

#### Step 3: Register in AppDbContext

**Location:** `src/SMO.Infrastructure/Data/AppDbContext.cs`

```csharp
// Add DbSet
public DbSet<{EntityName}> {EntityNamePlural} { get; set; }

// In OnModelCreating, add configuration
modelBuilder.ApplyConfiguration(new {EntityName}Configuration());
```

#### Step 4: Create DTOs

**Location:** `src/SMO.Application/DTOs/{EntityName}/`

```csharp
// {EntityName}Dto.cs
public class {EntityName}Dto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string Name => CultureHelper.IsArabic ? NameAr : NameEn; // i18n
    public string? DescriptionAr { get; set; }
    public string? DescriptionEn { get; set; }
    public string? Description => CultureHelper.IsArabic ? DescriptionAr : DescriptionEn;
    public int Order { get; set; }
    public bool IsActive { get; set; }
    public int? ParentId { get; set; }
    public string? ParentName { get; set; }
    public DateTime CreatedOn { get; set; }
    public string? CreatedBy { get; set; }
}

// Create{EntityName}Dto.cs
public class Create{EntityName}Dto
{
    public string Code { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string? DescriptionAr { get; set; }
    public string? DescriptionEn { get; set; }
    public int Order { get; set; }
    public bool IsActive { get; set; } = true;
    public int? ParentId { get; set; }
}

// Update{EntityName}Dto.cs
public class Update{EntityName}Dto
{
    public int Id { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string? DescriptionAr { get; set; }
    public string? DescriptionEn { get; set; }
    public int Order { get; set; }
    public bool IsActive { get; set; }
    public int? ParentId { get; set; }
}
```

#### Step 5: Create AutoMapper Profile

**Location:** `src/SMO.Application/MappingProfiles/{EntityName}Profile.cs`

```csharp
using AutoMapper;
using SMO.Domain.Entities;
using SMO.Application.DTOs.{EntityName};

namespace SMO.Application.MappingProfiles
{
    public class {EntityName}Profile : Profile
    {
        public {EntityName}Profile()
        {
            CreateMap<{EntityName}, {EntityName}Dto>()
                .ForMember(dest => dest.ParentName, opt => opt.MapFrom(src =>
                    src.Parent != null
                        ? (CultureHelper.IsArabic ? src.Parent.NameAr : src.Parent.NameEn)
                        : null));

            CreateMap<Create{EntityName}Dto, {EntityName}>();
            CreateMap<Update{EntityName}Dto, {EntityName}>();
        }
    }
}
```

#### Step 6: Create FluentValidation Validators

**Location:** `src/SMO.Application/Validators/{EntityName}/`

```csharp
using FluentValidation;
using SMO.Application.DTOs.{EntityName};

namespace SMO.Application.Validators.{EntityName}
{
    public class Create{EntityName}DtoValidator : AbstractValidator<Create{EntityName}Dto>
    {
        public Create{EntityName}DtoValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("الرمز مطلوب | Code is required")
                .MaximumLength(50).WithMessage("الرمز يجب ألا يتجاوز 50 حرف | Code must not exceed 50 characters");

            RuleFor(x => x.NameAr)
                .NotEmpty().WithMessage("الاسم بالعربية مطلوب | Arabic name is required")
                .MaximumLength(250).WithMessage("الاسم يجب ألا يتجاوز 250 حرف | Name must not exceed 250 characters");

            RuleFor(x => x.NameEn)
                .NotEmpty().WithMessage("الاسم بالإنجليزية مطلوب | English name is required")
                .MaximumLength(250).WithMessage("الاسم يجب ألا يتجاوز 250 حرف | Name must not exceed 250 characters");

            RuleFor(x => x.Order)
                .GreaterThanOrEqualTo(0).WithMessage("الترتيب يجب أن يكون رقم موجب | Order must be positive");
        }
    }

    public class Update{EntityName}DtoValidator : AbstractValidator<Update{EntityName}Dto>
    {
        public Update{EntityName}DtoValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("المعرف غير صالح | Invalid ID");

            // Same rules as Create validator
        }
    }
}
```

#### Step 7: Create Application Service

**Location:** `src/SMO.Application/Services/{EntityName}AppService.cs`

**CRITICAL:** Follow CLAUDE.md rules:
- Inject `IRepository<T>` directly (NOT custom repository unless needed)
- Inject `IUnitOfWork` for transaction management
- Use `TableNoTracking` for read-only queries
- Use default `GetAsync`, `GetByIdAsync`, `InsertAsync`, `UpdateAsync`, `DeleteAsync` methods

```csharp
using AutoMapper;
using Framework.Core.Data;
using SMO.Domain.Entities;
using SMO.Domain.Interfaces;
using SMO.Application.DTOs.{EntityName};
using System.Linq.Expressions;

namespace SMO.Application.Services
{
    public class {EntityName}AppService
    {
        private readonly IRepository<{EntityName}> _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public {EntityName}AppService(
            IRepository<{EntityName}> repository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET LIST with filtering, sorting, pagination
        public async Task<List<{EntityName}Dto>> GetAllAsync(
            string? searchTerm = null,
            int? parentId = null,
            bool? isActive = null,
            int skip = 0,
            int take = 20)
        {
            var predicate = BuildPredicate(searchTerm, parentId, isActive);

            var entities = await _repository.GetAsync(
                predicate: predicate,
                orderBy: q => q.OrderBy(x => x.Order).ThenBy(x => x.NameAr),
                includes: new List<Expression<Func<{EntityName}, object>>>
                {
                    x => x.Parent
                },
                disableTracking: true, // IMPORTANT: Use TableNoTracking for performance
                skip: skip,
                take: take
            );

            return _mapper.Map<List<{EntityName}Dto>>(entities);
        }

        // GET BY ID with related entities
        public async Task<{EntityName}Dto?> GetByIdAsync(int id)
        {
            var entity = await _repository.GetSingleWithDeepRelationsAsync(
                predicate: x => x.Id == id,
                include: source => source
                    .Include(x => x.Parent)
                    .Include(x => x.Children)
            );

            return entity != null ? _mapper.Map<{EntityName}Dto>(entity) : null;
        }

        // CREATE
        public async Task<{EntityName}Dto> CreateAsync(Create{EntityName}Dto dto)
        {
            // Business validation
            await ValidateUniqueCode(dto.Code);

            var entity = _mapper.Map<{EntityName}>(dto);
            await _repository.InsertAsync(entity, autoSave: false);

            await _unitOfWork.SaveChangesAsync(); // Transaction commit

            return _mapper.Map<{EntityName}Dto>(entity);
        }

        // UPDATE
        public async Task<{EntityName}Dto> UpdateAsync(int id, Update{EntityName}Dto dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
                throw new Exception($"Entity with ID {id} not found");

            // Map updates (Code is immutable per PRD)
            _mapper.Map(dto, entity);

            await _unitOfWork.SaveChangesAsync(); // EF tracks changes automatically

            return _mapper.Map<{EntityName}Dto>(entity);
        }

        // DELETE
        public async Task<bool> DeleteAsync(int id)
        {
            // Check dependencies per PRD business rules
            var hasChildren = await _repository.TableNoTracking
                .AnyAsync(x => x.ParentId == id);

            if (hasChildren)
                throw new Exception("لا يمكن حذف العنصر لوجود عناصر مرتبطة به | Cannot delete item with dependencies");

            await _repository.DeleteAsync(id, autoSave: false);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        // ACTIVATE/DEACTIVATE
        public async Task<bool> ToggleActiveAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return false;

            entity.IsActive = !entity.IsActive;
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        // Private helpers
        private Expression<Func<{EntityName}, bool>> BuildPredicate(
            string? searchTerm, int? parentId, bool? isActive)
        {
            Expression<Func<{EntityName}, bool>> predicate = x => true;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var search = searchTerm.ToLower();
                predicate = predicate.And(x =>
                    x.Code.ToLower().Contains(search) ||
                    x.NameAr.ToLower().Contains(search) ||
                    x.NameEn.ToLower().Contains(search));
            }

            if (parentId.HasValue)
                predicate = predicate.And(x => x.ParentId == parentId.Value);

            if (isActive.HasValue)
                predicate = predicate.And(x => x.IsActive == isActive.Value);

            return predicate;
        }

        private async Task ValidateUniqueCode(string code, int? excludeId = null)
        {
            var exists = await _repository.TableNoTracking
                .AnyAsync(x => x.Code == code && (!excludeId.HasValue || x.Id != excludeId.Value));

            if (exists)
                throw new Exception($"الرمز '{code}' موجود مسبقاً | Code '{code}' already exists");
        }
    }
}
```

#### Step 8: Create API Controller

**Location:** `src/SMO.Api/Controllers/{EntityName}Controller.cs`

**CRITICAL:** Controllers inject AppServices ONLY (never repositories!)

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMO.Application.Services;
using SMO.Application.DTOs.{EntityName};

namespace SMO.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Require JWT authentication
    public class {EntityNamePlural}Controller : ControllerBase
    {
        private readonly {EntityName}AppService _service;

        public {EntityNamePlural}Controller({EntityName}AppService service)
        {
            _service = service;
        }

        /// <summary>
        /// Get all {entity} with optional filters
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<{EntityName}Dto>>> GetAll(
            [FromQuery] string? search,
            [FromQuery] int? parentId,
            [FromQuery] bool? isActive,
            [FromQuery] int skip = 0,
            [FromQuery] int take = 20)
        {
            try
            {
                var result = await _service.GetAllAsync(search, parentId, isActive, skip, take);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get {entity} by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<{EntityName}Dto>> GetById(int id)
        {
            try
            {
                var result = await _service.GetByIdAsync(id);
                if (result == null)
                    return NotFound(new { message = $"العنصر غير موجود | Item not found" });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Create new {entity}
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<{EntityName}Dto>> Create([FromBody] Create{EntityName}Dto dto)
        {
            try
            {
                var result = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Update existing {entity}
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<{EntityName}Dto>> Update(int id, [FromBody] Update{EntityName}Dto dto)
        {
            try
            {
                var result = await _service.UpdateAsync(id, dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Delete {entity}
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                await _service.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Toggle active status
        /// </summary>
        [HttpPatch("{id}/toggle-active")]
        public async Task<ActionResult> ToggleActive(int id)
        {
            try
            {
                var result = await _service.ToggleActiveAsync(id);
                if (!result)
                    return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
```

#### Step 9: Register Services in DI

**Location:** `src/SMO.Application/ServiceCollectionExtensions.cs`

```csharp
// Add service registration
services.AddScoped<{EntityName}AppService>();

// Validators are auto-registered by FluentValidation
```

#### Step 10: Create EF Migration

```bash
cd src/SMO.Api
dotnet ef migrations add Add_{EntityNamePlural} --context AppDbContext
dotnet ef database update --context AppDbContext
```

#### Step 11: Create Backend Unit Tests

**Location:** `tests/SMO.Application.Tests/Services/{EntityName}AppServiceTests.cs`

```csharp
using Xunit;
using Moq;
using AutoMapper;
using SMO.Application.Services;
using SMO.Domain.Entities;
using SMO.Domain.Interfaces;
using Framework.Core.Data;

namespace SMO.Application.Tests.Services
{
    public class {EntityName}AppServiceTests
    {
        private readonly Mock<IRepository<{EntityName}>> _mockRepository;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly {EntityName}AppService _service;

        public {EntityName}AppServiceTests()
        {
            _mockRepository = new Mock<IRepository<{EntityName}>>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _service = new {EntityName}AppService(
                _mockRepository.Object,
                _mockUnitOfWork.Object,
                _mockMapper.Object
            );
        }

        [Fact]
        public async Task GetAllAsync_ReturnsItems_WhenItemsExist()
        {
            // Arrange
            var entities = new List<{EntityName}>
            {
                new {EntityName} { Id = 1, Code = "TEST001", NameAr = "اختبار", NameEn = "Test" }
            };

            _mockRepository.Setup(r => r.GetAsync(
                It.IsAny<Expression<Func<{EntityName}, bool>>>(),
                It.IsAny<Func<IQueryable<{EntityName}>, IOrderedQueryable<{EntityName}>>>(),
                It.IsAny<List<Expression<Func<{EntityName}, object>>>>(),
                It.IsAny<bool>(),
                It.IsAny<int>(),
                It.IsAny<int>()
            )).ReturnsAsync(entities);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task CreateAsync_ThrowsException_WhenCodeAlreadyExists()
        {
            // Arrange
            var dto = new Create{EntityName}Dto { Code = "TEST001" };

            _mockRepository.Setup(r => r.TableNoTracking)
                .Returns(new List<{EntityName}>
                {
                    new {EntityName} { Code = "TEST001" }
                }.AsQueryable().BuildMock());

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _service.CreateAsync(dto));
        }

        [Fact]
        public async Task DeleteAsync_ThrowsException_WhenHasDependencies()
        {
            // Arrange
            var id = 1;

            _mockRepository.Setup(r => r.TableNoTracking)
                .Returns(new List<{EntityName}>
                {
                    new {EntityName} { ParentId = id }
                }.AsQueryable().BuildMock());

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _service.DeleteAsync(id));
        }
    }
}
```

### Phase 3: Frontend Implementation (Angular 18)

#### Step 1: Generate Feature Module

```bash
cd src/SMO.Frontend/SMO-Portal
ng generate module features/{feature-name} --routing
ng generate component features/{feature-name}/{entity-name}-list
ng generate component features/{feature-name}/{entity-name}-form
ng generate component features/{feature-name}/{entity-name}-details
ng generate service features/{feature-name}/services/{entity-name}
```

#### Step 2: Create TypeScript Models

**Location:** `src/app/features/{feature-name}/models/{entity-name}.model.ts`

```typescript
export interface {EntityName} {
  id: number;
  code: string;
  nameAr: string;
  nameEn: string;
  name: string; // Culture-based from backend
  descriptionAr?: string;
  descriptionEn?: string;
  description?: string;
  order: number;
  isActive: boolean;
  parentId?: number;
  parentName?: string;
  createdOn: Date;
  createdBy?: string;
}

export interface Create{EntityName} {
  code: string;
  nameAr: string;
  nameEn: string;
  descriptionAr?: string;
  descriptionEn?: string;
  order: number;
  isActive: boolean;
  parentId?: number;
}

export interface Update{EntityName} {
  id: number;
  nameAr: string;
  nameEn: string;
  descriptionAr?: string;
  descriptionEn?: string;
  order: number;
  isActive: boolean;
  parentId?: number;
}
```

#### Step 3: Create HTTP Service

**Location:** `src/app/features/{feature-name}/services/{entity-name}.service.ts`

```typescript
import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '@environments/environment';
import { {EntityName}, Create{EntityName}, Update{EntityName} } from '../models/{entity-name}.model';

@Injectable({
  providedIn: 'root'
})
export class {EntityName}Service {
  private apiUrl = `${environment.apiUrl}/{entity-name-plural}`;

  constructor(private http: HttpClient) { }

  getAll(
    search?: string,
    parentId?: number,
    isActive?: boolean,
    skip: number = 0,
    take: number = 20
  ): Observable<{EntityName}[]> {
    let params = new HttpParams()
      .set('skip', skip.toString())
      .set('take', take.toString());

    if (search) params = params.set('search', search);
    if (parentId) params = params.set('parentId', parentId.toString());
    if (isActive !== undefined) params = params.set('isActive', isActive.toString());

    return this.http.get<{EntityName}[]>(this.apiUrl, { params });
  }

  getById(id: number): Observable<{EntityName}> {
    return this.http.get<{EntityName}>(`${this.apiUrl}/${id}`);
  }

  create(data: Create{EntityName}): Observable<{EntityName}> {
    return this.http.post<{EntityName}>(this.apiUrl, data);
  }

  update(id: number, data: Update{EntityName}): Observable<{EntityName}> {
    return this.http.put<{EntityName}>(`${this.apiUrl}/${id}`, data);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  toggleActive(id: number): Observable<void> {
    return this.http.patch<void>(`${this.apiUrl}/${id}/toggle-active`, {});
  }
}
```

#### Step 4: Create List Component (Using SMO-UI Table Pattern)

**Location:** `src/app/features/{feature-name}/{entity-name}-list/{entity-name}-list.component.ts`

**Reference:** `indicators.html:383-1182` for table structure

```typescript
import { Component, OnInit } from '@angular/core';
import { {EntityName}Service } from '../services/{entity-name}.service';
import { {EntityName} } from '../models/{entity-name}.model';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-{entity-name}-list',
  templateUrl: './{entity-name}-list.component.html',
  styleUrls: ['./{entity-name}-list.component.scss']
})
export class {EntityName}ListComponent implements OnInit {
  items: {EntityName}[] = [];
  loading = false;
  viewMode: 'table' | 'grid' = 'table';

  // Filters
  searchTerm = '';
  selectedParentId?: number;
  selectedIsActive?: boolean;

  // Pagination
  currentPage = 1;
  pageSize = 20;
  totalItems = 0;

  constructor(
    private service: {EntityName}Service,
    public translate: TranslateService
  ) { }

  ngOnInit(): void {
    this.loadItems();
  }

  loadItems(): void {
    this.loading = true;
    const skip = (this.currentPage - 1) * this.pageSize;

    this.service.getAll(this.searchTerm, this.selectedParentId, this.selectedIsActive, skip, this.pageSize)
      .subscribe({
        next: (data) => {
          this.items = data;
          this.loading = false;
        },
        error: (err) => {
          console.error('Error loading items:', err);
          this.loading = false;
        }
      });
  }

  onSearch(): void {
    this.currentPage = 1;
    this.loadItems();
  }

  onPageChange(page: number): void {
    this.currentPage = page;
    this.loadItems();
  }

  toggleViewMode(): void {
    this.viewMode = this.viewMode === 'table' ? 'grid' : 'table';
  }

  onDelete(id: number): void {
    if (confirm(this.translate.instant('CONFIRM_DELETE'))) {
      this.service.delete(id).subscribe({
        next: () => this.loadItems(),
        error: (err) => console.error('Delete failed:', err)
      });
    }
  }

  onToggleActive(id: number): void {
    this.service.toggleActive(id).subscribe({
      next: () => this.loadItems(),
      error: (err) => console.error('Toggle failed:', err)
    });
  }
}
```

**Template:** `{entity-name}-list.component.html`

```html
<!-- Breadcrumb - from vision.html:250-260 -->
<nav aria-label="breadcrumb">
  <ol class="breadcrumb">
    <li class="breadcrumb-item">
      <a routerLink="/">
        <img src="assets/images/home-icon.svg" alt="">
      </a>
    </li>
    <li class="breadcrumb-item active" aria-current="page">
      {{ 'BREADCRUMB.VISION_CENTER' | translate }}
    </li>
  </ol>
</nav>

<!-- Statistics Accordion - from indicators.html:264-320 -->
<div class="accordion" id="accordionStatistics">
  <div class="accordion-item">
    <h2 class="accordion-header">
      <button class="accordion-button" type="button" data-bs-toggle="collapse" data-bs-target="#collapseStatistics">
        {{ 'STATISTICS.TITLE' | translate }}
      </button>
    </h2>
    <div id="collapseStatistics" class="accordion-collapse collapse show">
      <div class="accordion-body">
        <div class="row g-4">
          <!-- Statistics cards here -->
        </div>
      </div>
    </div>
  </div>
</div>

<!-- Search & Filters - from indicators.html:356-380 -->
<div class="row g-3 mb-3">
  <div class="col-12 col-md-4 col-lg-5">
    <input class="form-control search-control"
           type="search"
           [(ngModel)]="searchTerm"
           (keyup.enter)="onSearch()"
           [placeholder]="'SEARCH.PLACEHOLDER' | translate">
  </div>
  <div class="col-auto ms-auto">
    <div class="view-btns">
      <button type="button"
              class="view-btns__item"
              [class.active]="viewMode === 'table'"
              (click)="toggleViewMode()">
        <img src="assets/images/list-view-icon.svg" alt="">
      </button>
      <button type="button"
              class="view-btns__item"
              [class.active]="viewMode === 'grid'"
              (click)="toggleViewMode()">
        <img src="assets/images/grid-view-icon.svg" alt="">
      </button>
    </div>
  </div>
</div>

<!-- Table View - from indicators.html:383-783 -->
<div *ngIf="viewMode === 'table'" class="table-responsive">
  <table class="table table-borderless">
    <thead>
      <tr>
        <th scope="col">{{ 'TABLE.CODE' | translate }}</th>
        <th scope="col">{{ 'TABLE.NAME' | translate }}</th>
        <th scope="col">{{ 'TABLE.ORDER' | translate }}</th>
        <th scope="col">{{ 'TABLE.STATUS' | translate }}</th>
        <th scope="col">{{ 'TABLE.ACTIONS' | translate }}</th>
      </tr>
    </thead>
    <tbody>
      <tr *ngFor="let item of items">
        <td>
          <span class="text-default">{{ item.code }}</span>
        </td>
        <td>{{ item.name }}</td>
        <td>{{ item.order }}</td>
        <td>
          <span [class]="item.isActive ? 'increasing-value' : 'decreasing-value'">
            {{ item.isActive ? ('ACTIVE' | translate) : ('INACTIVE' | translate) }}
          </span>
        </td>
        <td>
          <div class="dropdown actions-dropdown">
            <button class="dropdown-toggle" type="button" data-bs-toggle="dropdown">
            </button>
            <ul class="dropdown-menu">
              <li>
                <a class="dropdown-item" [routerLink]="[item.id]">
                  <img src="assets/images/view-icon-green.svg" alt="">
                  <span>{{ 'ACTIONS.VIEW' | translate }}</span>
                </a>
              </li>
              <li>
                <a class="dropdown-item" [routerLink]="[item.id, 'edit']">
                  <img src="assets/images/edit-icon-green.svg" alt="">
                  <span>{{ 'ACTIONS.EDIT' | translate }}</span>
                </a>
              </li>
              <li>
                <a class="dropdown-item" (click)="onDelete(item.id)">
                  <img src="assets/images/delete-icon-green.svg" alt="">
                  <span>{{ 'ACTIONS.DELETE' | translate }}</span>
                </a>
              </li>
            </ul>
          </div>
        </td>
      </tr>
    </tbody>
  </table>
</div>

<!-- Grid View - from indicators.html:788-1142 -->
<div *ngIf="viewMode === 'grid'" class="row g-4">
  <div *ngFor="let item of items" class="col-12 col-sm-6 col-md-6 col-lg-4 col-xl-6 col-xxl-4">
    <div class="indicator-card">
      <div class="indicator-card__header">
        <div class="pill" [class.pill--success]="item.isActive" [class.pill--danger]="!item.isActive">
          <span>{{ item.isActive ? ('ACTIVE' | translate) : ('INACTIVE' | translate) }}</span>
        </div>
        <div class="dropdown actions-dropdown">
          <button class="dropdown-toggle" type="button" data-bs-toggle="dropdown"></button>
          <ul class="dropdown-menu">
            <li><a class="dropdown-item" [routerLink]="[item.id]">
              <img src="assets/images/view-icon-green.svg" alt="">
              <span>{{ 'ACTIONS.VIEW' | translate }}</span>
            </a></li>
            <li><a class="dropdown-item" [routerLink]="[item.id, 'edit']">
              <img src="assets/images/edit-icon-green.svg" alt="">
              <span>{{ 'ACTIONS.EDIT' | translate }}</span>
            </a></li>
            <li><a class="dropdown-item" (click)="onDelete(item.id)">
              <img src="assets/images/delete-icon-green.svg" alt="">
              <span>{{ 'ACTIONS.DELETE' | translate }}</span>
            </a></li>
          </ul>
        </div>
      </div>
      <div class="indicator-card__body">
        <h3 class="indicator-card__title">{{ item.name }}</h3>
        <div class="indicator-card__details">
          <div class="indicator-card__details__item">
            <span class="indicator-card__details__item__label">{{ 'FIELDS.CODE' | translate }}</span>
            <span class="indicator-card__details__item__data">{{ item.code }}</span>
          </div>
          <div class="indicator-card__details__item">
            <span class="indicator-card__details__item__label">{{ 'FIELDS.ORDER' | translate }}</span>
            <span class="indicator-card__details__item__data">{{ item.order }}</span>
          </div>
        </div>
      </div>
    </div>
  </div>
</div>

<!-- Pagination - from indicators.html:1144-1180 -->
<div class="row">
  <div class="col-12 d-flex justify-content-center mt-5">
    <nav aria-label="Page navigation">
      <ul class="pagination">
        <!-- Implement pagination logic -->
      </ul>
    </nav>
  </div>
</div>

<!-- Loading Preloader - from indicators.html:1197-1208 -->
<div class="page-preloader" *ngIf="loading">
  <div class="loading-bar">
    <div class="loading-line"></div>
    <div class="loading-line"></div>
  </div>
  <div class="loading-message">
    <div class="loading-spinner"></div>
    <div class="loading-logo">
      <img src="assets/images/logo-icon.svg" alt="">
    </div>
  </div>
</div>
```

#### Step 5: Create Form Component (Create/Edit)

**Location:** `src/app/features/{feature-name}/{entity-name}-form/{entity-name}-form.component.ts`

```typescript
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { {EntityName}Service } from '../services/{entity-name}.service';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-{entity-name}-form',
  templateUrl: './{entity-name}-form.component.html',
  styleUrls: ['./{entity-name}-form.component.scss']
})
export class {EntityName}FormComponent implements OnInit {
  form!: FormGroup;
  isEditMode = false;
  itemId?: number;
  loading = false;

  constructor(
    private fb: FormBuilder,
    private service: {EntityName}Service,
    private route: ActivatedRoute,
    private router: Router,
    public translate: TranslateService
  ) { }

  ngOnInit(): void {
    this.initForm();

    this.route.params.subscribe(params => {
      if (params['id']) {
        this.isEditMode = true;
        this.itemId = +params['id'];
        this.loadItem(this.itemId);
      }
    });
  }

  initForm(): void {
    this.form = this.fb.group({
      code: ['', [Validators.required, Validators.maxLength(50)]],
      nameAr: ['', [Validators.required, Validators.maxLength(250)]],
      nameEn: ['', [Validators.required, Validators.maxLength(250)]],
      descriptionAr: ['', Validators.maxLength(2000)],
      descriptionEn: ['', Validators.maxLength(2000)],
      order: [0, [Validators.required, Validators.min(0)]],
      isActive: [true],
      parentId: [null]
    });
  }

  loadItem(id: number): void {
    this.loading = true;
    this.service.getById(id).subscribe({
      next: (item) => {
        this.form.patchValue(item);
        this.form.get('code')?.disable(); // Code is immutable per PRD
        this.loading = false;
      },
      error: (err) => {
        console.error('Error loading item:', err);
        this.loading = false;
      }
    });
  }

  onSubmit(): void {
    if (this.form.invalid) return;

    this.loading = true;
    const formValue = this.form.getRawValue();

    const observable = this.isEditMode && this.itemId
      ? this.service.update(this.itemId, formValue)
      : this.service.create(formValue);

    observable.subscribe({
      next: () => {
        this.router.navigate(['..'], { relativeTo: this.route });
      },
      error: (err) => {
        console.error('Save failed:', err);
        this.loading = false;
      }
    });
  }

  onCancel(): void {
    this.router.navigate(['..'], { relativeTo: this.route });
  }
}
```

**Template:** `{entity-name}-form.component.html`

```html
<div class="content-container">
  <form [formGroup]="form" (ngSubmit)="onSubmit()">
    <div class="row g-3">
      <!-- Code -->
      <div class="col-12 col-md-6">
        <label class="form-label">{{ 'FIELDS.CODE' | translate }} *</label>
        <input type="text" class="form-control" formControlName="code"
               [class.is-invalid]="form.get('code')?.invalid && form.get('code')?.touched">
        <div class="invalid-feedback" *ngIf="form.get('code')?.errors?.['required']">
          {{ 'VALIDATION.REQUIRED' | translate }}
        </div>
      </div>

      <!-- Name Arabic -->
      <div class="col-12 col-md-6">
        <label class="form-label">{{ 'FIELDS.NAME_AR' | translate }} *</label>
        <input type="text" class="form-control" formControlName="nameAr"
               [class.is-invalid]="form.get('nameAr')?.invalid && form.get('nameAr')?.touched">
        <div class="invalid-feedback" *ngIf="form.get('nameAr')?.errors?.['required']">
          {{ 'VALIDATION.REQUIRED' | translate }}
        </div>
      </div>

      <!-- Name English -->
      <div class="col-12 col-md-6">
        <label class="form-label">{{ 'FIELDS.NAME_EN' | translate }} *</label>
        <input type="text" class="form-control" formControlName="nameEn"
               [class.is-invalid]="form.get('nameEn')?.invalid && form.get('nameEn')?.touched">
        <div class="invalid-feedback" *ngIf="form.get('nameEn')?.errors?.['required']">
          {{ 'VALIDATION.REQUIRED' | translate }}
        </div>
      </div>

      <!-- Description Arabic -->
      <div class="col-12">
        <label class="form-label">{{ 'FIELDS.DESCRIPTION_AR' | translate }}</label>
        <textarea class="form-control" formControlName="descriptionAr" rows="3"></textarea>
      </div>

      <!-- Description English -->
      <div class="col-12">
        <label class="form-label">{{ 'FIELDS.DESCRIPTION_EN' | translate }}</label>
        <textarea class="form-control" formControlName="descriptionEn" rows="3"></textarea>
      </div>

      <!-- Order -->
      <div class="col-12 col-md-6">
        <label class="form-label">{{ 'FIELDS.ORDER' | translate }} *</label>
        <input type="number" class="form-control" formControlName="order" min="0">
      </div>

      <!-- Is Active -->
      <div class="col-12 col-md-6">
        <div class="form-check form-switch mt-4">
          <input class="form-check-input" type="checkbox" formControlName="isActive" id="isActive">
          <label class="form-check-label" for="isActive">
            {{ 'FIELDS.IS_ACTIVE' | translate }}
          </label>
        </div>
      </div>

      <!-- Actions -->
      <div class="col-12 d-flex gap-3 mt-4">
        <button type="submit" class="btn btn-primary" [disabled]="form.invalid || loading">
          {{ (isEditMode ? 'ACTIONS.UPDATE' : 'ACTIONS.CREATE') | translate }}
        </button>
        <button type="button" class="btn btn-outline-secondary" (click)="onCancel()">
          {{ 'ACTIONS.CANCEL' | translate }}
        </button>
      </div>
    </div>
  </form>
</div>
```

#### Step 6: Create i18n Translation Files

**Location:** `src/app/features/{feature-name}/i18n/ar.json`

```json
{
  "BREADCRUMB": {
    "VISION_CENTER": "مركز معلومات رؤية 2030"
  },
  "STATISTICS": {
    "TITLE": "إحصائيات المؤشرات"
  },
  "TABLE": {
    "CODE": "الرمز",
    "NAME": "الاسم",
    "ORDER": "الترتيب",
    "STATUS": "الحالة",
    "ACTIONS": "الإجراء"
  },
  "FIELDS": {
    "CODE": "الرمز",
    "NAME_AR": "الاسم بالعربية",
    "NAME_EN": "الاسم بالإنجليزية",
    "DESCRIPTION_AR": "الوصف بالعربية",
    "DESCRIPTION_EN": "الوصف بالإنجليزية",
    "ORDER": "الترتيب",
    "IS_ACTIVE": "نشط"
  },
  "ACTIONS": {
    "VIEW": "عرض",
    "EDIT": "تعديل",
    "DELETE": "حذف",
    "CREATE": "إضافة",
    "UPDATE": "تحديث",
    "CANCEL": "إلغاء"
  },
  "VALIDATION": {
    "REQUIRED": "هذا الحقل مطلوب"
  },
  "SEARCH": {
    "PLACEHOLDER": "ادخل للبحث..."
  },
  "ACTIVE": "نشط",
  "INACTIVE": "غير نشط",
  "CONFIRM_DELETE": "هل أنت متأكد من الحذف؟"
}
```

**Location:** `src/app/features/{feature-name}/i18n/en.json`

```json
{
  "BREADCRUMB": {
    "VISION_CENTER": "Vision 2030 Information Center"
  },
  "STATISTICS": {
    "TITLE": "Indicators Statistics"
  },
  "TABLE": {
    "CODE": "Code",
    "NAME": "Name",
    "ORDER": "Order",
    "STATUS": "Status",
    "ACTIONS": "Actions"
  },
  "FIELDS": {
    "CODE": "Code",
    "NAME_AR": "Arabic Name",
    "NAME_EN": "English Name",
    "DESCRIPTION_AR": "Arabic Description",
    "DESCRIPTION_EN": "English Description",
    "ORDER": "Order",
    "IS_ACTIVE": "Active"
  },
  "ACTIONS": {
    "VIEW": "View",
    "EDIT": "Edit",
    "DELETE": "Delete",
    "CREATE": "Create",
    "UPDATE": "Update",
    "CANCEL": "Cancel"
  },
  "VALIDATION": {
    "REQUIRED": "This field is required"
  },
  "SEARCH": {
    "PLACEHOLDER": "Search..."
  },
  "ACTIVE": "Active",
  "INACTIVE": "Inactive",
  "CONFIRM_DELETE": "Are you sure you want to delete?"
}
```

#### Step 7: Configure Routing

**Location:** `src/app/features/{feature-name}/{feature-name}-routing.module.ts`

```typescript
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { {EntityName}ListComponent } from './{entity-name}-list/{entity-name}-list.component';
import { {EntityName}FormComponent } from './{entity-name}-form/{entity-name}-form.component';
import { {EntityName}DetailsComponent } from './{entity-name}-details/{entity-name}-details.component';

const routes: Routes = [
  { path: '', component: {EntityName}ListComponent },
  { path: 'create', component: {EntityName}FormComponent },
  { path: ':id', component: {EntityName}DetailsComponent },
  { path: ':id/edit', component: {EntityName}FormComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class {FeatureName}RoutingModule { }
```

#### Step 8: Create Frontend Unit Tests

**Location:** `src/app/features/{feature-name}/{entity-name}-list/{entity-name}-list.component.spec.ts`

```typescript
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { TranslateModule } from '@ngx-translate/core';
import { {EntityName}ListComponent } from './{entity-name}-list.component';
import { {EntityName}Service } from '../services/{entity-name}.service';
import { of } from 'rxjs';

describe('{EntityName}ListComponent', () => {
  let component: {EntityName}ListComponent;
  let fixture: ComponentFixture<{EntityName}ListComponent>;
  let service: jasmine.SpyObj<{EntityName}Service>;

  beforeEach(async () => {
    const serviceSpy = jasmine.createSpyObj('{EntityName}Service', ['getAll', 'delete', 'toggleActive']);

    await TestBed.configureTestingModule({
      declarations: [ {EntityName}ListComponent ],
      imports: [ HttpClientTestingModule, TranslateModule.forRoot() ],
      providers: [
        { provide: {EntityName}Service, useValue: serviceSpy }
      ]
    }).compileComponents();

    service = TestBed.inject({EntityName}Service) as jasmine.SpyObj<{EntityName}Service>;
  });

  beforeEach(() => {
    fixture = TestBed.createComponent({EntityName}ListComponent);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load items on init', () => {
    const mockItems = [
      { id: 1, code: 'TEST001', nameAr: 'اختبار', nameEn: 'Test', name: 'Test', order: 1, isActive: true }
    ];
    service.getAll.and.returnValue(of(mockItems));

    component.ngOnInit();

    expect(service.getAll).toHaveBeenCalled();
    expect(component.items.length).toBe(1);
  });

  it('should toggle view mode', () => {
    expect(component.viewMode).toBe('table');
    component.toggleViewMode();
    expect(component.viewMode).toBe('grid');
    component.toggleViewMode();
    expect(component.viewMode).toBe('table');
  });

  it('should delete item when confirmed', () => {
    spyOn(window, 'confirm').and.returnValue(true);
    service.delete.and.returnValue(of(void 0));
    service.getAll.and.returnValue(of([]));

    component.onDelete(1);

    expect(service.delete).toHaveBeenCalledWith(1);
  });
});
```

### Phase 4: E2E Testing (Cypress)

**Location:** `cypress/e2e/{entity-name}.cy.ts`

```typescript
describe('{EntityName} CRUD', () => {
  beforeEach(() => {
    // Login first
    cy.login('admin', 'Admin@123');
    cy.visit('/{feature-route}');
  });

  it('should display list of items', () => {
    cy.get('table tbody tr').should('have.length.greaterThan', 0);
  });

  it('should create new item', () => {
    cy.get('button:contains("إضافة")').click();

    cy.get('input[formControlName="code"]').type('TEST001');
    cy.get('input[formControlName="nameAr"]').type('اختبار');
    cy.get('input[formControlName="nameEn"]').type('Test');
    cy.get('input[formControlName="order"]').clear().type('1');

    cy.get('button[type="submit"]').click();

    cy.url().should('include', '/{feature-route}');
    cy.contains('TEST001').should('exist');
  });

  it('should edit existing item', () => {
    cy.get('table tbody tr').first().within(() => {
      cy.get('.dropdown-toggle').click();
    });
    cy.contains('تعديل').click();

    cy.get('input[formControlName="nameAr"]').clear().type('تعديل اختبار');
    cy.get('button[type="submit"]').click();

    cy.contains('تعديل اختبار').should('exist');
  });

  it('should delete item', () => {
    cy.get('table tbody tr').first().within(() => {
      cy.get('.dropdown-toggle').click();
    });
    cy.contains('حذف').click();

    cy.on('window:confirm', () => true);

    // Verify item is removed
    cy.wait(500);
  });

  it('should switch between table and grid views', () => {
    cy.get('.view-btns__item').eq(1).click();
    cy.get('.indicator-card').should('exist');

    cy.get('.view-btns__item').eq(0).click();
    cy.get('table').should('exist');
  });

  it('should search items', () => {
    cy.get('.search-control').type('TEST001{enter}');
    cy.get('table tbody tr').should('have.length.lessThan', 10);
  });

  it('should support RTL layout for Arabic', () => {
    cy.get('html').should('have.attr', 'dir', 'rtl');
    cy.get('html').should('have.attr', 'lang', 'ar');
  });
});
```

### Phase 5: Integration & Finalization

#### Step 1: Copy SMO-UI Assets to Angular (One-time setup)

```bash
# From project root
cp -r SMO-Platform-UI/css src/SMO.Frontend/SMO-Portal/src/assets/
cp -r SMO-Platform-UI/fonts src/SMO.Frontend/SMO-Portal/src/assets/
cp -r SMO-Platform-UI/images src/SMO.Frontend/SMO-Portal/src/assets/
```

#### Step 2: Configure angular.json to include SMO-UI styles

**Location:** `src/SMO.Frontend/SMO-Portal/angular.json`

```json
{
  "styles": [
    "src/assets/css/bootstrap.rtl.min.css",
    "src/assets/css/bootstrap-icons.min.css",
    "src/assets/css/styles.min.css",
    "src/styles.scss"
  ],
  "scripts": [
    "src/assets/js/bootstrap.min.js"
  ]
}
```

#### Step 3: Configure RTL/LTR Service

**Location:** `src/app/core/services/rtl.service.ts`

```typescript
import { Injectable } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';

@Injectable({
  providedIn: 'root'
})
export class RtlService {
  constructor(private translate: TranslateService) {}

  setLanguage(lang: 'ar' | 'en'): void {
    this.translate.use(lang);

    const htmlTag = document.documentElement;
    if (lang === 'ar') {
      htmlTag.setAttribute('dir', 'rtl');
      htmlTag.setAttribute('lang', 'ar');
      this.loadRtlStyles();
    } else {
      htmlTag.setAttribute('dir', 'ltr');
      htmlTag.setAttribute('lang', 'en');
      this.loadLtrStyles();
    }
  }

  private loadRtlStyles(): void {
    this.updateStylesheet('bootstrap', 'assets/css/bootstrap.rtl.min.css');
  }

  private loadLtrStyles(): void {
    this.updateStylesheet('bootstrap', 'assets/css/bootstrap.min.css');
  }

  private updateStylesheet(id: string, href: string): void {
    let link = document.getElementById(id) as HTMLLinkElement;
    if (!link) {
      link = document.createElement('link');
      link.id = id;
      link.rel = 'stylesheet';
      document.head.appendChild(link);
    }
    link.href = href;
  }
}
```

#### Step 4: Create Git Commit

```bash
git add .
git commit -m "UC-{XX} Complete: {Use Case Name}

Backend:
- Created {EntityName} entity with FullAuditedEntityBase
- EF Core configuration with indexes
- Application service with IRepository<T> injection
- API controller with CRUD endpoints
- FluentValidation validators
- xUnit unit tests (X tests)

Frontend:
- Angular module and routing
- List component with table/grid views (SMO-UI patterns)
- Form component with bilingual validation
- TypeScript models and HTTP service
- i18n translation files (ar/en)
- Jasmine unit tests (X tests)

E2E:
- Cypress tests covering CRUD operations
- RTL/LTR layout testing

Migration:
- Add_{EntityNamePlural} migration applied

🤖 Generated with [Claude Code](https://claude.com/claude-code)

Co-Authored-By: Claude <noreply@anthropic.com>"
```

## Summary Checklist

When implementing a user story, ensure ALL of these are completed:

### Backend (.NET 8)
- [ ] Domain entity created (inheriting from FullAuditedEntityBase or LookupEntityBase)
- [ ] EF Core configuration with proper indexes
- [ ] Registered in AppDbContext
- [ ] DTOs created (List, Create, Update)
- [ ] AutoMapper profile configured
- [ ] FluentValidation validators (bilingual messages)
- [ ] Application service using IRepository<T> + IUnitOfWork
- [ ] API controller injecting AppService only
- [ ] Service registered in DI
- [ ] EF migration created and applied
- [ ] xUnit unit tests written

### Frontend (Angular 18)
- [ ] Feature module generated
- [ ] TypeScript models created
- [ ] HTTP service with all CRUD methods
- [ ] List component (table + grid views using SMO-UI patterns)
- [ ] Form component (create + edit)
- [ ] Details component (if required by PRD)
- [ ] i18n translation files (ar.json + en.json)
- [ ] Routing configured
- [ ] Jasmine unit tests written

### E2E Testing
- [ ] Cypress test suite covering CRUD operations
- [ ] RTL/LTR layout validation
- [ ] Search and filter testing

### Assets & Styling
- [ ] SMO-UI CSS/SASS referenced
- [ ] Arabic fonts configured
- [ ] Icons and images available
- [ ] RTL stylesheet loaded for Arabic

### Git & Documentation
- [ ] Git commit with detailed message
- [ ] Todo list updated (all tasks completed)

## Important Notes

1. **NEVER skip auditing** - All entities MUST inherit from FullAuditedEntityBase or LookupEntityBase
2. **NEVER inject repositories in controllers** - Controllers inject AppServices only
3. **ALWAYS use IRepository&lt;T&gt; by default** - Only create custom repositories when needed
4. **ALWAYS use TableNoTracking for read-only queries** - Performance critical
5. **ALWAYS include bilingual support** - NameAr, NameEn, i18n translations
6. **ALWAYS follow SMO-UI patterns** - Reference HTML files for exact structure
7. **ALWAYS write unit tests** - Backend (xUnit) and Frontend (Jasmine)
8. **ALWAYS write E2E tests** - Cypress for full user journeys
9. **ALWAYS commit after each user story** - With comprehensive commit message

## Skill Invocation Examples

**Example 1: Implement UC-04**
```
User: "Implement UC-04 from PRD: عرض قائمة الأهداف الإستراتيجية"

Skill:
1. Reads docs/PRD.md for UC-04 details
2. Creates todo list (20+ tasks)
3. Generates StrategicObjective entity with 5 levels
4. Generates AppService, Controller, DTOs
5. Generates Angular list component using indicators.html patterns
6. Generates all unit tests and E2E tests
7. Creates migration
8. Commits with message
```

**Example 2: Implement UC-26**
```
User: "Implement UC-26: عرض قائمة المؤشرات"

Skill:
1. Analyzes PRD for KPI entity structure
2. Generates backend with formula calculation support
3. Uses indicators.html table pattern for frontend
4. Includes progress bars and currency formatting
5. Tests cover formula validation
6. Commits with comprehensive message
```

## End of Skill

This skill provides COMPLETE full-stack implementation guidance following all SMO project conventions, Clean Architecture principles, and SMO-Platform-UI design patterns.