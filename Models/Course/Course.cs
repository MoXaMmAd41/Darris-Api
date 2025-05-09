namespace Darris_Api.Models.Course
{
    public class Course
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; }

        public CourseBook Book { get; set; }
        public ICollection<CourseSlide> Slides { get; set; }
        public ICollection<CourseNoteBook> Notebooks { get; set; }
        public ICollection<CourseTestBank> TestBanks { get; set; }
        public ICollection<CourseExplanation> YouTubeVideos { get; set; }

        public ICollection<StudentCourse> StudentCourses { get; set; }


    }
}
