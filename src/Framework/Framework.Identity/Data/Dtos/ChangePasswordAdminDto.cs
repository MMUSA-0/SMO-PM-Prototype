using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace Framework.Identity.Data.Dtos
{
    public class ChangePasswordAdminDto
    {
        [HiddenInput]
        public Guid UserId { get; set; }

        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }

    public class ChangePasswordRequestDto
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        [Compare("NewPassword")]
        public string ConfirmPassword { get; set; }
    }
}