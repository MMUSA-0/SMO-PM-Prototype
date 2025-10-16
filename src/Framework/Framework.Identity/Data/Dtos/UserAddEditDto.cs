
using Framework.Core.Data;

namespace Framework.Identity.Data.Dtos
{
    public class UserAddEditDto : EntityDto<Guid>
    {
        public string UserName { get; set; }
        public string FullNameEn { get; set; }
        public string FullNameAr { get; set; }
        public string TitleEn { get; set; }
        public string TitleAr { get; set; }
        public string Email { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string? Password { get; set; }
        public string? IdentityNo { get; set; }
        public int? IdentityTypeId { get; set; }
        public string PhoneNumber { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public bool LockoutEnabled { get; set; }
        public bool IsActive { get; set; }
        public IList<string> RoleNames { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
    }
}