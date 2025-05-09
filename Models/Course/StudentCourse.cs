namespace Darris_Api.Models.Course
{
    public class StudentCourse
    {
        public int Id { get; set; }
        public UserInformation User { get; set; }

        public int CourseId { get; set; }
        public Course Course { get; set; }

        public CourseStatus Status { get; set; }

        public string? StudentAdvice { get; set; }

    }
}

