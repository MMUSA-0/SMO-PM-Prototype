using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Framework.Identity.Data.Entities
{
    public class ApplicationUserRoles : IdentityUserRole<Guid>
    {
        public Guid Id { get; set; }
        public ApplicationUser User { get; set; }
        public ApplicationRole Role { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
}
