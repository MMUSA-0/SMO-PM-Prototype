using System;
using Framework.Core.Data;
using PagedList.Core;

namespace Framework.Identity.Data.Dtos
{
    public class UserRolesSearchDto : PagingDto
    {
        public Guid? UserId { get; set; }
        public Guid? RoleId { get; set; }
        public int TotalItemsCount { get; set; }
        public new StaticPagedList<UserRolesDto> Items { get; set; }
    }
}
