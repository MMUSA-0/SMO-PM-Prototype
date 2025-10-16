using System;
using System.Collections.Generic;

namespace Framework.Identity.Data.Dtos
{
    public class LoginResult
    {
        public LoginStatusEnum LoginStatus { get; set; }
        public string VerificationCode { get; set; }
        public string UserName { get; set; }
        public string NationalId { get; set; }
        public string PhoneNumber { get; set; }
        public List<string> UserRoles { get; set; }
        public UserVM Token { get; set; }
    }
}
