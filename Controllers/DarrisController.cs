using Darris_Api.Data;
using Darris_Api.Email_Config;
using Darris_Api.Models;
using Darris_Api.Models.Dto;
using Mapster;
using Microsoft.AspNetCore.Mvc;
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


namespace Darris_Api.Controllers
{
    [ApiController]
    [Route("api/Darris_Api")]

    public class DarrisController : ControllerBase
    {
        private readonly DarrisDbContext _db;
        private readonly IEmailSender _emailSender;

        public DarrisController(DarrisDbContext db, IEmailSender emailSender)
        {
            _db = db;
            _emailSender = emailSender;
        }





        [HttpPost("Create account")]
        public async Task<ActionResult<UserInformationDto>> CreateAccount(UserInformationDto userInformationDto)
        {
            if (userInformationDto == null)
            {
                return BadRequest();
            }

            if (await _db.UserInfo.AnyAsync(u => u.Email.ToLower() == userInformationDto.Email.ToLower()))
            {
                ModelState.AddModelError("", "User Already Exist");
                return BadRequest();
            }

            var UserInform = userInformationDto.Adapt<UserInformation>();
            UserInform.Password = PasswordHelper.HashPassword(UserInform.Password);
            UserInform.Role = UserRole.Student;
            await _db.AddAsync(UserInform);
            await _db.SaveChangesAsync();
            string subject = "welcome to darris";
            string message = $@"
            <h2>Hi {UserInform.FirstName},</h2>
            <p>Thank you for registering on <strong>Darris</strong>.</p>
            <p>Start browsing courses and student reviews now!</p>
            <br />
            <p>- The Darris Team</p>";

            await _emailSender.SendEmailAsync(UserInform.Email, subject, message);


            return Ok(UserInform);
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




        [HttpPost("request password reset")]
        public async Task<IActionResult> RequestPasswordReset([FromBody] PasswordRequestResetDto passwordRequestResetDto)
        {
            var User = await _db.UserInfo.FirstOrDefaultAsync(u => u.Email.ToLower() == passwordRequestResetDto.Email.ToLower());
            if (User == null)
            {
                return NotFound("Email not found");
            }
            var token = GenaratePasswordResetToken(User);
            User.PasswordResetToken = token;
            User.PasswordResetTokenExpiration = DateTime.UtcNow.AddMinutes(10);
            await _db.SaveChangesAsync();
            var resetLink = $"http://localhost:5173/reset-password/{token}";
            string subject = "Reset your password - Darris";
            string message = $@"
        <h2>Hi {User.FirstName},</h2>
        <p>You requested to reset your password.</p>
        <p><a href='{resetLink}'>Click here to reset it</a>. This link will expire in 30 minutes.</p>
        <br />
        <p>- Darris Team</p>";

            await _emailSender.SendEmailAsync(User.Email, subject, message);

            return Ok("Reset link sent to your email.");

        }




        private string GenaratePasswordResetToken(UserInformation user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("MySuperSecureAndRandomKeyThatlooksJustAwesomeAndNeedsVeryLong"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
                new Claim("Email",user.Email),
                new Claim("UserId",user.Id.ToString())
            };
            var Token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(10),
                signingCredentials: creds
                );
            return new JwtSecurityTokenHandler().WriteToken(Token);
        }





        [HttpPost("Reset Password")]
        public async Task<IActionResult> ResetPassword([FromBody] PasswordResetDto passwordResetDto)
        {
            var user = await _db.UserInfo.FirstOrDefaultAsync(u => u.Email.ToLower() ==
            passwordResetDto.Email.ToLower() && u.PasswordResetToken == passwordResetDto.Token &&
            u.PasswordResetTokenExpiration > DateTime.UtcNow);
            if (user == null)
            {
                return BadRequest("invalid");
            }
            user.Password = PasswordHelper.HashPassword(passwordResetDto.NewPassword);
            user.PasswordResetToken = null;
            user.PasswordResetTokenExpiration = null;
            await _db.SaveChangesAsync();
            return Ok("Password has been reset successfully.");
        }



        [HttpPut("UpdateUserInfo/{id}")]
        public async Task<IActionResult> UpdateUserInfo(int id, [FromBody] UpdateDto updatedInfo)
        {
            var user = await _db.UserInfo.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return NotFound("User not found");
            }

            user.FirstName = updatedInfo.FirstName;
            user.LastName = updatedInfo.LastName;
            user.StudentMajor = updatedInfo.StudentMajor;

            await _db.SaveChangesAsync();
            return Ok("User info updated successfully");
        }




        [HttpPost("joinCollageClub")]
        public async Task<IActionResult> JoinCollegeClub([FromBody] CollageClubJoinReqDto collageClubJoinReqDto)
        {
            var student = await _db.UserInfo.FirstOrDefaultAsync(u => u.Id == collageClubJoinReqDto.StudentId);
            if (student == null)
            {
                return NotFound("student not found");
            }
            var joinRequest = new CollageClubJoinReq
            {
                FirstName = collageClubJoinReqDto.FirstName,
                LastName = collageClubJoinReqDto.LastName,
                GPA = collageClubJoinReqDto.GPA,
                Status = JoinCollageClubReqStatus.Pending,
                StudentId = student.Id
            };
            await _db.CollegeClubJoinRequests.AddAsync(joinRequest);
            await _db.SaveChangesAsync();
            string subject = "New Join Request from " + collageClubJoinReqDto.FirstName;
            string message = $@"
                <h2>New Join Request</h2>
                 <p>Student: {collageClubJoinReqDto.FirstName} {collageClubJoinReqDto.LastName}</p>
                 <p>GPA: {collageClubJoinReqDto.GPA}</p>
                    <p>Status: Pending</p>";
            await _emailSender.SendEmailAsync(joinRequest.Student.Email, subject, message);
            return Ok("Your request has been sent to the admin for review.");

        }




        [Authorize(Roles = "Admin")]
        [HttpPut("ReviewJoinRequest/{requestId}")]
        public async Task<IActionResult> ReviewJoinRequest(int requestId, [FromBody] JoinCollageClubReqStatus newStatus)
        {
            var joinRequest = await _db.CollegeClubJoinRequests.FirstOrDefaultAsync(u => u.Id == requestId);
            if (joinRequest == null)
            {
                return NotFound("Join request not found.");
            }

            if (joinRequest.Status != JoinCollageClubReqStatus.Pending)
            {
                return BadRequest("This request has already been reviewed.");
            }

            joinRequest.Status = newStatus;

            if (newStatus == JoinCollageClubReqStatus.Accepted)
            {
                var student = await _db.UserInfo.FirstOrDefaultAsync(u => u.Id == joinRequest.StudentId);
                if (student != null)
                {
                    student.Role = UserRole.CollegeClub;
                    await _db.SaveChangesAsync();
                }
            }

            await _db.SaveChangesAsync();
            string subject = "Your College Club Join Request Status";
            string message = $@"
                <h2>Hi {joinRequest.FirstName},</h2>
                <p>Your request to join the College Club has been {newStatus}.</p>
                <br />
                 <p>- The Darris Team</p>";

            await _emailSender.SendEmailAsync(joinRequest.Student.Email, subject, message);

            return Ok($"Join request status updated to {newStatus}");

        }



        [HttpGet]
        public async Task<ActionResult<IEnumerable<Course>>> GetCourses()
        {
            return await _db.Courses.ToListAsync();
        }



        public async Task<ActionResult<Course>> GetCourse(int id)
        {
            var course = await _db.Courses
                .Include(c => c.Book)
                .Include(c => c.Slides)
                .Include(c => c.Notebooks)
                .Include(c => c.TestBanks)
                .Include(c => c.YouTubeVideos)
                .FirstOrDefaultAsync(c => c.CourseId == id);
            if (course == null)
            {
                return NotFound("course not found");
            }

            return Ok(course);
        }
        [HttpPost]
        public async Task<ActionResult> CreateCourse(CourseDto coursedto)
        {
            if (coursedto == null)
            {
                return BadRequest("Invalid course data.");
            }

            bool courseExists = await _db.Courses
                .AnyAsync(u => u.CourseName.ToLower().Trim() == coursedto.CourseName.ToLower().Trim());

            if (courseExists)
            {
                return BadRequest("Course already exists.");
            }

            var course = new Course
            {
                CourseName = coursedto.CourseName.Trim()
            };

            _db.Courses.Add(course);
            await _db.SaveChangesAsync();

            return Ok("Course created successfully.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var course = await _db.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            _db.Courses.Remove(course);
            await _db.SaveChangesAsync();
            return NoContent();
        }


    }

}


