using System;
using System.Collections.Generic;
using Framework.Core.Data;
using Framework.Core.Globalization;
using Microsoft.AspNetCore.Mvc;

namespace Framework.Identity.Data.Dtos
{
    public class RoleDto : EntityDto<Guid>
    {
        [HiddenInput]
        public string Name { get; set; }
        public int Code { get; set; }
        public string Description => CultureHelper.IsArabic ? DescriptionAr : DescriptionEn;
        public string DescriptionAr { get; set; }
        public string DescriptionEn { get; set; }
        public string DisplayName => CultureHelper.IsArabic ? DisplayNameAr : DisplayNameEn;
        public string DisplayNameAr { get; set; }
        public string DisplayNameEn { get; set; }
        public string RoleGroup { get; set; }
        public List<UserDto> Users { get; set; }

    }
}