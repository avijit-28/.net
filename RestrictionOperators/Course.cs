using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestrictionOperators
{
    public class Course
    {
        public string CourseCode { get; set; }
        public List<string> Prerequisites { get; set; }
        // <-- Class ends here

        public static List<Course> enrolledCourses = new List<Course>()
        {
           new Course
                    {
                        CourseCode = "CS301",Prerequisites = new List<string> { "CS101", "CS102" }
                    },
                    new Course
                    {
                        CourseCode = "CS302",Prerequisites = new List<string> { "CS102", "CS201" }
                    },
                    new Course
                    {
                        CourseCode = "CS303",Prerequisites = new List<string> { "CS101", "MATH101" }
                    }
        };
    }
       
}
