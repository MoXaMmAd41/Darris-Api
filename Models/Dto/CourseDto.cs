using System.ComponentModel.DataAnnotations;

namespace Darris_Api.Models.Dto
{
    public class CourseDto
    {
        [Required]
        public string CourseName { get; set; }
        public string? StudentAdvice { get; set; }
    }
}
