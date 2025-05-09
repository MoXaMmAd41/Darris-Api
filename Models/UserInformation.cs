namespace Darris_Api.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.CodeAnalysis;
    using Darris_Api.Models.Course;
    using Darris_Api.Roles;

    public class UserInformation
    {
        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string StudentMajor { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string? PasswordResetToken { get; set; }
        public DateTime? PasswordResetTokenExpiration { get; set; }
        public UserRole Role { get; set; }
        public ICollection<StudentCourse> StudentCourses { get; set; }
        public string? EmailVerificationCode { get; set; }
        public bool IsEmailVerified { get; set; }
        public DateTime? EmailVerificationCodeExpiration { get; set; }

        public string? PasswordResetCode { get; set; }
        public DateTime? PasswordResetCodeExpiration { get; set; }
    }
}
