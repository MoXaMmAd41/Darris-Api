using Darris_Api.Models.Course;

namespace Darris_Api.Data
{
    public class DataBaseSeeder
    {
        public static void SeedCoursesAndMajors(DarrisDbContext context)
        {
            var courseMajorMap = new Dictionary<string, string[]>
{
                    { "التربية الوطنية", new[] { "CS", "SWE", "CIS", "BIT", "CYS", "AI" } },
                    { "English 99", new[] { "CS", "SWE", "CIS", "BIT", "CYS", "AI" } },
                    { "English 101", new[] { "CS", "SWE", "CIS", "BIT", "CYS", "AI" } },
                    { "عربي 101", new[] { "CS", "SWE", "CIS", "BIT", "CYS", "AI" } },
                    { "عربي 99", new[] { "CS", "SWE", "CIS", "BIT", "CYS", "AI" } },
                    { "علوم عسكرية", new[] { "CS", "SWE", "CIS", "BIT", "CYS", "AI" } },
                    { "مهارات حياتية", new[] { "CS", "SWE", "CIS", "BIT", "CYS", "AI" } },
                    { "الريادة والإبداع في تكنولوجيا المعلومات", new[] { "SWE" } },
                    { "القيادة والمسؤولية المجتمعية", new[] { "CS", "SWE", "CIS", "BIT", "CYS", "AI" } },
                    { "حاسوب 99", new[] { "CS", "SWE", "CIS", "BIT", "CYS", "AI" } },
                    { "C++", new[] { "CS", "SWE", "CIS", "BIT", "CYS", "AI" } },
                    { "Discrete", new[] { "CS", "SWE", "CIS", "BIT", "CYS", "AI" } },
                    { "Data Structures", new[] { "CS", "SWE", "CIS", "BIT", "CYS", "AI" } },
                    { "Calculus 1", new[] { "CS", "SWE", "CIS", "BIT", "CYS", "AI" } },
                    { "Lab C++", new[] { "CS", "SWE", "CIS", "BIT", "CYS", "AI" } },
                    { "Logic", new[] { "CS", "SWE", "CIS", "BIT", "CYS", "AI" } },
                    { "Software", new[] { "CS", "SWE", "CIS", "BIT", "CYS", "AI" } },
                    { "مبادئ إحصاء", new[] { "CS", "SWE", "CIS", "BIT", "CYS", "AI" } },
                    { "Java 1", new[] { "CS", "SWE", "CIS", "BIT", "CYS", "AI" } },
                    { "Java 2", new[] { "CS", "CIS", "BIT", "CYS" } },
                    { "VB", new[] { "CS", "CIS", "BIT", "CYS" } },
                    { "Database", new[] { "CS", "SWE", "CIS", "BIT", "CYS", "AI" } },
                    { "Web", new[] { "CS", "CIS", "BIT", "CYS" } },
                    { "Algorithms", new[] { "CS", "SWE", "CIS", "CYS" } },
                    { "Lab Java 1", new[] { "CS", "SWE", "CIS", "BIT", "CYS", "AI" } },
                    { "Lab Java 2", new[] { "CS", "CIS", "CYS" } },
                    { "Lab VB", new[] { "CS", "CIS", "CYS" } },
                    { "Lab Database", new[] { "CS", "SWE", "CIS", "BIT", "CYS", "AI" } },
                    { "Theory", new[] { "CS" } },
                    { "AI", new[] { "CS", "CIS", "CYS" } },
                    { "Analysis", new[] { "CS", "SWE", "CIS", "BIT", "CYS" } },
                    { "Parallel Computing", new[] { "CS" } },
                    { "OS", new[] { "CS", "SWE", "CIS", "CYS" } },
                    { "ORG", new[] { "CS", "CIS", "CYS" } },
                    { "Security CIS", new[] { "CS" } },
                    { "Network", new[] { "CS", "SWE", "CIS", "BIT", "CYS" } },
                    { "Architecture", new[] { "CS" } },
                    { "Multimedia", new[] { "CS", "CIS" } },
                    { "Simulation", new[] { "CS", "BIT" } },
                    { "IP", new[] { "CS", "CIS", "CYS" } },
                    { "GUI", new[] { "CS", "SWE", "CIS" } },
                    { "Data Mining", new[] { "CS", "CIS", "BIT", "AI" } },
                    { "Wireless", new[] { "CS", "CIS", "BIT" } },
                    { "موضوعات خاصة في علم الحاسوب", new[] { "CS" } },
                    { "Network Security", new[] { "CS", "CIS", "BIT" } },
                    { "Physics 1", new[] { "CS" } },
                    { "Calculus 2", new[] { "CS" } },
                    { "Lab Physics 1", new[] { "CS" } },
                    { "بحوث عمليات", new[] { "CS" } },
                    { "Linear", new[] { "CS", "SWE", "CIS", "BIT", "CYS", "AI" } },
                    { "Physics 2", new[] { "CS" } },
                    { "Numerical", new[] { "CS", "SWE" } },
                    { "Java Advance", new[] { "SWE" } },
                    { "VP SWE", new[] { "SWE" } },
                    { "Web SWE", new[] { "SWE" } },
                    { "Design", new[] { "SWE" } },
                    { "Documentation", new[] { "SWE" } },
                    { "Requirements", new[] { "SWE" } },
                    { "Quality", new[] { "SWE" } },
                    { "Testing", new[] { "SWE" } },
                    { "UML", new[] { "SWE", "CIS" } },
                    { "SPM", new[] { "SWE", "CIS", "BIT", "CYS" } },
                    { "ORG SWE", new[] { "SWE" } },
                    { "Security SWE", new[] { "SWE" } },
                    { "Analysis SWE", new[] { "SWE" } },
                    { "Android SWE", new[] { "SWE" } },
                    { "موضوعات خاصة في هندسة البرمجيات", new[] { "SWE" } },
                    { "Cloud SWE", new[] { "SWE" } },
                    { "Maintenance", new[] { "SWE" } },
                    { "Lab Web", new[] { "CIS" } },
                    { "Warehouse", new[] { "CIS", "AI" } },
                    { "IR", new[] { "CIS", "AI" } },
                    { "Database Advance", new[] { "CIS", "BIT", "AI" } },
                    { "E-Commerce", new[] { "CIS" } },
                    { "MIS", new[] { "CIS", "BIT" } },
                    { "Network Management", new[] { "CIS" } },
                    { "ITS", new[] { "CIS" } },
                    { "Web Advance", new[] { "CIS", "BIT", "CYS" } },
                    { "مبادئ اقتصاد جزئي", new[] { "CIS" } },
                    { "مبادئ محاسبة", new[] { "CIS" } },
                    { "مبادئ إدارة 1", new[] { "CIS", "BIT" } },
                    { "مبادئ محاسبة 1", new[] { "CIS", "BIT" } },
                    { "E-Marketing", new[] { "BIT" } },
                    { "E-Services", new[] { "BIT" } },
                    { "DSS", new[] { "BIT" } },
                    { "أمن المعلومات", new[] { "BIT" } },
                    { "إدارة المعرفة", new[] { "BIT" } },
                    { "تكامل الأعمال", new[] { "BIT" } },
                        { "تخطيط موارد الأعمال", new[] { "BIT" } },
                        { "إدارة وتطوير قواعد البيانات الأعمال", new[] { "BIT" } },
                        { "تحليل بيانات الأعمال", new[] { "BIT" } },
                        { "مبادئ مالية", new[] { "BIT" } },
                        { "تطبيقات الذكاء الاصطناعي في الأعمال", new[] { "BIT" } },
                        { "Ethical Hacking", new[] { "CYS" } },
                        { "Lab Ethical Hacking", new[] { "CYS" } },
                        { "Digital Forensics", new[] { "CYS" } },
                        { "Lab Digital Forensics", new[] { "CYS" } },
                        { "Database Security", new[] { "CYS" } },
                        { "مبادئ الأمن السيبراني", new[] { "CYS" } },
                        { "مرتكزات أمن المعلومات", new[] { "CYS" } },
                        { "مقدمة في علم التشفير", new[] { "CYS" } },
                        { "أمن التجارة الإلكترونية", new[] { "CYS" } },
                        { "أمن الشبكات وتطبيقاتها", new[] { "CYS" } },
                        { "تقنيات قواعد البيانات", new[] { "CYS" } },
                        { "Python", new[] { "CYS" } },
                        { "تطبيقات الحماية من الاختراق", new[] { "CYS" } },
                        { "Malicious Software", new[] { "CYS" } },
                        { "Introduction to AI", new[] { "AI" } },
                        { "Python 1", new[] { "AI" } },
                        { "Python 2", new[] { "AI" } },
                        { "Introduction to Data Science", new[] { "AI" } },
                        { "Big Data", new[] { "AI" } },
                        { "Machine Learning", new[] { "AI" } },
                        { "Digital Image Processing", new[] { "AI" } },
                        { "Cloud Computing", new[] { "AI", "BIT" } },
                        { "Robot Programming", new[] { "AI" } },
                        { "Computer Vision", new[] { "AI" } },
                        { "Introduction to Mobile Robots", new[] { "AI" } },
                        { "Natural Language Processing and Text Mining", new[] { "AI" } },
                        { "Business Data Analytics", new[] { "AI" } }
};
            var majorNames = courseMajorMap.SelectMany(c => c.Value).Distinct().ToList();
            var majorsInDb = context.Majors.ToList();

            foreach (var name in majorNames)
            {
                if (!majorsInDb.Any(m => m.MajorName == name))
                {
                    var newMajor = new Major { MajorName = name };
                    context.Majors.Add(newMajor);
                    majorsInDb.Add(newMajor); 
                }
            }

            context.SaveChanges(); 


            foreach (var entry in courseMajorMap)
            {
                var courseName = entry.Key;
                var majorList = entry.Value;

                if (context.Courses.Any(c => c.CourseName == courseName))
                    continue;

                var majors = majorsInDb.Where(m => majorList.Contains(m.MajorName)).ToList();

                var course = new Course
                {
                    CourseName = courseName,
                    CourseMajors = majors.Select(m => new CourseMajor
                    {
                        Major = m
                    }).ToList()
                };

                context.Courses.Add(course);
            }

            context.SaveChanges(); 
        }
    }
}

        

