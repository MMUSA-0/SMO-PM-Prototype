namespace Framework.Identity.Data.Dtos
{
    public class LoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }

        public string CaptchaText { get; set; }
        public string CaptchaKey { get; set; }

        //public bool IsInternal { get; set; }
    }
}
