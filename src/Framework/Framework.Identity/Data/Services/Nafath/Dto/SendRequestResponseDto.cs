
namespace Framework.Identity.Data.Services.Nafath.Dto
{
    public class SendRequestResponseDto
    {
        public string TransId { get; set; }
        public string Random { get; set; }

        //in case error response
        public string Code { get; set; }
        public string RequestedURL { get; set; }
        public string Message { get; set; }
        public string Trace { get; set; }
    }
}

