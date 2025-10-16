

namespace Framework.Identity.Data.Dtos
{
    public abstract class UserCreateOrUpdateDtoBase
    {
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string IdentityNo { get; set; }
    }
}