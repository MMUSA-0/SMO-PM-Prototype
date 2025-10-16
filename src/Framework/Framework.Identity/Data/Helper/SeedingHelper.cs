using Framework.Identity.Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace Framework.Identity.Data.Helper
{
    public static class SeedingHelper
    {
        public static Guid AdminId { get; set; } = new Guid("4684F03A-D163-4E92-AF57-069771C31E97");
        public static Guid learningPartnerId { get; set; } = Guid.Parse("A3B0E143-6F4D-4C5B-9E2D-01D1D78D1111");
        public static Guid coachesId { get; set; } = Guid.Parse("B4C1F254-7A5E-4D6F-AF2E-02E2E89E2222");
        public static Guid participantId { get; set; } = Guid.Parse("C5D2F365-8B6F-4E7F-BF3E-03F3F90F3333");
        public static Guid trackDicetorId { get; set; } = Guid.Parse("f6fe3d61-e40e-4c5d-b428-6e1711525563");

        public static string PassGenerate(ApplicationUser user)
        {
            var passHash = new PasswordHasher<ApplicationUser>();
            return passHash.HashPassword(user, "P@ssw0rd");

        }
    }
}
