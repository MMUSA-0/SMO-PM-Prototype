// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemSetting.cs" company="Usama Nada">
//   No Copyright .. Copy, Share, and Evolve.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Framework.Core.Data;
using System.ComponentModel.DataAnnotations;

namespace Framework.Core.SharedServices.Entities
{
    public class SystemSetting : FullAuditedEntityBase<int>
    {
        [StringLength(100)]
        [MaxLength(100)]
        public string Name { get; set; }
        [StringLength(50)]
        [MaxLength(50)]
        public string ValueType { get; set; }
        [StringLength(100)]
        [MaxLength(100)]
        public string Value { get; set; }
        [StringLength(100)]
        [MaxLength(100)]
        public string GroupName { get; set; }
        public bool IsSecure { get; set; }
        public bool IsSticky { get; set; }
        public bool IsActive { get; set; }
    }
}