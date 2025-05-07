namespace Darris_Api.Models.Course
{
    public class CourseExplanation
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; }
    }
}
