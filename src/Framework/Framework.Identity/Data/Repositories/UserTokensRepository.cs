using System;
using Framework.Core.Data.Repositories;
using Microsoft.AspNetCore.Identity;

namespace Framework.Identity.Data.Repositories
{
    public class UserTokensRepository : RepositoryBase<AppIdentityDbContext, IdentityUserToken<Guid>>
    {
        public UserTokensRepository(AppIdentityDbContext dbContext) : base(dbContext)
        {

        }
       
    }
}
