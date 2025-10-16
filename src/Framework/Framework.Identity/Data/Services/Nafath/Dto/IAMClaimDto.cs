
using System;

namespace Framework.Identity.Data.Services.Nafath.Dto
{
    public class IAMClaimDto
    {
        public long Id { get; set; }

        public string? ArFullName { get; set; }
        public string? EnFullName { get; set; }
        public int DobH { get; set; }
        public DateTime DobG { get; set; }
        public char Gender { get; set; }
        public string? ArFirst { get; set; }
        public string? EnFirst { get; set; }
        public string? ArFamily { get; set; }
        public string? EnFamily { get; set; }
        public string? ArFather { get; set; }
        public string? EnFather { get; set; }
        public string? ArGrand { get; set; }
        public string? EnGrand { get; set; }
        public int IdVersion { get; set; }
        public DateTime IdIssueDateG { get; set; }
        public int IdIssueDateH { get; set; }
        public DateTime IdExpiryDateG { get; set; }
        public int IdExpiryDateH { get; set; }
        public int Nationality { get; set; }
        public string? EnNationality { get; set; }
        public string? ArNationality { get; set; }
        public string? Language { get; set; }

        public string? TransId { get; set; }
    }
}

