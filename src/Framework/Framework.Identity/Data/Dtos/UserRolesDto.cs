using System;

namespace Framework.Identity.Data.Dtos
{
    public class UserRolesDto 
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public Guid RoleId { get; set; }
        public string RoleName { get; set; }
        public string CreatedBy { get; set; }
    }
}
