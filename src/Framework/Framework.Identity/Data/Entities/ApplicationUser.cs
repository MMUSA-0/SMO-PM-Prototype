using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Framework.Core;
using Framework.Core.Extensions;
using Microsoft.AspNetCore.Identity;

namespace Framework.Identity.Data.Entities
{

    public class ApplicationUser : IdentityUser<Guid>
    {
        public ApplicationUser()
        {

        }
        public ApplicationUser(string userName, string fullNameEn, 
            string fullNameAr, string titleEn, string titleAr, 
            string? email = null, DateOnly? dateOfBirth = null)
        {
            Check.NotNull(userName, nameof(userName));

            Id = Guid.NewGuid().AsSequentialGuid();
            UserName = userName.ToLower();
            FullNameEn = fullNameEn;
            FullNameAr = fullNameAr;
            TitleEn = titleEn;
            TitleAr = titleAr;
            DateOfBirth = dateOfBirth;
            NormalizedUserName = userName.ToUpperInvariant();
            Email = email?.ToLower();
            NormalizedEmail = email?.ToUpperInvariant();
            SecurityStamp = Guid.NewGuid().ToString();
            IsActive = true;
        }
        public string FullNameEn { get; set; }
        public string FullNameAr { get; set; }
        public string TitleEn { get; set; }
        public string TitleAr { get; set; }
        public DateOnly? DateOfBirth { get; set; }

        public bool IsActive { get; set; }
        public string? IdentityNo { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        
        public List<ApplicationUserRoles> UserRoles { get; set; }
    }
}
