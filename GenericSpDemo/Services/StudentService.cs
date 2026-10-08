using System;
using System.Data;
using GenericSpDemo.Data;
using GenericSpDemo.Models;

namespace GenericSpDemo.Services
{
    public class StudentService
    {
        public StudentResult GetStudents(string course, int minMarks)
        {
            // Empty course means "all courses" -> pass NULL to the stored procedure
            var filter = new StudentFilter
            {
                Course = string.IsNullOrWhiteSpace(course) ? null : course.Trim(),
                MinMarks = minMarks
            };

            // Generic method call -> DataSet
            DataSet ds = DbHelper.GetDataSet("usp_GetStudents", filter);

            var result = new StudentResult();

            // Result set 1 -> List<Student>
            if (ds.Tables.Count > 0)
                result.Students = DbHelper.ToList<Student>(ds.Tables[0]);

            // Result set 2 -> summary
            if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
            {
                DataRow row = ds.Tables[1].Rows[0];
                result.TotalStudents = Convert.ToInt32(row["TotalStudents"]);
                result.AverageMarks = row["AverageMarks"] == DBNull.Value
                                      ? 0
                                      : Convert.ToDecimal(row["AverageMarks"]);
            }

            return result;
        }
    }
}