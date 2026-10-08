using System.Collections.Generic;

namespace GenericSpDemo.Models
{
    public class StudentResult
    {
        public List<Student> Students { get; set; } = new List<Student>();
        public int TotalStudents { get; set; }
        public decimal AverageMarks { get; set; }
    }
}