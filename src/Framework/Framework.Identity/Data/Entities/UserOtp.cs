using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Identity.Data.Entities
{
    public class UserOtp
    {
        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public string? Email { get; set; }
        public string? Mobile { get; set; }
        public ApplicationUser User { get; set; }
        public string Otp { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
}
