using System.ComponentModel.DataAnnotations.Schema;
using Framework.Core.Helpers;

namespace Framework.Core.Data;

/// <summary>
/// Abstract base class for lookup/reference data entities.
/// Extends FullAuditedEntityBase with bilingual name support, active status, and ordering.
/// </summary>
/// <typeparam name="TKey">The type of the entity's primary key (e.g., int, Guid, long)</typeparam>
/// <remarks>
/// Usage Example:
/// <code>
/// public class ProgramType : LookupEntityBase&lt;int&gt;
/// {
///     // Only add domain-specific properties here
///     // NameAr, NameEn, IsActive, Order, and audit fields are inherited
/// }
/// </code>
///
/// Inherited Features:
/// - Bilingual Support: NameAr (Arabic) and NameEn (English) properties
/// - Computed Name: Automatically returns NameAr or NameEn based on current culture
/// - Active Status: IsActive flag for soft deletion (default: true)
/// - Ordering: Optional Order property for display sequence
/// - Full Audit Trail: CreatedBy, CreatedOn, UpdatedBy, UpdatedOn
///
/// Global Query Filter:
/// - AppDbContext automatically applies .Where(x => x.IsActive) to all queries
/// - Use IgnoreQueryFilters() to include inactive records
///
/// This class should be used for:
/// - Static lookup tables (Status, Type, Category, etc.)
/// - Reference data with bilingual names
/// - Master data that requires soft deletion
///
/// DO NOT use for:
/// - Transactional entities (use FullAuditedEntityBase instead)
/// - Entities without bilingual requirements (use FullAuditedEntityBase instead)
/// </remarks>
public abstract class LookupEntityBase<TKey> : FullAuditedEntityBase<TKey>
{
    /// <summary>
    /// Gets or sets the Arabic name of the lookup entity.
    /// Required field for bilingual support.
    /// </summary>
    public string NameAr { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the English name of the lookup entity.
    /// Required field for bilingual support.
    /// </summary>
    public string NameEn { get; set; } = string.Empty;

    /// <summary>
    /// Gets the localized name based on the current culture.
    /// Returns NameAr if current culture is Arabic, otherwise returns NameEn.
    /// This property is not mapped to the database (computed at runtime).
    /// </summary>
    [NotMapped]
    public string Name => CultureHelper.IsArabic ? NameAr : NameEn;

    /// <summary>
    /// Gets or sets whether this lookup entity is active.
    /// Default value is true.
    /// Inactive records are automatically filtered by AppDbContext global query filter.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the display order for this lookup entity.
    /// Lower values appear first. Null values are typically sorted last.
    /// Used for controlling the display sequence in dropdowns and lists.
    /// </summary>
    public int? Order { get; set; }
}
