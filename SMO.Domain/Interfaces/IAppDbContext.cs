using Framework.Core.Data;

namespace SMO.Domain.Interfaces;

/// <summary>
/// Application-specific database context interface for the SMO Vision Center.
/// Inherits base database operations and defines domain-specific DbSets.
/// </summary>
/// <remarks>
/// Design Purpose:
/// - Abstraction layer between Application/Domain layers and Infrastructure
/// - Enables Clean Architecture dependency inversion
/// - Facilitates unit testing (can be mocked in tests)
/// - Documents all entity collections available in the system
///
/// Architecture Notes:
/// - This interface is defined in SMO.Domain (domain layer)
/// - Implementation (AppDbContext) is in SMO.Infrastructure (infrastructure layer)
/// - Application services depend on this interface, NOT the concrete implementation
/// - Infrastructure layer implements this interface and provides concrete DbContext
///
/// Usage Example:
/// <code>
/// // In Application Service
/// public class StrategicObjectiveAppService
/// {
///     private readonly IUnitOfWork _unitOfWork;
///
///     // IUnitOfWork uses IAppDbContext internally
///     public StrategicObjectiveAppService(IUnitOfWork unitOfWork)
///     {
///         _unitOfWork = unitOfWork;
///     }
///
///     public async Task&lt;List&lt;StrategicObjective&gt;&gt; GetAllAsync()
///     {
///         return await _unitOfWork.Repository&lt;StrategicObjective&gt;().GetAsync();
///     }
/// }
/// </code>
///
/// Future Enhancement:
/// - DbSet properties will be added in future stories when domain entities are created
/// - Example: DbSet&lt;StrategicObjective&gt; StrategicObjectives { get; set; }
/// - Example: DbSet&lt;Initiative&gt; Initiatives { get; set; }
/// - Example: DbSet&lt;KPI&gt; KPIs { get; set; }
///
/// Current State (Story 0.0):
/// - Interface defines contract but no DbSet properties yet
/// - DbSets will be added as entities are created in subsequent stories
/// - Generic Set&lt;TEntity&gt;() method (inherited from IBaseDbContext) provides access to any entity type
/// </remarks>
public interface IAppDbContext : IBaseDbContext
{
    // NOTE: DbSet properties will be added in future stories as domain entities are created
    // This ensures the interface reflects the actual domain model

    // Example DbSet declarations (to be added in Story 1.x+):
    // DbSet<Pillar> Pillars { get; set; }
    // DbSet<Theme> Themes { get; set; }
    // DbSet<StrategicObjective> StrategicObjectives { get; set; }
    // DbSet<VisionProgram> VisionPrograms { get; set; }
    // DbSet<Initiative> Initiatives { get; set; }
    // DbSet<InitiativeMilestone> InitiativeMilestones { get; set; }
    // DbSet<KPI> KPIs { get; set; }
    // DbSet<KPITarget> KPITargets { get; set; }
    // DbSet<KPIActual> KPIActuals { get; set; }
    // DbSet<Attachment> Attachments { get; set; }
    // And many more...
}
