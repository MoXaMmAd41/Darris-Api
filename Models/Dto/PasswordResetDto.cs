namespace Darris_Api.Models.Dto
{
    public class PasswordResetDto
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public string NewPassword { get; set; }
    }
}
