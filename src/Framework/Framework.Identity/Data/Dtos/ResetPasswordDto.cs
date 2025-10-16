
using System;

namespace Framework.Identity.Data.Dtos
{
    public class ResetPasswordDto
    {
        public string Token { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
