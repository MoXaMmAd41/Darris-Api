﻿namespace Darris_Api.Models.Dto
{
    public class PasswordResetDto
    {
        public string Email { get; set; }
        public string Code { get; set; }
        public string NewPassword { get; set; }
    }
}
