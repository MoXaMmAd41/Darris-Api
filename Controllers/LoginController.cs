using Microsoft.AspNetCore.Mvc;
using Darris_Api.Data;
using Darris_Api.Email_Config;
using Darris_Api.Models;
using Darris_Api.Models.Dto;
using Mapster;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Darris_Api.PasswordHashed;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Darris_Api.Roles;
using Microsoft.AspNetCore.Authorization;
using Darris_Api.Models.Course;
using Darris_Api.Migrations;
using Microsoft.Extensions.Options;

namespace Darris_Api.Controllers
{
    [ApiController]
    [Route("api/Darris_Api")]
    public class LoginController : ControllerBase
    {
        private readonly DarrisDbContext _db;
        private readonly IEmailSender _emailSender;
        private readonly EmailSettings _emailSettings;

        public LoginController(DarrisDbContext db, IEmailSender emailSender, IOptions<EmailSettings> emailSettings)
        {
            _db = db;
            _emailSender = emailSender;
            _emailSettings = emailSettings.Value;
        }

        [HttpPost("create-account")]
        public async Task<ActionResult<string>> CreateAccount(UserInformationDto userInformationDto)
        {
            if (userInformationDto == null)
                return BadRequest();

            if (await _db.UserInfo.AnyAsync(u => u.Email.ToLower() == userInformationDto.Email.ToLower()))
            {
                ModelState.AddModelError("", "User already exists");
                return BadRequest();
            }

            var user = userInformationDto.Adapt<UserInformation>();
            user.Password = PasswordHelper.HashPassword(user.Password);
            user.Role = UserRole.Student;
            user.IsEmailVerified = false;

            var code = new Random().Next(100000, 999999).ToString();
            user.EmailVerificationCode = code;
            user.EmailVerificationCodeExpiration = DateTime.UtcNow.AddMinutes(10);

            await _db.AddAsync(user);
            await _db.SaveChangesAsync();

            string subject = "Verify Your Email - Darris";
            string message = $@"
            <h2>Hi {user.FirstName},</h2>
            <p>Thank you for registering on <strong>Darris</strong>.</p>
            <p>Your verification code is:</p>
            <h3>{code}</h3>
            <p>This code expires in 10 minutes.</p>
            <br />
            <p>- The Darris Team</p>";

            await _emailSender.SendEmailAsync(user.Email, subject, message);

            return Ok("Account created. Please check your email for the verification code.");
        }
        [HttpPost("verify-email-code")]
        public async Task<IActionResult> VerifyEmailCode([FromBody] EmailCodeVerificationDto dto)
        {
            var user = await _db.UserInfo.FirstOrDefaultAsync(u =>
                u.Email.ToLower() == dto.Email.ToLower() &&
                u.EmailVerificationCode == dto.Code &&
                u.EmailVerificationCodeExpiration > DateTime.UtcNow);

            if (user == null)
                return BadRequest("Invalid or expired verification code.");

            user.IsEmailVerified = true;
            user.EmailVerificationCode = null;
            user.EmailVerificationCodeExpiration = null;

            await _db.SaveChangesAsync();

            return Ok("Email verified successfully.");
        }

        [HttpPost("resend-verification-code")]
        public async Task<IActionResult> ResendVerificationCode([FromBody] ResendVerificationCodeDto dto)
        {
            var user = await _db.UserInfo.FirstOrDefaultAsync(u =>
                u.Email.ToLower() == dto.Email.ToLower() && !u.IsEmailVerified);

            if (user == null)
                return BadRequest("User not found or already verified.");

            var code = new Random().Next(100000, 999999).ToString();
            user.EmailVerificationCode = code;
            user.EmailVerificationCodeExpiration = DateTime.UtcNow.AddMinutes(10);
            await _db.SaveChangesAsync();

            string subject = "Your New Verification Code - Darris";
            string message = $@"
            <h2>Hi {user.FirstName},</h2>
            <p>Here is your new email verification code:</p>
            <h3>{code}</h3>
            <p>This code will expire in 10 minutes.</p>
            <br />
            <p>- The Darris Team</p>";

            await _emailSender.SendEmailAsync(user.Email, subject, message);

            return Ok("A new verification code has been sent to your email.");
        }


        [HttpPost("Login")]
        public async Task<ActionResult<UserInformationDto>> Login([FromBody] LoginDto loginDto)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var User = await _db.UserInfo.FirstOrDefaultAsync(u => u.Email.ToLower() == loginDto.Email.ToLower());

            if (User == null)
            {
                return Unauthorized("Invalid email or password");
            }
            if (User.Password != loginDto.Password)

            {
                return Unauthorized("Incorrect password");

            }
            var informationAboutUser = User.Adapt<UserInformationDto>();
            informationAboutUser.Password = null;
            var token = GenerateJwtToken(User);
            return Ok(new
            {
                informationAboutUser = User,
                Token = token
            });

        }




        private string GenerateJwtToken(UserInformation user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("MySuperSecureAndRandomKeyThatlooksJustAwesomeAndNeedsVeryLong"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
  {
      new Claim("FirstName",user.FirstName),
      new Claim("LastName",user.LastName),
      new Claim("Email", user.Email),
      new Claim("StudentMajor", user.StudentMajor),
      new Claim("UserId", user.Id.ToString()),
      new Claim(ClaimTypes.Role,UserRole.Student.ToString())
  };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }




        [HttpPost("request-password-reset")]
        public async Task<IActionResult> RequestPasswordReset([FromBody] PasswordRequestResetDto passwordRequestResetDto)
        {
            var user = await _db.UserInfo.FirstOrDefaultAsync(u => u.Email.ToLower() == passwordRequestResetDto.Email.ToLower());
            if (user == null)
            {
                return NotFound("Email not found");
            }

            var code = new Random().Next(100000, 999999).ToString();
            user.PasswordResetCode = code;
            user.PasswordResetCodeExpiration = DateTime.UtcNow.AddMinutes(10);
            await _db.SaveChangesAsync();

            string subject = "Your Password Reset Code - Darris";
            string message = $@"
            <h2>Hi {user.FirstName},</h2>
            <p>You requested to reset your password.</p>
            <p>Use the following code to reset your password (valid for 10 minutes):</p>
            <h3>{code}</h3>
            <p>If you didn't request this, please ignore it.</p>
            <br />
            <p>- Darris Team</p>";

            await _emailSender.SendEmailAsync(user.Email, subject, message);

            return Ok("Verification code sent to your email.");
        }
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] PasswordResetDto passwordResetDto)
        {
            var user = await _db.UserInfo.FirstOrDefaultAsync(u =>
                u.Email.ToLower() == passwordResetDto.Email.ToLower() &&
                u.PasswordResetCode == passwordResetDto.Code &&
                u.PasswordResetCodeExpiration > DateTime.UtcNow);

            if (user == null)
            {
                return BadRequest("Invalid or expired code.");
            }

            user.Password = PasswordHelper.HashPassword(passwordResetDto.NewPassword);
            user.PasswordResetCode = null;
            user.PasswordResetCodeExpiration = null;
            await _db.SaveChangesAsync();

            return Ok("Password has been reset successfully.");
        }
    }
}
