using System;
using System.Collections.Generic;

namespace Framework.Identity.Data.Dtos
{
    public class ADUserCreateDto : UserCreateOrUpdateDtoBase
    {
        public string UserName { get; set; }
        public string FullName { get; set; }
        public bool IsActive { get; set; }
        public string NationalId { get; set; }
        public string TelephoneNumber { get; set; }
        public string JobTitle { get; set; }
        public string SurName { get; set; }
        public string Description { get; set; }
        public string GivenName { get; set; }
        public string DisplayName { get; set; }
        public string Name { get; set; }
        public string UserPrincipalName { get; set; }
        public Guid? UserId { get; set; }
    }
}