namespace Darris_Api.Data

{
    using Darris_Api.Models;
    using Darris_Api.Models.Course;
    using Microsoft.EntityFrameworkCore;
    public class DarrisDbContext:DbContext
    {
        public DarrisDbContext(DbContextOptions<DarrisDbContext> options)
            : base(options)
        {
        }
        public DbSet<UserInformation>UserInfo {  get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseBook> CourseBooks { get; set; }
        public DbSet<CourseSlide> CourseSlides { get; set; }
        public DbSet<CourseNoteBook> CourseNotebooks { get; set; }
        public DbSet<CourseExplanation> CourseExplanation { get; set; }
        public DbSet<CourseTestBank> CourseTestBank { get; set; }
        public DbSet<CollageClubJoinReq> CollegeClubJoinRequests { get; set; }
        public DbSet<StudentCourse> StudentCourses { get; set; }
        public DbSet<Major> Majors { get; set; }
        public DbSet<CourseMajor> CourseMajors { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<CourseMajor>()
          .HasKey(cm => new { cm.CourseId, cm.MajorId });

            modelBuilder.Entity<CourseMajor>()
                .HasOne(cm => cm.Course)
                .WithMany(c => c.CourseMajors)
                .HasForeignKey(cm => cm.CourseId);

            modelBuilder.Entity<CourseMajor>()
                .HasOne(cm => cm.Major)
                .WithMany(m => m.CourseMajors)
                .HasForeignKey(cm => cm.MajorId);


            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<StudentCourse>()
                .HasKey(sc => new { sc.UserId, sc.CourseId });

            modelBuilder.Entity<StudentCourse>()
                .HasOne(sc => sc.User)
                .WithMany(u => u.StudentCourses)
                .HasForeignKey(sc => sc.UserId);

            modelBuilder.Entity<StudentCourse>()
                .HasOne(sc => sc.Course)
                .WithMany(c => c.StudentCourses)
                .HasForeignKey(sc => sc.CourseId);
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<UserInformation>().HasData(
                new UserInformation
                {
                    Id = 1,
                    FirstName = "Mohammad",
                    LastName = "Yousef",
                    Email = "Mohamadyousef41@gmail.com",
                    Password="Mohamad@2003412",
                    StudentMajor="Cs",
                    PasswordResetToken = "",
                    PasswordResetTokenExpiration = new DateTime(2025, 1, 1)
                });
        }
    }
}
