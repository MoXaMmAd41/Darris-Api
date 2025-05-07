namespace Darris_Api.Models.Course
{
    public class CourseBook
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string FileUrl { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; }
    }
}
