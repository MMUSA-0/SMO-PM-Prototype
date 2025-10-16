using System.ComponentModel.DataAnnotations;
using Framework.Core.Data;
using Framework.Identity.Data.Entities;
using Framework.Identity.Recources;
using PagedList.Core;

namespace Framework.Identity.Data.Dtos
{
    public class RoleSearchDto : PagingDto
    {
        [Display(Name = "RoleName", ResourceType = typeof(IdentityResources))]
        public string Name { get; set; }
        [Display(Name = "RoleGroup", ResourceType = typeof(IdentityResources))]
        public string Group { get; set; }

        public new StaticPagedList<ApplicationRole> Items { get; set; }


    }
}
