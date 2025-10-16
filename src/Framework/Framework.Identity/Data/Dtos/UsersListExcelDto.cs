using System;
using System.Collections.Generic;
using Framework.Core.Data;

namespace Framework.Identity.Data.Dtos
{
    public class UsersListExcelDto 
    {
        public Guid? Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string RoleName { get; set; }
        public string Status { get; set; }
    }
}