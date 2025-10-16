using System.Collections.Generic;
using System.Threading.Tasks;
using Framework.Core.Data.Repositories;
using Framework.Identity.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Framework.Identity.Data.Repositories
{
    public class UserRolesRepository : RepositoryBase<AppIdentityDbContext, ApplicationUserRoles>
    {
        public UserRolesRepository(AppIdentityDbContext dbContext): base(dbContext)
        {

        }
        public virtual async Task<List<ApplicationUserRoles>> GetListAsync()
        {
            return await TableNoTracking.ToListAsync();
        }
        public async Task<ApplicationUserRoles> AddAsync(ApplicationUserRoles userRoles)
        {
            ApplicationUserRoles result = await InsertAsync(userRoles, true);
            return result;
        }
    }
}
