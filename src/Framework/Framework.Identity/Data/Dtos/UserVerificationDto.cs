
using System;

namespace Framework.Identity.Data.Dtos
{
    public class UserVerificationDto
    {
        public string UserName { get; set; }
        public string Code { get; set; }
        public string Token { get; set; }
    }
}
