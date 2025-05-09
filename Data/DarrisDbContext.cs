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



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<StudentCourse>()
                .HasKey(sc => new { sc.Id, sc.CourseId });

            modelBuilder.Entity<StudentCourse>()
                .HasOne(sc => sc.User)
                .WithMany(u => u.StudentCourses)
                .HasForeignKey(sc => sc.Id);

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
