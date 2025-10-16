using Framework.Identity.Data.Dtos;
using Framework.Identity.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Framework.Identity.Data.Services.Interfaces
{
    public interface IRoleAppService
    {
        public Task<RoleDto> GetAsync(Guid id);

        public Task<RoleDto> FindByRoleNameAsync(string roleName);

        public RoleSearchDto SearchList(RoleSearchDto model);
        public  Task<IEnumerable<SelectListItem>> List();
        public Task<IEnumerable<RoleDto>> GetEmployeesRoleList();

        public Task DeleteAsync(Guid id);

        public Task<bool> IsRoleInRoleGroup(string roleName, string roleGroup);

        public Task<bool> IsRoleInRoleGroup(Guid roleId, string roleGroup);

        Task<List<ApplicationRole>> GetRoleByNames(IList<string> RoleNames);
        Task<List<ApplicationRole>> SearchByName(string keyword);
    }
}
