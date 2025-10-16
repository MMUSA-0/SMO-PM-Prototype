using Framework.Core.Globalization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Framework.Core.Data
{
    [Serializable]
    public abstract class EntityBase : IEntityBase
    {
        
    }

    [Serializable]
    public abstract class EntityBase<TKey> : EntityBase, IEntityBase<TKey>
    {
        [Column(Order = 0)]
        public TKey Id { get; set; }
    }

    [Serializable]
    public abstract class FullAuditedEntityBase : EntityBase
    {
        [StringLength(100)]
        [MaxLength(100)]
        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        [StringLength(100)]
        [MaxLength(100)]
        public string? UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

    }


    [Serializable]
    public abstract class FullAuditedEntityBase<TKey> : EntityBase<TKey>
    {
        //By default when creating a table with Migrations,
        //EF Core orders primary key columns first, followed by properties of the entity type and owned types,
        //and finally properties from base types.You can, however, specify a different column order:
        //[Column(Order = 300)]
        [StringLength(100)]
        [MaxLength(100)]
        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        [StringLength(100)]
        [MaxLength(100)]
        public string? UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

    }

    [Serializable]
    public class LookupEntityBase : FullAuditedEntityBase
    {
        [Column(Order = 0)]
        public int Id { get; set; }

        [Column(Order = 1)]
        [StringLength(250, MinimumLength = 1)]
        [MaxLength(250)]
        public string NameAr { get; set; }

        [Column(Order = 2)]
        [StringLength(250, MinimumLength = 1)]
        [MaxLength(250)]
        public string NameEn { get; set; }
        
        [NotMapped]
        public string Name { get { return CultureHelper.IsArabic ? this.NameAr : this.NameEn; } }

        public bool IsActive { get; set; } = true;
    }


    [Serializable]
    public abstract class LookupEntityBase<TKey> : FullAuditedEntityBase<TKey>
    {
        [Column(Order = 1)]
        [StringLength(250, MinimumLength = 1)]
        [MaxLength(250)]
        public string NameAr { get; set; }

        [Column(Order = 2)]
        [StringLength(250, MinimumLength = 1)]
        [MaxLength(250)]
        public string NameEn { get; set; }        

        [NotMapped]
        public string Name { get { return CultureHelper.IsArabic ? this.NameAr : this.NameEn; } }

        public bool IsActive { get; set; } = true;

        public int? Order { get; set; }
    }

}
