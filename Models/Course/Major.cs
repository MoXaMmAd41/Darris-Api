namespace Darris_Api.Models.Course
{
    public class Major
    {
        public int MajorId { get; set; }
        public string MajorName { get; set; }

        public ICollection<CourseMajor> CourseMajors { get; set; }
    }
}
