
namespace Framework.Identity.Data.Services.Nafath.Dto
{
    public class CheckRequestStatusResponseDto
    {
        public string? Status { get; set; }
        public IAMClaimDto Person { get; set; } = new IAMClaimDto();

        //in case error response
        public string? Code { get; set; }
        public string? RequestedURL { get; set; }
        public string? Message { get; set; }
        public string? Trace { get; set; }
    }
}

