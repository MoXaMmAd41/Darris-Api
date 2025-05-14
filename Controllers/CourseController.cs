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
using Darris_Api.Migrations;
using Microsoft.Extensions.Options;


namespace Darris_Api.Controllers
{

    [ApiController]
    [Route("api/Darris_Api")]

    public class CourseController : ControllerBase
    {
        private readonly DarrisDbContext _db;
        private readonly IEmailSender _emailSender;
        private readonly EmailSettings _emailSettings;

        public CourseController(DarrisDbContext db, IEmailSender emailSender, IOptions<EmailSettings> emailSettings)
        {
            _db = db;
            _emailSender = emailSender;
            _emailSettings = emailSettings.Value;
        }

        [Authorize]
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



        [Authorize]
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

            string acceptLink = $"https://localhost:7168/api/collegeclub/handle-request?id={joinRequest.Id}&action=accept";
            string rejectLink = $"https://localhost:7168/api/collegeclub/handle-request?id={joinRequest.Id}&action=reject";

            string subject = "New Join Request from " + collageClubJoinReqDto.FirstName;
            string message = $@"
                <h2>New Join Request</h2>
                <p>Student: {collageClubJoinReqDto.FirstName} {collageClubJoinReqDto.LastName}</p>
                <p>GPA: {collageClubJoinReqDto.GPA}</p>
                <p>Status: Pending</p>
                <p><a href='{acceptLink}'>Accept</a> | <a href='{rejectLink}'>Reject</a></p>";

            await _emailSender.SendEmailAsync(_emailSettings.AdminEmail, subject, message);

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

        //================================
      //  [Authorize]
        [HttpGet("GetCourses")]
        public async Task<ActionResult<IEnumerable<CourseDto>>> GetCourses()
        {
            var courses = await _db.Courses
                .Include(c => c.CourseMajors)
                .ThenInclude(cm => cm.Major)
                .ToListAsync();

            var courseDto = courses.Adapt<IEnumerable<CourseDto>>();

            return Ok(courseDto);
        }

        [Authorize]
        [HttpGet("GetCourse/{id}")]
        public async Task<ActionResult<CourseDto>> GetCourse(int id)
        {

            var course = await _db.Courses
                .Include(c => c.Book)
                .Include(c => c.Slides)
                .Include(c => c.Notebooks)
                .Include(c => c.TestBanks)
                .Include(c => c.YouTubeVideos)
                .Include(c => c.CourseMajors)
                .ThenInclude(cm => cm.Major)
                .FirstOrDefaultAsync(c => c.CourseId == id);

            if (course == null)
            {
                return NotFound("course not found");
            }

            var courseDto = course.Adapt<CourseDto>();
            return Ok(courseDto);
        }

        [Authorize(Roles = "CollegeClub")]
        [HttpPost("CreateCourse")]
        public async Task<ActionResult> CreateCourse(CourseCreateDto coursedto)
        {
            if (coursedto == null || coursedto.MajorIds == null || !coursedto.MajorIds.Any())
            {
                return BadRequest("Invalid course data or majors not provided.");
            }

            bool courseExists = await _db.Courses
                .AnyAsync(u => u.CourseName.ToLower().Trim() == coursedto.CourseName.ToLower().Trim());

            if (courseExists)
            {
                return BadRequest("Course already exists.");
            }

            var course = new Course
            {
                CourseName = coursedto.CourseName.Trim(),
                CourseMajors = coursedto.MajorIds.Select(mid => new CourseMajor { MajorId = mid }).ToList()
            };

            _db.Courses.Add(course);
            await _db.SaveChangesAsync();

            return Ok("Course created successfully.");
        }

        [Authorize(Roles = "CollegeClub")]
        [HttpDelete("DeleteCourse/{id}")]
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

        // [Authorize(Roles = "CollegeClub")]
        [HttpPost("{courseId}/book")]
        public async Task<IActionResult> AddCourseBook(int courseId, CourseBookDto dto)
        {
            var book = new CourseBook
            {
                CourseId = courseId,
                FileUrl = dto.FileUrl
            };

            _db.CourseBooks.Add(book);
            await _db.SaveChangesAsync();

            return Ok(book);
        }
        
            

            [Authorize]
        [HttpGet("{courseId}/book")]
        public async Task<ActionResult<CourseBook>> GetCourseBook(int courseId)
        {
            var book = await _db.CourseBooks.FirstOrDefaultAsync(b => b.CourseId == courseId);
            if (book == null)
            {
                return NotFound("book does not exist");
            }
            return Ok(book);
        }


        [Authorize]
        [HttpGet("{courseId}/notebooks")]
        public async Task<ActionResult<IEnumerable<CourseNoteBook>>> GetNotebooks(int courseId)
        {
            var notebooks = await _db.CourseNotebooks.Where(n => n.CourseId == courseId).ToListAsync();
            return Ok(notebooks);
        }

        [HttpPost("{courseId}/notebook")]
        public async Task<IActionResult> AddCourseNotebook(int courseId, NoteBookDto noteBookDto)
        {
            var notebook = new CourseNoteBook
            {
                CourseId = courseId,
                FileUrl = noteBookDto.FileUrl
            };

            _db.CourseNotebooks.Add(notebook);
            await _db.SaveChangesAsync();

            return Ok(notebook);
        }


        [Authorize]
        [HttpGet("{courseId}/slides")]
        public async Task<ActionResult<IEnumerable<CourseSlide>>> GetSlides(int courseId)
        {
            var slides = await _db.CourseSlides.Where(s => s.CourseId == courseId).ToListAsync();
            return Ok(slides);
        }



        [Authorize(Roles = "CollegeClub")]
        [HttpPost("{courseId}/slide")]
        public async Task<IActionResult> AddCourseSlide(int courseId, SlidesDto slidesDto)
        {
            var slide = new CourseSlide
            {
                CourseId = courseId,
                FileUrl = slidesDto.FileUrl
            };

            _db.CourseSlides.Add(slide);
            await _db.SaveChangesAsync();

            return Ok(slide);
        }



        [Authorize]
        [HttpGet("{courseId}/videos")]
        public async Task<ActionResult<IEnumerable<CourseExplanation>>> GetVideos(int courseId)
        {
            var videos = await _db.CourseExplanation.Where(v => v.CourseId == courseId).ToListAsync();
            return Ok(videos);
        }


        [Authorize(Roles = "CollegeClub")]
        [HttpPost("{courseId}/youtube")]
        public async Task<IActionResult> AddYouTubeVideo(int courseId, YouToubeDto youToubeDto)
        {
            var video = new CourseExplanation
            {
                CourseId = courseId,
                Url = youToubeDto.Url
            };

            _db.CourseExplanation.Add(video);
            await _db.SaveChangesAsync();

            return Ok(video);
        }



        [Authorize]
        [HttpGet("{courseId}/testbanks")]
        public async Task<ActionResult<IEnumerable<CourseTestBank>>> GetTestBanks(int courseId)
        {
            var testBanks = await _db.CourseTestBank.Where(t => t.CourseId == courseId).ToListAsync();
            return Ok(testBanks);
        }



        [Authorize(Roles = "CollegeClub")]
        [HttpPost("{courseId}/testbank")]
        public async Task<IActionResult> AddCourseTestBank(int courseId, TestBankDto testBankDto)
        {
            var testBank = new CourseTestBank
            {
                CourseId = courseId,
                FileUrl = testBankDto.FileUrl
            };

            _db.CourseTestBank.Add(testBank);
            await _db.SaveChangesAsync();

            return Ok(testBank);
        }

        [Authorize]
        [HttpPut("update-status/{userId}/{courseId}")]
        public async Task<IActionResult> UpdateCourseStatus(int userId, int courseId, CourseStatus newStatus)
        {
            var studentCourse = await _db.StudentCourses
                .FirstOrDefaultAsync(sc => sc.UserId == userId && sc.CourseId == courseId);

            if (studentCourse == null)
            {
                return NotFound("Student-course relationship not found.");
            }

            studentCourse.Status = newStatus;
            await _db.SaveChangesAsync();

            return Ok("Course status updated successfully.");
        }

        [Authorize]
        [HttpGet("SearchBar")]
        public async Task<ActionResult<IEnumerable<Course>>> SearchCourses([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("Search query is required.");
            }
            var courses = await _db.Courses.Where(c => c.CourseName.ToLower().Contains(query.Trim().ToLower())).ToListAsync();
            return Ok(courses);
        }



        [Authorize]
        [HttpPost("course/{id}/{courseId}/advice")]
        public async Task<IActionResult> SubmitAdvice(int id, int courseId, [FromBody] string advice)
        {
            var course = await _db.StudentCourses.FindAsync(id, courseId); 

            if (course == null)
                return NotFound("Course not found");

            if (course.Status != CourseStatus.Completed)
                return BadRequest("You can only submit advice after completing the course.");

            course.StudentAdvice = advice;
            await _db.SaveChangesAsync();

            return Ok("Advice submitted successfully");

        }
    }
}


