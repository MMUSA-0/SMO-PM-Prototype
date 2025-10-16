using System;
using System.Collections.Generic;
using Framework.Core.Data;

namespace Framework.Identity.Data.Dtos
{
    public class UsersListDto : EntityDto<Guid>
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsActive { get; set; }
        public IList<string> RoleNames { get; set; }
    }
}