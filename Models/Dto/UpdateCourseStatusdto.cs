using Darris_Api.Models.Course;

namespace Darris_Api.Models.Dto
{
    public class UpdateCourseStatusdto
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public CourseStatus NewStatus { get; set; }

    }       
      
}
