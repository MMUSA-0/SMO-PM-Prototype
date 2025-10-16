using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Framework.Identity.Data.Entities
{
    /// <summary>
    /// Represents a role in the identity system
    /// </summary>
    public class ApplicationRole : IdentityRole<Guid>
    {
        public int Code { get; set; }
        public string DescriptionAr { get; set; }
        public string DescriptionEn { get; set; }
        public string DisplayNameAr { get; set; }
        public string DisplayNameEn { get; set; }
        public string? RoleGroup { get; set; }

        public virtual bool IsDefault { get; set; }

        [Column(Order = 300)]
        public string CreatedBy { get; set; }

        [Column(Order = 301)]
        public DateTime CreatedOn { get; set; }
        [Column(Order = 302)]

        public string? UpdatedBy { get; set; }
        [Column(Order = 303)]
        public DateTime? UpdatedOn { get; set; }

        public List<ApplicationUserRoles> UserRoles { get; set; }
    }
}
