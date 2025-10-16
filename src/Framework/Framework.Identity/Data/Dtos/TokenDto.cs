using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Identity.Data.Dtos
{
    public class TokenDto
    {
        public string UserId { get; set; }
        public string AccessToken { get; set; }
    }
}
