using Darris_Api.Models.Course;

namespace Darris_Api.Models.Dto
{
    public class CourseDto
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public CourseBookDto Book { get; set; }
        public ICollection<SlidesDto> Slides { get; set; }
        public ICollection<CourseNoteBook> Notebooks { get; set; }
        public ICollection<CourseTestBank> TestBanks { get; set; }
        public ICollection<CourseExplanation> YouTubeVideos { get; set; }

        public ICollection<StudentCourse> StudentCourses { get; set; }
        public ICollection<CourseMajorDto> CourseMajors { get; set; }


    }
}
