
using Framework.Core.Data.Repositories;
using Framework.Identity.Data.Entities;

namespace Framework.Identity.Data.Repositories
{
    public class UserOtpRepository : RepositoryBase<AppIdentityDbContext, UserOtp>
    {
        public UserOtpRepository(AppIdentityDbContext dbContext) : base(dbContext)
        {

        }
    }
}
