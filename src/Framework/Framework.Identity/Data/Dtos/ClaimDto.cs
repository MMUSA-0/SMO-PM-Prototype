using System;
using System.Collections.Generic;

namespace Framework.Identity.Data.Dtos
{
    public class ClaimDto
    {
        public List<ClaimUserRoleDto> UserRole { get; set; } = new List<ClaimUserRoleDto>();
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string FullNameAr { get; set; }
    }
}
