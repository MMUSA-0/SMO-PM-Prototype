
using System;

namespace Framework.Identity.Data.Dtos
{
    public class EmailMobileVerificationDto
    {
        public string? Email { get; set; }
        public string? Mobile { get; set; }
        public string Code { get; set; }
    }
}
