using Framework.Core.Data;
using System;

namespace Framework.Identity.Data.Dtos
{
    public class UserSearchDto : PagingDto
    {
        public string? PhoneNumber { get; set; }
        public string? UserName { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public Guid? RoleId { get; set; }
        public string? ActivationStatus { get; set; }
        //public bool? IsOrderAsc { get; set; }
    }
}
